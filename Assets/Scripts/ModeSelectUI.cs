using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using System.Collections;
using System.Collections.Generic;

public class ModeSelectUI : MonoBehaviour
{
    // Scene names for mono (non-VR) mode and VR mode
    public string monoSceneName = "Player_Scene_Mono";
    public string vrSceneName   = "Player_Scene_VR";

    // Internal flag to prevent multiple simultaneous scene loads
    bool loading;

    // Method to start the Mono scene (non-VR gameplay)
    public void StartMono()
    {
        if (!loading)
            SceneManager.LoadScene(monoSceneName, LoadSceneMode.Single);
    }

    // Method to start the VR scene (VR gameplay)
    public void StartVR()
    {
        if (!loading)
            SceneManager.LoadScene(vrSceneName, LoadSceneMode.Single);
    }

    // Method to quit the application
    public void QuitGame()
    {
        Application.Quit();
    }
}
