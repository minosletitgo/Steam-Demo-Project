using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

static public class UnityMenuBar_Scene
{
    [MenuItem("Scene/XA/BehaviorDesign_HelloWorld", false, 99)]
    static public void OpenScene_BehaviorDesign_HelloWorld()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/BehaviorDesign_HelloWorld.unity");
    }

    [MenuItem("Scene/XA/XA_StartScreen", false, 100)]
    static public void OpenScene_XA_StartScreen()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/XA_StartScreen.unity");
    }

    [MenuItem("Scene/XA/XA_Dungeon", false, 101)]
    static public void OpenScene_XA_Dungeon()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/XA_Dungeon.unity");
    }








    [MenuItem("Scene/TopDownEngineDemos/StartScreen", false, 1000)]
    static public void OpenScene_TD_Demo_StartScreen()
    {
        EditorSceneManager.OpenScene("Assets/TopDownEngine/Common/Scenes/StartScreen.unity");
        Debug.Log("-> " + EditorSceneManager.GetActiveScene().name);
    }

    [MenuItem("Scene/TopDownEngineDemos/Explodudes", false, 1002)]
    static public void OpenScene_TD_Demo_Explodudes()
    {
        EditorSceneManager.OpenScene("Assets/TopDownEngine/Demos/Explodudes/Explodudes.unity");
        Debug.Log("-> " + EditorSceneManager.GetActiveScene().name);
    }

    [MenuItem("Scene/TopDownEngineDemos/Grasslands", false, 1002)]
    static public void OpenScene_TD_Demo_Grasslands()
    {
        EditorSceneManager.OpenScene("Assets/TopDownEngine/Demos/Grasslands/Grasslands.unity");
        Debug.Log("-> " + EditorSceneManager.GetActiveScene().name);
    }

    [MenuItem("Scene/TopDownEngineDemos/KoalaDungeon", false, 1003)]
    static public void OpenScene_TD_Demo_KoalaDungeon()
    {
        EditorSceneManager.OpenScene("Assets/TopDownEngine/Demos/Koala2D/KoalaDungeon.unity");
        Debug.Log("-> " + EditorSceneManager.GetActiveScene().name);
    }

    [MenuItem("Scene/TopDownEngineDemos/Loft3D", false, 1004)]
    static public void OpenScene_TD_Demo_Loft3D()
    {
        EditorSceneManager.OpenScene("Assets/TopDownEngine/Demos/Loft3D/Loft3D.unity");
        Debug.Log("-> " + EditorSceneManager.GetActiveScene().name);
    }

    [MenuItem("Scene/TopDownEngineDemos/Minimal2D", false, 1005)]
    static public void OpenScene_TD_Demo_Minimal2D()
    {
        EditorSceneManager.OpenScene("Assets/TopDownEngine/Demos/Minimal2D/Minimal2D.unity");
        Debug.Log("-> " + EditorSceneManager.GetActiveScene().name);
    }

    [MenuItem("Scene/TopDownEngineDemos/Minimal2DRooms1", false, 1006)]
    static public void OpenScene_TD_Demo_Minimal2DRooms1()
    {
        EditorSceneManager.OpenScene("Assets/TopDownEngine/Demos/Minimal2D/Minimal2DRooms1.unity");
        Debug.Log("-> " + EditorSceneManager.GetActiveScene().name);
    }

    [MenuItem("Scene/TopDownEngineDemos/Minimal2DRooms2", false, 1007)]
    static public void OpenScene_TD_Demo_Minimal2DRooms2()
    {
        EditorSceneManager.OpenScene("Assets/TopDownEngine/Demos/Minimal2D/Minimal2DRooms2.unity");
        Debug.Log("-> " + EditorSceneManager.GetActiveScene().name);
    }

    [MenuItem("Scene/TopDownEngineDemos/MinimalSandbox2D", false, 1008)]
    static public void OpenScene_TD_Demo_MinimalSandbox2D()
    {
        EditorSceneManager.OpenScene("Assets/TopDownEngine/Demos/Minimal2D/MinimalSandbox2D.unity");
        Debug.Log("-> " + EditorSceneManager.GetActiveScene().name);
    }

    [MenuItem("Scene/TopDownEngineDemos/MinimalAI3D", false, 1009)]
    static public void OpenScene_TD_Demo_MinimalAI3D()
    {
        EditorSceneManager.OpenScene("Assets/TopDownEngine/Demos/Minimal3D/MinimalAI3D.unity");
        Debug.Log("-> " + EditorSceneManager.GetActiveScene().name);
    }

    [MenuItem("Scene/TopDownEngineDemos/MinimalPathfinding3D", false, 1010)]
    static public void OpenScene_TD_Demo_MinimalPathfinding3D()
    {
        EditorSceneManager.OpenScene("Assets/TopDownEngine/Demos/Minimal3D/MinimalPathfinding3D.unity");
        Debug.Log("-> " + EditorSceneManager.GetActiveScene().name);
    }

    [MenuItem("Scene/TopDownEngineDemos/MinimalSandbox3D", false, 1011)]
    static public void OpenScene_TD_Demo_MinimalSandbox3D()
    {
        EditorSceneManager.OpenScene("Assets/TopDownEngine/Demos/Minimal3D/MinimalSandbox3D.unity");
        Debug.Log("-> " + EditorSceneManager.GetActiveScene().name);
    }

    [MenuItem("Scene/TopDownEngineDemos/MinimalScene3D", false, 1012)]
    static public void OpenScene_TD_Demo_MinimalScene3D()
    {
        EditorSceneManager.OpenScene("Assets/TopDownEngine/Demos/Minimal3D/MinimalScene3D.unity");
        Debug.Log("-> " + EditorSceneManager.GetActiveScene().name);
    }

    [MenuItem("Scene/TopDownEngineDemos/MinimalSword3D", false, 1013)]
    static public void OpenScene_TD_Demo_MinimalSword3D()
    {
        EditorSceneManager.OpenScene("Assets/TopDownEngine/Demos/Minimal3D/MinimalSword3D.unity");
        Debug.Log("-> " + EditorSceneManager.GetActiveScene().name);
    }
}
