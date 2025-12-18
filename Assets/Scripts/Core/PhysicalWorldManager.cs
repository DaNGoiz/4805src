using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhysicalWorldManager : MonoBehaviour
{
    public static PhysicalWorldManager instance;
    public static int NPCCount = 19; // 预设


    public List<string> scenes = new List<string>
    {
        "Library",
        "The Palace",
        "Garden",
    };


    private void Awake()
    {
        instance = this;
    }

    public void LoadScene(string sceneName)
    {
        if (scenes.Contains(sceneName))
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }

    public void UnloadScene(string sceneName)
    {
        if (scenes.Contains(sceneName))
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
    }

    public void LoadAllScene()
    {
        foreach (string sceneName in scenes)
        {
            if (!SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
        }
    }
}
