using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneContextReporter : MonoBehaviour
{
    void Start()
    {
        CompanionManager.Instance.SetScene(SceneManager.GetActiveScene().name);
    }
}
