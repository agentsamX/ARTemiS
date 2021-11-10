using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using PathOS;
using Malee.Editor;

/*
PathOSEvaluationWindow.cs 
(Atiya Nova) 2021
 */

public enum HeuristicPriority
{
    NONE = 0,
    LOW = 1,
    MED = 2,
    HIGH = 3,
}

public enum HeuristicCategory
{
    NONE = 0,
    POS = 1,
    NEG = 2,
}

[Serializable]
class UserComment
{
    public string description = "";
    public bool categoryFoldout = false;
    public HeuristicPriority priority = HeuristicPriority.NONE;
    public HeuristicCategory category = HeuristicCategory.NONE;
}

[Serializable]
class ExpertEvaluation
{ 
    //TODO: Spread things out in here to clean it up
    public List<UserComment> userComments = new List<UserComment>();
    private GUIStyle foldoutStyle = GUIStyle.none, buttonStyle = GUIStyle.none, labelStyle = GUIStyle.none;

    private readonly string[] priorityNames = new string[] { "NA", "LOW", "MED", "HIGH" };
    private readonly string[] categoryNames = new string[] { "NA", "POS", "NEG" };
    private readonly string headerRow = "#";
    private Color[] priorityColors = new Color[] { Color.white, Color.green, Color.yellow, new Color32(248, 114, 126, 255) };
    private Color[] categoryColors = new Color[] { Color.white, Color.green, new Color32(248, 114, 126, 255) };

    public void SaveData()
    {
        string saveName;
        Scene scene = SceneManager.GetActiveScene();

        saveName = scene.name + " heuristicAmount";

        int counter = userComments.Count;
        PlayerPrefs.SetInt(saveName, counter);

        for (int i = 0; i < userComments.Count; i++)
        {
            //saveName = scene.name + " heuristicsInputs " + i;
            saveName = scene.name + " heuristicsInputs " + i;

            PlayerPrefs.SetString(saveName, userComments[i].description);

            //saveName = "heuristicsPriorities " + i;
            saveName = scene.name + " heuristicsPriorities " + i;

            PlayerPrefs.SetInt(saveName, (int)userComments[i].priority);

            //saveName = "heuristicsCategories " + i;
            saveName = scene.name + " heuristicsCategories " + i;

            PlayerPrefs.SetInt(saveName, (int)userComments[i].category);
        }
    }

    public void LoadData()
    {
        string saveName;
        Scene scene = SceneManager.GetActiveScene();
        int counter = 0;

        userComments.Clear();

        saveName = scene.name + " heuristicAmount";

        if (PlayerPrefs.HasKey(saveName))
            counter = PlayerPrefs.GetInt(saveName);

        for (int i = 0; i < counter; i++)
        {
            userComments.Add(new UserComment());

            saveName = scene.name + " heuristicsInputs " + i;
            if (PlayerPrefs.HasKey(saveName))
                userComments[i].description = PlayerPrefs.GetString(saveName);

            //saveName = "heuristicsPriorities " + i;
            saveName = scene.name + " heuristicsPriorities " + i;
            if (PlayerPrefs.HasKey(saveName))
                userComments[i].priority = (HeuristicPriority)PlayerPrefs.GetInt(saveName);


           // saveName = "heuristicsCategories " + i;
            saveName = scene.name + " heuristicsCategories " + i;

            if (PlayerPrefs.HasKey(saveName))
                userComments[i].category = (HeuristicCategory)PlayerPrefs.GetInt(saveName);
        }
    }

    public void DrawComments()
    {
        EditorGUILayout.Space();

        foldoutStyle = EditorStyles.foldout;
        foldoutStyle.fontSize = 14;

        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 15;

        labelStyle.fontSize = 15;
        labelStyle.fontStyle = FontStyle.Italic;

        EditorGUILayout.BeginVertical("Box");

        if (userComments.Count <= 0)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("   There are currently no comments.", labelStyle);
            EditorGUILayout.EndHorizontal();
        }

        //girl what is this
        for (int i = 0; i < userComments.Count; i++)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("Button");
            foldoutStyle.fontStyle = FontStyle.Italic;

            EditorGUILayout.BeginHorizontal();
            userComments[i].categoryFoldout = EditorGUILayout.Foldout(userComments[i].categoryFoldout, "Comment #" + (i+1), foldoutStyle);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("X", GUILayout.Width(17), GUILayout.Height(15)))
            {
                userComments.RemoveAt(i);
                i--;
                continue;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            if (!userComments[i].categoryFoldout)
            {
                EditorGUILayout.EndVertical();
                continue;
            }

            EditorGUI.indentLevel++;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            EditorStyles.label.wordWrap = true;
            userComments[i].description = EditorGUILayout.TextArea(userComments[i].description, GUILayout.Width(Screen.width * 0.6f));

            GUI.backgroundColor = categoryColors[((int)userComments[i].category)];
            userComments[i].category = (HeuristicCategory)EditorGUILayout.Popup((int)userComments[i].category, categoryNames);
            GUI.backgroundColor = priorityColors[((int)userComments[i].priority)];
            userComments[i].priority = (HeuristicPriority)EditorGUILayout.Popup((int)userComments[i].priority, priorityNames);
            GUI.backgroundColor = priorityColors[0];
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                SaveData();
            }
                
            
            EditorGUILayout.Space(5);

            EditorGUILayout.EndVertical();

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("+", buttonStyle, GUILayout.Width(100)))
        {
            userComments.Add(new UserComment());
            SaveData();
        }
        if (GUILayout.Button("-", buttonStyle, GUILayout.Width(100)))
        {
            if (userComments.Count > 0) 
            {
                userComments.RemoveAt(userComments.Count - 1);
                SaveData();
            }
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        EditorGUILayout.EndVertical();

        foldoutStyle.fontSize = 12;

    }

    public void DeleteAll()
    {
        userComments.Clear();

        SaveData();
    }

    public void ImportInputs(string filename)
    {
        StreamReader reader = new StreamReader(filename);

        string line = "";
        string[] lineContents;

        int inputCounter = 0;

        userComments.Clear();

        while ((line = reader.ReadLine()) != null)
        {
            lineContents = line.Split(',');

            if (lineContents.Length < 1)
            {
                Debug.Log("Error! Unable to read line");
                continue;
            }

            if (lineContents[0] == headerRow)
            {
                continue;
            }

            userComments.Add(new UserComment());

            string newDescription = lineContents[1].Replace("  ", "\n").Replace("/", ",");
            userComments[inputCounter].description = newDescription;

            userComments[inputCounter].priority = StringToHeuristicPriority(lineContents[2]);

            userComments[inputCounter].category = StringToHeuristicCategory(lineContents[3]);

            inputCounter++;
        }

        reader.Close();

        SaveData();
    }

    public void ExportHeuristics(string filename)
    {
        StreamWriter writer = new StreamWriter(filename);

        writer.WriteLine("#, Description, Input, Priority, Category");
        string description, priority, category, number;

        for (int i = 0; i < userComments.Count; i++)
        {
            number = (i + 1).ToString();
            description = userComments[i].description.Replace("\r", "").Replace("\n", "  ").Replace(",", "/");

            priority = HeuristicPriorityToString(userComments[i].priority);

            category = HeuristicCategoryToString(userComments[i].category);

            writer.WriteLine(number + ',' + description + ',' + priority + ',' + category);
        }

        writer.Close();
        
        SaveData();
    }


    private string HeuristicPriorityToString(HeuristicPriority name)
    {
        switch (name)
        {
            case HeuristicPriority.NONE:
                return "NA";
            case HeuristicPriority.LOW:
                return "LOW";
            case HeuristicPriority.MED:
                return "MED";
            case HeuristicPriority.HIGH:
                return "HIGH";
            default:
                return "NA";
        }
    }

    private HeuristicPriority StringToHeuristicPriority(string name)
    {
        switch (name)
        {
            case "NA":
                return HeuristicPriority.NONE;
            case "LOW":
                return HeuristicPriority.LOW;
            case "MED":
                return HeuristicPriority.MED;
            case "HIGH":
                return HeuristicPriority.HIGH;
            default:
                return HeuristicPriority.NONE;
        }
    }
    private string HeuristicCategoryToString(HeuristicCategory name)
    {
        switch (name)
        {
            case HeuristicCategory.NONE:
                return "NA";
            case HeuristicCategory.POS:
                return "POS";
            case HeuristicCategory.NEG:
                return "NEG";
            default:
                return "NA";
        }
    }
    private HeuristicCategory StringToHeuristicCategory(string name)
    {
        switch (name)
        {
            case "NA":
                return HeuristicCategory.NONE;
            case "POS":
                return HeuristicCategory.POS;
            case "NEG":
                return HeuristicCategory.POS;
            default:
                return HeuristicCategory.NONE;
        }
    }

    public void AddNewComment()
    {
        userComments.Add(new UserComment());
        SaveData();
    }
}

public class PathOSEvaluationWindow : EditorWindow
{
    private Color bgColor, btnColor;
    ExpertEvaluation comments = new ExpertEvaluation();
    private GUIStyle headerStyle = new GUIStyle();
    private GameObject selection = null;
    public bool popupAlreadyOpen = false;
    private string expertEvaluation = "Expert Evaluation", deleteAll = "DELETE ALL", import = "IMPORT", export = "EXPORT";
    private bool initialized = false; 
   
    private void OnEnable()
    {
        //Background color
        comments.LoadData();
        bgColor = GUI.backgroundColor;
        btnColor = new Color32(200, 203, 224, 255);

      //  SceneView.onSceneGUIDelegate += this.OnSceneGUI;
    }

    private void OnDestroy()
    {
      //  SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
       // comments.SaveData();
    }

    private void OnDisable()
    {
     //   SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
     //   comments.SaveData();
    }

    public void OnWindowOpen()
    {
        GUILayout.BeginHorizontal();

        GUI.backgroundColor = btnColor;
        headerStyle.fontSize = 20;

        EditorGUILayout.LabelField(expertEvaluation, headerStyle);

        if (GUILayout.Button(deleteAll))
        {
            comments.DeleteAll();
        }

        if (GUILayout.Button(import))
        {
            string importPath = EditorUtility.OpenFilePanel("Import Evaluation", "ASSETS\\EvaluationFiles", "csv");

            if (importPath.Length != 0)
            {
                comments.ImportInputs(importPath);
            }
        }

        if (GUILayout.Button(export))
        {
            string exportPath = EditorUtility.OpenFilePanel("Export Evaluation", "ASSETS\\EvaluationFiles", "csv");

            if (exportPath.Length != 0)
            {
                comments.ExportHeuristics(exportPath);
            }
        }

        GUI.backgroundColor = bgColor;
        GUILayout.EndHorizontal();
        comments.DrawComments();
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (popupAlreadyOpen) return;

       // Event e = Event.current;
       //
       // //Selection update.
       // if (EditorWindow.mouseOverWindow != null &&
       //     EditorWindow.mouseOverWindow.ToString() == " (UnityEditor.SceneView)")
       // {
       //     if (e.type == EventType.MouseUp && e.button == 1)
       //     {
       //         selection = HandleUtility.PickGameObject(Event.current.mousePosition, true);
       //         //popupAlreadyOpen = true;
       //         // OpenPopup();
       //     }
       // }
       // else
       // {
       //     selection = null;
       // }
    }

    public void AddComment()
    {
        popupAlreadyOpen = false;
        comments.AddNewComment();
    }

   // private void OpenPopup()
   // {
   //     CommentPopup window = ScriptableObject.CreateInstance<CommentPopup>();
   //     window.evaluationWindow = this;
   //     window.position = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 250, 150);
   //     window.ShowUtility();
   //
   // }

}

//public class CommentPopup : EditorWindow
//{
//    public PathOSEvaluationWindow evaluationWindow;
//
//    void OnGUI()
//    {
//        EditorGUILayout.LabelField("This is an example of EditorWindow.ShowPopup", EditorStyles.wordWrappedLabel);
//        GUILayout.Space(70);
//
//        if (GUILayout.Button("Add Comment"))
//        {
//            evaluationWindow.AddComment();
//            this.Close();
//        }
//    }
//}