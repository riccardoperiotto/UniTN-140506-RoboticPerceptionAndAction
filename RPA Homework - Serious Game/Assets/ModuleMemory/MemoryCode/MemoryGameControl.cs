using Lean.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryGameControl : MonoBehaviour
{
    GameObject screen;
    public GameObject cardPrefab;
    MemoryScene_GazeableCard tokenUp1 = null;
    MemoryScene_GazeableCard tokenUp2 = null;
    //randomization of the cards
    List<int> faceIndexes = new List<int> { 0, 1, 2, 3, 4, 0, 1, 2, 3, 4 };
    public static System.Random rnd = new System.Random();
    public int shuffleNum = 0;
    int[] visibleFaces = { -1, -2 }; //tracking the visible cards
    private int clickCount = 0;


    void Start()
    {
        //positioning of clone cards: we have one game object set in position on the screen
        //when the game starts it maxes the clones in 2x5 batches
        int originalLength = faceIndexes.Count;
        float yPosition = 190f;
        float xPosition = -200f;
        for (int i = 0; i < 9; i++)
        {
            shuffleNum = rnd.Next(0, (faceIndexes.Count));
            var temp = Instantiate(cardPrefab, new Vector3(xPosition, yPosition, 0), Quaternion.identity);

            temp.GetComponent<LeanButton>().OnClick.AddListener(delegate
            {
                temp.GetComponent<MemoryScene_GazeableCard>().Flip();
            });

            // sets up the parenting and do it on the local frame
            temp.transform.SetParent(screen.transform, false);
            temp.GetComponent<MemoryScene_GazeableCard>().faceIndex = faceIndexes[shuffleNum];

            //allows only one (pair) occurrence for the indicies
            faceIndexes.Remove(faceIndexes[shuffleNum]);
            xPosition = xPosition + 210;
            if (i == (originalLength / 2 - 2))
            {
                yPosition = -30f;
                xPosition = -400f;
            }
        }
        cardPrefab.GetComponent<MemoryScene_GazeableCard>().faceIndex = faceIndexes[0];

    }

    //Prevent the user to flip up more than two cards at a time
    public bool TwoCardsUp()
    {
        bool cardsUp = false;
        if (visibleFaces[0] >= 0 && visibleFaces[1] >= 0)
        {
            cardsUp = true;
        }
        return cardsUp;
    }

    public void TokenDown(MemoryScene_GazeableCard tempToken)
    {
        if (tokenUp1 == tempToken)
        {
            tokenUp1 = null;
        }
        else if (tokenUp2 == tempToken)
        {
            tokenUp2 = null;
        }
    }

    public bool TokenUp(MemoryScene_GazeableCard tempToken)
    {
        bool flipCard = true;
        if (tokenUp1 == null)
        {
            tokenUp1 = tempToken;
        }
        else if (tokenUp2 == null)
        {
            tokenUp2 = tempToken;
        }
        else
        {
            flipCard = false;
        }
        return flipCard;
    }

    //Observe the matching pairs and freeze them so they can't be flipped over again
    public void CheckTokens()
    {
        clickCount++;
        if (tokenUp1 != null && tokenUp2 != null &&
            tokenUp1.faceIndex == tokenUp2.faceIndex)
        {
            tokenUp1.identicals = true;
            tokenUp2.identicals = true;
            tokenUp1 = null;
            tokenUp2 = null;
        }
    }

    private void Awake()
    {
        screen = GameObject.Find("Screen 2");
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (Input.GetMouseButtonDown(0))
        {
            if (hit.collider != null)
            {
                // change the color
                if (hit.collider.CompareTag("Card"))
                {
                    hit.collider.transform.GetComponent<MemoryScene_GazeableCard>().Flip();
                }
            }

        }
    }

}
