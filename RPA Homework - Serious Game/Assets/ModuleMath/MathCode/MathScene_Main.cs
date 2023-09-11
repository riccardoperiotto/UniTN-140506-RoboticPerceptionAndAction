using Lean.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MathScene_Main : MonoBehaviour
{
    private GameManager gameManager;
    private string toggleNameCommonPart = "LevelSelectionToggle";
    private Level selectedLevel = Level.Easy;
    private List<Exercise> exercises;

    private int indexCurrentExercise = 0;
    private float deletionDelay = 1.2f;

    private GameObject description;
    private GameObject elements;
    private GameObject proposedSolutions;
    private GameObject exerciseCounter;

    public GameObject exerciseElementButtonPrefab;
    public GameObject rightSolutionButtonPrefab;
    public GameObject wrongSolutionButtonPrefab;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;

        description = GameObject.Find("ExerciseDescription");
        elements = GameObject.Find("ExerciseElements");
        proposedSolutions = GameObject.Find("ProposedSolutions");
        exerciseCounter = GameObject.Find("ExerciseCounter");


        GameObject.Find("Screen 2").transform.Find("ButtonNext").GetComponent<LeanButton>().OnClick.AddListener(delegate
        {
            // set the level as the one selected
            foreach (Transform levelSelectionToggle in GameObject.Find("MathLevelOptions").transform)
            {
                if (levelSelectionToggle.GetComponent<LeanToggle>().On)
                {
                    selectedLevel = (Level)Enum.Parse(typeof(Level), levelSelectionToggle.name.Substring(toggleNameCommonPart.Length), true);
                    break;
                }
            }

            SetLevel();
            CreateExercises();
            DrawExercise();
        });

        // hide the ButtonNext of third screen
        GameObject.Find("Screen 3").transform.Find("ButtonNext").gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetLevel()
    {
        if (gameManager?.gameData?.MathLevel != null)
        {
            gameManager.gameData.MathLevel = selectedLevel;
            gameManager.SaveGameData();
        }
    }
    void CreateExercises()
    {
        exercises = MathScene_ExerciseGenerator.IntializeExercises(selectedLevel);
    }

    /// <summary>
    /// Recursive function that place the components of the exercise with the current indexCurrentExercise
    /// In case the exercises are finished it renders the last screen
    /// </summary>
    void DrawExercise()
    {
        // BASE CASE: SHIFT TO NEXT SCREEN
        if (indexCurrentExercise == exercises.Count)
        {
            // make the ButtonNext available
            GameObject.Find("Screen 3").transform.Find("ButtonNext").gameObject.SetActive(true);
            return;
        }

        // RECURSIVE STEP: FILL THE EXERCISEDETAILS
        Exercise exercise = exercises[indexCurrentExercise];

        // exercise.Description
        description.GetComponent<Text>().text = exercise.Description;

        // exercise.Elements
        foreach (Transform child in elements.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (string elementText in exercise.Elements)
        {
            GameObject expressionElement = (GameObject)Instantiate(exerciseElementButtonPrefab, elements.transform);
            expressionElement.transform.Find("Text").GetComponent<Text>().text = elementText;
        }

        // exercises counter update
        exerciseCounter.transform.Find("Counter").GetComponent<TMP_Text>().text = $"{indexCurrentExercise + 1}/{exercises.Count}";

        // exercise proposed solutions
        // delete all the children of the previous proposed solution
        foreach (Transform child in proposedSolutions.transform)
        {
            Destroy(child.gameObject);
        }

        // fill the proposed solution buttons
        // one of them have to be the right one, but randomly
        int rightPos = MathScene_ExerciseGenerator.rnd.Next(0, exercise.WrongSolutions.Count - 1);
        for (int i = 0; i < exercise.WrongSolutions.Count; i++)
        {
            if (i == rightPos)
            {
                GameObject solutionButton = (GameObject)Instantiate(rightSolutionButtonPrefab, proposedSolutions.transform);
                solutionButton.transform.Find("Panel").transform.Find("Text").GetComponent<Text>().text = exercise.RightSolution;

                // REDUCTION STEP AND RECURSIVE CALL
                // only when the exercise is completed correctly
                solutionButton.GetComponent<LeanButton>().OnClick.AddListener(delegate
                {
                    indexCurrentExercise++;
                    Destroy(solutionButton, deletionDelay);
                    DrawExercise();
                });
            }

            {
                GameObject solutionButton = (GameObject)Instantiate(wrongSolutionButtonPrefab, proposedSolutions.transform);
                solutionButton.transform.Find("Panel").transform.Find("Text").GetComponent<Text>().text = exercise.WrongSolutions[i];

                solutionButton.GetComponent<LeanButton>().OnClick.AddListener(delegate
                {
                    Destroy(solutionButton, deletionDelay);
                });
            }

        }
    }
}
