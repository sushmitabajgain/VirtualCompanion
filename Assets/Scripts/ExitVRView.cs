using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_ANDROID || UNITY_IOS
using Google.XR.Cardboard;
#endif

public class ExitVRView : MonoBehaviour
{
    // Set this to your Hub SCENE name
    public string hubScene = "HubScene";

    private bool exiting;

    void Update()
    {
        if (exiting)
            return;

#if UNITY_ANDROID || UNITY_IOS
        // Cardboard system X button
        if (Api.IsCloseButtonPressed)
        {
            ExitToHub();
            return;
        }
#endif

        // Android back button
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitToHub();
        }
    }

    private void ExitToHub()
    {
        exiting = true;

        // Exit XR FIRST
        PlayerModeSwitcher.Instance.EnableMono();

        // Then load Hub scene
        SceneManager.LoadScene(hubScene);
    }
}
