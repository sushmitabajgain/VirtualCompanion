using UnityEngine;

public class NarrationTrigger : MonoBehaviour
{
    [TextArea(2, 4)]
    public string narrationText;

    bool hasPlayed;

    void OnTriggerEnter(Collider other)
    {
        if (hasPlayed)
            return;

        if (!other.CompareTag("Player"))
            return;

        if (CompanionManager.Instance == null)
            return;

        CompanionManager.Instance.PlayImmediateNarration(narrationText);
        hasPlayed = true;
    }
}
