using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    // Name of the menu scene to load
    public string menuSceneName = "ModeSelect";

    // Method to return to the menu scene
    public void GoMenu()
    {
        SceneManager.LoadScene(menuSceneName, LoadSceneMode.Single);
    }

    // Check each frame for Escape key press
    void Update()
    {
        // If Escape is pressed, go back to the menu
        if (Input.GetKeyDown(KeyCode.Escape))
            GoMenu();
    }
}
