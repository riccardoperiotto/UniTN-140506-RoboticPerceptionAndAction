using System;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;

public class MathScene_ExerciseGenerator : ScriptableObject
{
    static public System.Random rnd = new System.Random(Convert.ToInt32(DateTime.Now.Millisecond));

    static private Dictionary<int, string> NumberMapping = new Dictionary<int, string>()
    {
        {0, "zero" },
        {1, "uno" },
        {2, "due" },
        {3, "tre" },
        {4, "quattro" },
        {5, "cinque" },
        {6, "sei" },
        {7, "sette" },
        {8, "otto" },
        {9, "nove" },
        {10, "dieci" },
    };
    static private List<int> GetTerms()
    {
        var res = new List<int>
        {
            rnd.Next(0, 10),
            rnd.Next(0, 10)
        };
        return res;
    }

    // a simple exercise is the recognition
    static public string GetSimpleExericise(ref string description, ref List<string> elements, ref List<string> wrongSolutions)
    {
        description = "Che numero è?";

        // elements creation
        int rightSolution = rnd.Next(0, 10);
        elements.Add(rightSolution.ToString());

        // wrong solution creation
        int counter = 0;
        do
        {
            int wrongSolution = rnd.Next(0, 10);
            if (wrongSolution != rightSolution && !wrongSolutions.Contains(NumberMapping[wrongSolution]))
            {
                wrongSolutions.Add(NumberMapping[wrongSolution]);
            }
            counter++;
        } while (counter < 4);

        return NumberMapping[rightSolution];
    }

    // a medium exercise is the addition
    static public string GetMediumExericise(ref string description, ref List<string> elements, ref List<string> wrongSolutions)
    {
        description = "Addizioni! Quanto fa?";

        // elements creation
        List<int> terms = GetTerms();
        elements.Add(terms[0].ToString());
        elements.Add("+");
        elements.Add(terms[1].ToString());
        elements.Add("=");
        elements.Add("?");

        int rightSolution = terms[0] + terms[1];

        // wrong solution creation
        int counter = 0;
        do
        {
            int wrongSolution = rnd.Next(-20, 100);
            if (wrongSolution != rightSolution && !wrongSolutions.Contains(wrongSolution.ToString()))
            {
                wrongSolutions.Add(wrongSolution.ToString());
                counter++;
            }
        } while (counter < 3);

        return rightSolution.ToString();
    }

    // an hard exercise is a subtraction
    static public string GethHardExericise(ref string description, ref List<string> elements, ref List<string> wrongSolutions)
    {
        description = "Sottrazioni! Quanto fa?";

        // elements creation
        List<int> terms = GetTerms();
        elements.Add(terms[0].ToString());
        elements.Add("-");
        elements.Add(terms[1].ToString());
        elements.Add("=");
        elements.Add("?");

        int rightSolution = terms[0] - terms[1];

        // wrong solution creation
        int counter = 0;
        do
        {
            int wrongSolution = rnd.Next(-20, 100);
            if (wrongSolution != rightSolution && !wrongSolutions.Contains(wrongSolution.ToString()))
            {
                wrongSolutions.Add(wrongSolution.ToString());
            }

        } while (++counter < 3);

        return rightSolution.ToString();
    }
    /// <summary>
    /// Get an exercise with difficulty up to maximumLevel
    /// </summary>
    /// <param name="maximumLevel"></param>
    /// <returns></returns>
    //static public Exercise GetExercise(Level maximumLevel)
    //{
    //    string description = "";
    //    List<string> elements = new List<string>();
    //    List<string> wrongSolutions = new List<string>();
    //    string rightSolution = "";
    //    Level obtainedLevel = Level.Easy;
    //    switch (rnd.Next(0, (int)maximumLevel))
    //    {
    //        case (int)Level.Easy:
    //            obtainedLevel = Level.Easy;
    //            rightSolution = GetSimpleExericise(ref description, ref elements, ref wrongSolutions);
    //            break;
    //        case (int)Level.Medium:
    //            obtainedLevel = Level.Medium;
    //            rightSolution = GetMediumExericise(ref description, ref elements, ref wrongSolutions);
    //            break;
    //        case (int)Level.Hard:
    //            obtainedLevel = Level.Hard;
    //            rightSolution = GethHardExericise(ref description, ref elements, ref wrongSolutions);
    //            break;
    //        default:
    //            Debug.Log("Emmm...");
    //            break;
    //    }

    //    return new Exercise()
    //    {
    //        Level = obtainedLevel,
    //        Description = description,
    //        Elements = elements,
    //        WrongSolutions = wrongSolutions,
    //        RightSolution = rightSolution
    //    };
    //}

    /// <summary>
    /// Get an exercise of the specified level
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    static public Exercise GetExercise(Level level)
    {
        string description = "";
        List<string> elements = new List<string>();
        List<string> wrongSolutions = new List<string>();
        string rightSolution = "";

        switch ((int)level)
        {
            case (int)Level.Easy:
                rightSolution = GetSimpleExericise(ref description, ref elements, ref wrongSolutions);
                break;
            case (int)Level.Medium:
                rightSolution = GetMediumExericise(ref description, ref elements, ref wrongSolutions);
                break;
            case (int)Level.Hard:
                rightSolution = GethHardExericise(ref description, ref elements, ref wrongSolutions);
                break;
            default:
                Debug.Log("Emmm...");
                break;
        }

        return new Exercise()
        {
            Level = level,
            Description = description,
            Elements = elements,
            WrongSolutions = wrongSolutions,
            RightSolution = rightSolution
        };
    }

    static public List<Exercise> IntializeExercises(Level level)
    {
        List<Exercise> exercises = new List<Exercise>();
        for (int i = 0; i < GameConfig.NumberOfMathExercises; i++)
        {
            Exercise exercise;
            bool goOn = false;
            do
            {
                exercise = GetExercise(level);
                goOn = exercises.FindIndex(i => i.Equals(exercise)) < 0;
            } while (!goOn);
            exercises.Add(exercise);
        }
        return exercises;
    }
}

public class Exercise
{
    public Level Level;
    public string Description;
    public List<string> Elements;
    public List<string> WrongSolutions;
    public string RightSolution;


    public override bool Equals(object obj)
    {
        return Equals(obj as Exercise);
    }

    public bool Equals(Exercise other)
    {
        if (other == null) return false;

        if (Level != other.Level) return false;
       
        for (int i = 0; i < Elements.Count; i++)
        {
            if (Elements[i] != other.Elements[i]) return false;
        }

        return true;
    }

    // not implemented
    public override int GetHashCode()
    {
        return 21870;
    }
}
