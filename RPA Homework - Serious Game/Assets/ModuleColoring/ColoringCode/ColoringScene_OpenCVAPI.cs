using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColoringScene_OpenCVAPI
{
    public static List<Texture2D> SegmentTexture(Texture2D rawTexture, float width, float height)
    {
        List<Texture2D> segmentTextures = new List<Texture2D>();

        // (1) Preprocess the raw image
        Mat imgRaw = OpenCvSharp.Unity.TextureToMat(rawTexture);
        var oldImageHeight = imgRaw.Height;
        var oldImageWidth = imgRaw.Width;
        var channels = imgRaw.Channels();

        // create new image of desired size and color (transparent) for padding
        var imageBiggestSide = Mathf.Max(oldImageHeight, oldImageWidth);
        var contourAreaThreshold = imageBiggestSide * imageBiggestSide * 0.00001;
        Mat img = new Mat(new Size(imageBiggestSide, imageBiggestSide), MatType.CV_8UC3, new Scalar(255.0, 255.0, 255.0));

        // Copy img image into center of result image
        var paddingY = (imageBiggestSide - oldImageHeight) / 2;
        var paddingX = (imageBiggestSide - oldImageWidth) / 2;

        img[paddingY, paddingY + oldImageHeight, paddingX, paddingX + oldImageWidth] = imgRaw;
        Mat imgGray = new Mat();
        Cv2.CvtColor(img, imgGray, ColorConversionCodes.BGR2GRAY);

        // (2) Threshold
        Mat threshed = new Mat();
        Cv2.Threshold(imgGray, threshed, 30, 255, ThresholdTypes.BinaryInv | ThresholdTypes.Otsu);

        // (3) Find the min-area contour
        Mat[] contours = new List<Mat>().ToArray();
        OutputArray hierarchy = new OutputArray(new Mat());
        Cv2.FindContours(threshed, out contours, hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);
        contours = contours.OrderByDescending(x => x.ContourArea()).ToArray();

        for (int i = 1; i < contours.Length && contourAreaThreshold < contours[i].ContourArea(); i++)
        {
            // (4) Create mask and do bitwise-op
            Mat contourImage = new Mat(new Size(imageBiggestSide, imageBiggestSide), MatType.CV_8UC1, 0);
            Cv2.DrawContours(contourImage, new List<Mat>() { contours[i] }, -1, 255, -1);
            Mat segmentImage = new Mat();
            Cv2.BitwiseAnd(img, img, segmentImage, contourImage);

            //// (5) Rescale the image appropriately
            Cv2.Resize(segmentImage, segmentImage, new Size(width, height));

            //// (6) Choose the background color (which is black so far, but we want it to be transparent) and add the image to the list to return 
            Mat alpha = new Mat();
            Cv2.CvtColor(segmentImage, alpha, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(alpha, alpha, 0, 255, ThresholdTypes.Binary);

            Mat[] split = new List<Mat>().ToArray();
            Cv2.Split(segmentImage, out split);
            Cv2.Merge(new Mat[] { split[0], split[1], split[2], alpha }, segmentImage);

            Texture2D segmentTexture = new Texture2D(1, 1, TextureFormat.RGBA32, true); // 1, 1 are just symbolic
            segmentTexture = OpenCvSharp.Unity.MatToTexture(segmentImage);
            segmentTexture.name = $"Segment{i}";
            segmentTextures.Add(segmentTexture);
        }

        // (7) Extract the edges
        Mat edgesMask = new Mat();
        Cv2.Threshold(imgGray, edgesMask, 30, 255, ThresholdTypes.Binary);

        Mat edgesAlpha = new Mat();
        Cv2.Threshold(edgesMask, edgesAlpha, 0, 255, ThresholdTypes.BinaryInv);

        Mat[] edgesSplit = new List<Mat>().ToArray();
        Mat edgesImage = new Mat();
        Cv2.Split(img, out edgesSplit);
        Cv2.Merge(new Mat[] { edgesSplit[0], edgesSplit[1], edgesSplit[2], edgesAlpha }, edgesImage);
        Cv2.Resize(edgesImage, edgesImage, new Size(width, height));

        Texture2D edgesTexture = new Texture2D(1, 1, TextureFormat.RGBA32, true);
        edgesTexture = OpenCvSharp.Unity.MatToTexture(edgesImage);
        edgesTexture.name = "Edges";
        segmentTextures.Add(edgesTexture);

        return segmentTextures;
    }

}