using Lean.Gui;
using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ColoringScene_ImageSelection : MonoBehaviour
{
    public GameObject segmentPrefab;

    void Start()
    {
        // Set filters (optional)
        // It is sufficient to set the filters just once (instead of each time before showing the file browser dialog), 
        // if all the dialogs will be using the same filters
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png"));

        GameObject.Find("SelectImageButton").GetComponent<LeanButton>().OnClick.AddListener(delegate
        {
            StartCoroutine(LoadFileFromFileSystem());
        });
    }

    // Update is called once per frame
    void Update()
    {

    }


    IEnumerator LoadFileFromFileSystem()
    {

        // Show a load file dialog and wait for a response from user
        // Load file/folder: both, Allow multiple selection: true
        // Initial path: default (Documents), Initial filename: empty
        // Title: "Load File", Submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, false, null, null, "Load Files and Folders", "Load");

        if (FileBrowser.Success)
        {
            // process the texture
            Transform drawingContainerTransform = GameObject.Find("ColoringCanvas").transform;

            foreach (Transform child in drawingContainerTransform.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            // Read the bytes of the first file via FileBrowserHelpers
            // Contrary to File.ReadAllBytes, this function works on Android 10+, as well
            byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result[0]);

            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);

            float size = Mathf.Min(drawingContainerTransform.GetComponent<RectTransform>().rect.width, drawingContainerTransform.GetComponent<RectTransform>().rect.height);
            List<Texture2D> textureSegments = ColoringScene_OpenCVAPI.SegmentTexture(texture, size, size);


            foreach (Texture2D textureSegment in textureSegments)
            {
                GameObject segmentObject = (GameObject)Instantiate(segmentPrefab, transform);
                segmentObject.name = textureSegment.name;
                if (segmentObject.name == "Edges") { segmentObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0); }
                segmentObject.transform.SetParent(drawingContainerTransform);

                RectTransform segmentRectTransform = segmentObject.GetComponent<RectTransform>();
                segmentRectTransform.anchorMin = new Vector2(0, 0);
                segmentRectTransform.anchorMax = new Vector2(1, 1);
                segmentRectTransform.pivot = new Vector2(0.5f, 0.5f);
                segmentRectTransform.offsetMin = new Vector2(0, 0);
                segmentRectTransform.offsetMax = new Vector2(0, 0);

                Sprite s = Sprite.Create(textureSegment, new Rect(0, 0, textureSegment.width, textureSegment.height), new Vector2(0.5f, 0.5f), 1, 1, SpriteMeshType.Tight);
                segmentObject.GetComponent<SpriteRenderer>().sprite = s;
                segmentObject.AddComponent<PolygonCollider2D>();
                segmentObject.AddComponent<ColoringScene_GazeableSegment>();
                segmentObject.AddComponent<GazeableObject>();

                // update the label on the screen
                GameObject.Find("ImageName").GetComponent<Text>().text = FileBrowser.Result[0].Split(new char[] { '\\' }).ToList().Last();
            }
        }
    }

}

