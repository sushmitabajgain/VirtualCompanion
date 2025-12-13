using UnityEngine;
using UnityEngine.SceneManagement;

public class HubUIController : MonoBehaviour
{
    public GameObject modePanel;

    private string pendingScene;

    void Start()
    {
        pendingScene = null;
        if (modePanel != null)
            modePanel.SetActive(false);
    }

    // Called by Camp / Building buttons
    public void SelectScene(string sceneName)
    {
        pendingScene = sceneName;

        if (modePanel != null)
            modePanel.SetActive(true);
    }

    // Mono button
    public void LoadMono()
    {
        if (string.IsNullOrEmpty(pendingScene))
            return;

        PlayerModeSwitcher.Instance.EnableMono();
        SceneManager.LoadScene(pendingScene);
    }

    // VR button
    public void LoadVR()
    {
        if (string.IsNullOrEmpty(pendingScene))
            return;

        PlayerModeSwitcher.Instance.EnableVR();
        SceneManager.LoadScene(pendingScene);
    }
}
