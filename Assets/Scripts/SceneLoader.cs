using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadCamp()
    {
        LoadScene("CampScene");
    }

    public void LoadBuilding()
    {
        LoadScene("BuildingScene");
    }

    public void LoadHub()
    {
        LoadScene("HubScene");
    }

    private void LoadScene(string sceneName)
    {
        Debug.Log("LOADING SCENE: " + sceneName);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
