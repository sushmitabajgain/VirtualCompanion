using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnPlayerOnSceneLoad : MonoBehaviour
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
        // Only apply spawn in content scenes
        if (scene.name != "CampScene" && scene.name != "BuildingScene")
            return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject spawn = GameObject.Find("SpawnPoint");

        if (player && spawn)
        {
            player.transform.SetPositionAndRotation(
                spawn.transform.position,
                spawn.transform.rotation
            );

            Debug.Log($"Player spawned at SpawnPoint in {scene.name}");
        }
        else
        {
            Debug.LogError("SpawnPoint or Player not found");
        }
    }
}
