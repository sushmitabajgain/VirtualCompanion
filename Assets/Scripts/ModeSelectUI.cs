using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeSelectUI : MonoBehaviour
{
    public string monoSceneName = "Player_Scene_Mono";
    public string vrSceneName   = "Player_Scene_VR";

    private bool loading;

    public void StartMono()
    {
        if (loading) return;
        loading = true;

        PlayerModeSwitcher.Instance.EnableMono();
        SceneManager.LoadScene(monoSceneName, LoadSceneMode.Single);
    }

    public void StartVR()
    {
        if (loading) return;
        loading = true;

        PlayerModeSwitcher.Instance.EnableVR();
        SceneManager.LoadScene(vrSceneName, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
