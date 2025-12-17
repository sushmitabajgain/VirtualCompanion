using UnityEngine;
using UnityEngine.SceneManagement;

public class HubUIController : MonoBehaviour
{
    public GameObject modePanel;

    private string pendingScene;
    private bool loading;

    void Start()
    {
        pendingScene = null;
        loading = false;

        if (modePanel != null)
            modePanel.SetActive(false);
    }

    // Called by Camp / Building buttons
    public void SelectScene(string sceneName)
    {
        if (loading)
            return;

        pendingScene = sceneName;

        if (modePanel != null)
            modePanel.SetActive(true);
    }

    // Mono button
    public void LoadMono()
    {
        if (loading || string.IsNullOrEmpty(pendingScene))
            return;

        loading = true;

        PlayerModeSwitcher.Instance.EnableMono();
        SceneManager.LoadScene(pendingScene);
    }

    // VR button
    public void LoadVR()
    {
        if (loading || string.IsNullOrEmpty(pendingScene))
            return;

        loading = true;

        PlayerModeSwitcher.Instance.EnableVR();
        SceneManager.LoadScene(pendingScene);
    }
}
