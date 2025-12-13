using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSceneSpawner : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ignore Hub / menu scenes
        if (scene.name == "HubScene")
            return;

        // Look for SpawnPoint by NAME
        GameObject spawn = GameObject.Find("SpawnPoint");

        if (spawn == null)
        {
            Debug.LogWarning($"SpawnPoint not found in {scene.name}");
            return;
        }

        transform.position = spawn.transform.position;
        transform.rotation = spawn.transform.rotation;

        Debug.Log($"Player moved to SpawnPoint in {scene.name}");
    }
}
