using UnityEngine;
using UnityEngine.XR;

public class BackButtonVisibility : MonoBehaviour
{
    [SerializeField] private GameObject backButton;

    void Start()
    {
        UpdateBackButtonVisibility();
    }

    void UpdateBackButtonVisibility()
    {
        bool isVRActive = XRSettings.isDeviceActive;
        backButton.SetActive(!isVRActive);
    }
}
