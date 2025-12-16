using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ElevenLabsTTS : MonoBehaviour
{
    [Header("ElevenLabs Settings")]
    [Tooltip("Paste your ElevenLabs API key here (no quotes, no spaces)")]
    public string apiKey;

    [Tooltip("Voice ID from your ElevenLabs account")]
    public string voiceId;

    [Range(0.1f, 1f)] public float stability = 0.8f;
    [Range(0.1f, 1f)] public float similarityBoost = 0.5f;

    public IEnumerator Speak(string text, AudioSource audioSource)
    {
        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(voiceId))
        {
            Debug.LogError("[ElevenLabs] API Key or Voice ID missing");
            yield break;
        }

        string url = $"https://api.elevenlabs.io/v1/text-to-speech/{voiceId}";

        string jsonBody = JsonUtility.ToJson(new ElevenLabsRequest
        {
            text = text,
            voice_settings = new VoiceSettings
            {
                stability = stability,
                similarity_boost = similarityBoost
            }
        });

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerAudioClip(url, AudioType.MPEG);

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "audio/mpeg");
            request.SetRequestHeader("xi-api-key", apiKey);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(
                    $"[ElevenLabs] TTS failed | HTTP {request.responseCode} | {request.error}"
                );
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(request);

            if (clip == null)
            {
                Debug.LogError("[ElevenLabs] Received null AudioClip");
                yield break;
            }

            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    // ---------- REQUEST DATA ----------
    [System.Serializable]
    class ElevenLabsRequest
    {
        public string text;
        public VoiceSettings voice_settings;
    }

    [System.Serializable]
    class VoiceSettings
    {
        public float stability;
        public float similarity_boost;
    }
}
