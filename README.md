# Robotic Perception and Action, UniTN [140506]
Project developed for the Robotic Perception and Action (RPA) course of the master's degree in Autonomous Systems (EIT), University of Trento.

## Information
Course professor: Mariolino De Cecco

Course website: https://www.miro.ing.unitn.it/

The course covers four main areas of study: Mobile Robots, Mixed Reality, Sensor Fusion, and (some) Machine Learning. To pass the course, in addition to the theory behind these fields, the students can develop a project (called *Homework*) connected to one or more topics discussed and presented. In the year 2021/2022, one of the topics proposed was the development of a serious game for children with reduced moving capabilities. This repository contains the solution I proposed.

## Description
The project consists of a set of small video games developed in Unity. The user provides the commands with the eyes, via a Tobii Eye Tracker 5. The minigames can be integrated into a bigger video game developed by the research laboratory and follow the same best practices taught in the course. For example, each scene corresponds to a state, and the architecture is based on a Finite State Machine (FSM).

Each main functionality is contained in a separate module. Each scene, which often corresponds to a mini-game, is in an independent folder containing the scene object, the code, the documentation folder, an image folder, the prefabs, and the sprites. With this structure, to include a module in a project is enough to copy the folder containing it and add the scene to the Build settings. Another thing to consider is that all modules rely on a shared module containing the logic of the FSM called General.

With this overview in mind, it is possible to inspect each module a bit more closely.

### General
It contains the parts shared between the different modules, organized in the folders described in the following.


The **Data** folder contains some json files used for different purposes.
- GameData contains the settings of the game, like the name of the default player, the name of the player currently using the game, the math level of the player, and the logic we want to use for the drawing functionality (i.e. how to draw through the gaze, better explained later).
- Keyboard is a file used to communicate between the scene invoking the keyboard and the KeyboardScene, which is a scene allowing the user to write texts via a virtual keyboard (more details below).
- Players contains the list of the players added to the game. 

The **Enum** folder contains the enumerators adopted inside the project.

The **EyeTracking** folder contains the logic adopted to use the Tobii Eye Tracker 5.
- Inside the *2DIntegration* folder there are the main scripts allowing the user to interact with specific game objects. To highlight these objects, their names start with the word *Gazeable*. This folder contains the scripts for the components used in more than one module. Separatedly, each module has its gazeable objects.
- The *GazeDrawing* folder contains the logic to draw starting from the gaze input of the user. To change the appearance of the gaze stripe, it is sufficient to add the image inside this folder, load it as a sprite, and assign it to the GazePlotter (Script) component attached to the *GazePlot* GameObject. To be more flexible, we already added circles of different colors.

The **UI** folder contains the graphic components used in the project.
- The Lean folder was taken from the  free *Lean GUI* asset available on the asset store ([link](https://assetstore.unity.com/packages/tools/gui/lean-gui-72138)). Note that the version online is slightly different from that in the project, as we modified it.
- The *Speechbubble* folder contains the sprites representing the comic speech. They are used throughout the project to represent the instructions given by the teacher.
- *TextMesh Pro* is the library needed to use the homonym category of text components. 
- The **Environment** prefab is inserted in each scene. It is a fundamental part of the entire project. The items it contains are listed below, together with a brief description of their purpose. 
   - *Canvas* contains the eye tracking toggle connected to the eye tracking functionality.
  - *EventSystem*: object needed by Unity; it is placed here to help not to forget it.
  - GameManager: object instantiated when the game starts and never deleted thanks to the *DontDestroyOnLoad* property. In addition to the API to switch between scenes, it also contains the methods to manage the eye-tracking functionality. 
  - GazePlot: if the eye tracker is connected and activated, it draws a stripe indicating where the user is looking to allow a better user experience.
  - MainCamera: default Unity camera object.
  - Skybox: it defines the property of the background used in the entire project.

- **GameConfig** is used to store some paths to the data files.
- **GameData** is the class mapping the content of the data stored in a file with a C# object.
- **GameManager** is the script attached to the GameManager object of the Environment that manages the switching between the different scenes.
- The **PlayerList** is the class allowing the reading and writing of the players' list from and to file as a JSON serializable object. The idea is to develop it better in the future in case it will be required.

## Coloring module
This module contains a mini-game where the player can load and color a black-and-white image.
- The image is loaded using the *SelectImageButton* on the first slide. Both .png and .jpg formats work well; we never tried others.
- The *ColoringScene_ImageSelection* class contains the method *LoadFileFromFileSystem* called when the button to load the image is clicked. This method passes the image to another class in charge of segmenting it into its white pieces (*ColoringScene_OpenCVAPI*). The logic to segment the image is described separately below.
- Each **segment** (i.e. white part of the image with black contour) is treated as a single GameObject. For each segment, the method instantiates a clone of the *segmentPrefab* and sets its position to the middle of the *ColoringCanvas* contained in the scene. In addition to the components already present in the prefab, the method adds, for each segment, the following components:
  - *SpriteRenderer*, which shows the texture;
  - *PolygonCollider2D*, which allows to define the clickable/gazeable contour of the segment;
  - *ColoringScene_GazeableSegment*, which contains the logic to follow when the user looks at the segment;
  - *GazeableObject*, which is the script mentioned above providing the connection between the Tobii Eye Tracker library and our implementation of the behavior to follow when an object is gazed.

### Image segmentation
The **segmentation** of the image is performed with the **OpenCvSharp** library. It is possible to integrate this library as an asset directly from the unity assets store ([link](https://assetstore.unity.com/packages/tools/integration/opencv-plus-unity-85928)). In our project, what we need from the asset is stored inside the *OpenV+Unity* folder. The *ColoringScene_OpenCVAPI* class is responsible of interacting with the library. This class only has one method, *SegmentTexture*. This method takes a Texture2D object as input and returns the list of Texture2D objects corresponding to the segments of the original image. Together with these, it also returns a texture of the edges for better visualization. The steps followed by the method to segment the image are described in the code and here briefly reported.
1.  The texture is converted into an **openCv matrix**, as all the other functionalities of this library work with this type of object.
      - Create a new square matrix whose edges are of the same size as the biggest edge of the original one.
      - Copy the original matrix in the middle of the one just created.
      - Convert the color of the matrix to grayscale.
2. Apply the OpenCV threshold function ([link](https://docs.opencv.org/4.x/d7/d4d/tutorial_py_thresholding.htm)) to get a matrix in which the pixels are only black and white.
3. Again with OpenCv, find the contours of the image in the matrix ([link](https://docs.opencv.org/3.4/d4/d73/tutorial_py_contours_begin.html)).
4. For each contour, extract the content of the matrix thanks to the function *DrawContours* and assign the result to a new matrix.
5. Resize the segment matrix to be as large as the reshaped one we are processing.
6. Color the background of the matrix to transparent.
7. After processing all the segments, create a matrix containing the contours of the original image. This is used as a top layer to hide the residual dirt between the segments.

### Segments interaction
The interaction with the segments can happen both with the mouse and with the eye tracker.
The interaction with the mouse is managed inside the *ColoringScene_PlayScreen* class.
The interaction with the gaze inside the *ColoringScene_GazeableSegement* class.

![representative gif](/Media/coloring_game.gif "Representative gif")

## Drawing module
This module contains a game where the player can draw on white paper with different colors.
- The general idea behind the logic adopted in this game came from some free assets found on the assets store. However, this module is independent and does not need the installation of any library to work.
- The *DrawingSprites* folder contains the image set containing the parts of the visual environment. 
- We have two prefabs in this module: one is responsible for the different colors (*DrawingScene_ColorPrefab*), and the other is for the contour size of the “brush” (*DrawingScene_WidthPrefab*). In addition to the components already present in the prefabs mentioned, the method adds, to each segment, the components listed below.
  - *SpriteRenderer*, which shows the texture;
  - *PolygonCollider2D*, which allows to define the clickable/gazeable contour of the segment;
  - *DrawingScene_GazeableColor*, which contains the logic to follow when the user looks at the color palette;
  - *DrawingScene_GazeableWidth*, which contains the logic allowing the player to change the curvature size;
  - *GazeableObject*, which is the script providing the connection between the Tobii Eye Tracker library and our implementation of the behavior to follow when an object is gazed at.
- One crucial step to perform to draw on a sprite is to change its **Advanced Setting** by checking the *Read/Write Enabled* option. The sprite has to be placed in the scene and the script managing the drawing logic has to be attached to it as a component.

### Drawing mechanism
The *DrawingScene_Drawing* script contains the logic of the mechanisms developed to draw. The user can draw both with the mouse and through the gaze.
Drawing with the gaze is more complex, as logic to translate gazes into stripes is required. We developed three approaches to solve this issue and made them available by changing the game settings. However, the default approach is the one we prefer, as it considers the velocity of the gaze to draw.

![representative image](/Media/drawing.png "Representative video")

## Introduction module (only on GitHub)
This module was initially developed when the project's scope was not yet well defined. In the *IntroductionScene*, the player can define his or her name through the virtual keyboard. This feature was designed to allow different players to interact with the game, saving each player's information locally and using it next time.
As the project went on, its scope changed. We left the  module for future developments.
![representative image](/Media/introduction.png "Representative image")

## Keyboard module (only on Github)
As mentioned, this module provides a scene with a virtual keyboard to compose text input. It is used with the *KeyboardScene_KeyboardAPI*. The API provides two methods to write and read messages to and from the keyboard file. The typical utilization of the keyboard scene consists of the following steps:
- the scene invoking the keyboard writes its name in the keyboard file and calls the GameManager to open the keyboard scene;
- the keyboard scene allows the user to insert text;
- when the user terminates, the keyboard reads the name of the scene previously activated and opens it through the GameManager;
- if the user clicks the “OK” button, the input field's content is written into the file and can be accessed from another scene.

![representative image](/Media/keyboard.png "Representative image")

## Main module
It contains a scene showing the set of buttons to launch the minigames. Every time the user stops using one minigame, the control is returned to this scene to allow the launching of a new one.

![representative image](/Media/main.png "Representative image")

## Math module
This module contains a game in which the player has to do a certain number of math exercises targeted to her/his level. 
- There are three difficulty levels:
  - **easy**: the user has only to map the figure with the text representing the same number,
  - **medium**: the user has to perform additions,
  - **difficult**: the user has to perform subtractions.
- The difficulty can be set when the scene is loaded or directly from the *GameConfig* file.
- All the exercises are structured in the same way. The request and the expression are written in white on the blackboard, while the list of possible answers is at the bottom.  
- All the exercises are generated dynamically by the class *MathScene_ExerciseGenerator*. 
- It is possible to change how to generate the exercises by using the commented *GetExercise* function inside the exercise generator.

### Future developments
It would be great to improve this game by playing different audio tracks depending on what the user does (e.g. right or wrong choice, math expression, etc.).

![representative image](/Media/math.png "Representative image")

## Memory module
Initially developed by Benjamin Südi and fixed by me.

This module contains a game where the player has to search for identical image pairs among ten faced-down cards.

## Number module
This module contains a game with the aim of recognizing the figure written by the user. 
- The logic to draw on the canvas is the same as applied in the DrawingModule. The only difference is that, in this case, only the black color is available. This limitation is to limit the objects shown by the interface. We decided to leave the possibility of changing the width of the brush.
- The recognition happens thanks to the integration of a **Machine Learning algorithm**. 
The solution implemented is the result of different trials. We tried libraries such as *Accord* and *ML.net*, but at the end we found it was easier to make the prediction through an already developed neural network imported as an **onnx model** ([https://onnx.ai/](https://onnx.ai/)).
We took the onnx model called **mnist-8** from Git Hub ([link](https://github.com/onnx/models)) and integrated it in Unity. For the integration, it has been necessary to install **Barracuda**. 
- The *NumbersScene_Predictor* is the class performing the prediction. The *Predict* method is called every time the user stops writing. The mnist-8 model returns an array containing a value associated with each figure. The **SoftMax** function allows to convert this value into probability. The predicted number is that with the higher probability and is written in the square on the right.

![representative image](/Media/number.png "Representative image")
