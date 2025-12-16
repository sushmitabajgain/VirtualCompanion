using UnityEngine;
using System.Collections;

public enum CompanionMode
{
    Mono,
    VR
}

public class CompanionManager : MonoBehaviour
{
    public static CompanionManager Instance;

    [Header("Narration Timing")]
    [Tooltip("Pause between narration lines (seconds)")]
    public float pauseBetweenLines = 15f;

    [Header("Fallback Text")]
    [TextArea(2, 4)]
    public string defaultText = "You are in a calm and gentle space… take your time.";

    public string CurrentScene;
    public CompanionMode CurrentMode;

    AudioSource voiceSource;
    ElevenLabsTTS tts;

    float silenceUntilTime;
    float nextAllowedSpeechTime;

    int narrationIndex;
    int pendingNarrationIndex = -1;

    bool isSpeaking;
    Coroutine speakingCoroutine;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        voiceSource = gameObject.AddComponent<AudioSource>();
        voiceSource.playOnAwake = false;
        voiceSource.spatialBlend = 0f;
        voiceSource.volume = 0.6f;
        voiceSource.pitch = 0.95f;   // slightly slower, natural


        tts = GetComponent<ElevenLabsTTS>();
        if (tts == null)
            Debug.LogError("[Companion] ElevenLabsTTS missing!");
    }

    // ---------- CONTEXT ----------
    public void SetScene(string sceneName)
    {
        if (CurrentScene == sceneName)
            return;

        CurrentScene = sceneName;
        ForceStopSpeech();
        ResetState();
    }

    public void SetMode(CompanionMode mode)
    {
        if (CurrentMode == mode)
            return;

        ForceStopSpeech();
        CurrentMode = mode;

        // Exit VR → silence narration
        if (mode == CompanionMode.Mono)
        {
            silenceUntilTime = float.MaxValue;
            return;
        }

        ResetState();
    }

    void ResetState()
    {
        // ⏳ Wait 2 seconds before FIRST narration
        silenceUntilTime = Time.time + 2f;

        // ✅ Allow first narration immediately after silence
        nextAllowedSpeechTime = silenceUntilTime;

        narrationIndex = 0;
        pendingNarrationIndex = -1;
    }

    bool IsSilent() => Time.time < silenceUntilTime;

    // ---------- UPDATE ----------
    void Update()
    {
        if (CurrentMode == CompanionMode.Mono && CurrentScene != "HubScene")
            return;

        if (tts == null || isSpeaking)
            return;

        if (IsSilent())
            return;

        if (Time.time < nextAllowedSpeechTime)
            return;

        TrySpeakNarration();
    }

    // ---------- SPEAK ----------
    void TrySpeakNarration()
    {
        var segments = GetNarrationForScene();
        if (segments == null || narrationIndex >= segments.Length)
            return;

        pendingNarrationIndex = narrationIndex;
        StartSpeech(segments[narrationIndex]);
    }

    void StartSpeech(string text)
    {
        nextAllowedSpeechTime = Time.time + pauseBetweenLines;
        voiceSource.spatialBlend = (CurrentMode == CompanionMode.VR) ? 0.7f : 0f;
        speakingCoroutine = StartCoroutine(SpeakRoutine(text));
    }

    IEnumerator SpeakRoutine(string text)
    {
        isSpeaking = true;
        yield return StartCoroutine(tts.Speak(text, voiceSource));

        // Commit narration after successful speech
        if (pendingNarrationIndex >= 0)
        {
            narrationIndex = pendingNarrationIndex + 1;
            pendingNarrationIndex = -1;
        }

        isSpeaking = false;
        speakingCoroutine = null;
    }

    void ForceStopSpeech()
    {
        if (speakingCoroutine != null)
        {
            StopCoroutine(speakingCoroutine);
            speakingCoroutine = null;
        }

        if (voiceSource.isPlaying)
            voiceSource.Stop();

        isSpeaking = false;
        pendingNarrationIndex = -1;
    }

    // ---------- TEXT ----------
    string[] GetNarrationForScene()
    {
        if (CurrentScene == "HubScene")
        {
            return new string[]
            {
                "Welcome... I am your virtual companion for this experience... This space is designed to help you feel calm and grounded... When you are ready... you can choose a mode to explore",
            };
        }

        if (CurrentScene == "CampScene")
        {
            return new string[]
            {
                "Welcome to the campsite… a place of quiet warmth and open air",
                "You may notice the gentle breeze moving through the trees",
                "The ground beneath you feels steady and supportive",
                "Soft natural sounds create a calm rhythm around you",
                "This space invites you to slow down and observe",
                "There is no destination you need to reach here",
                "Each moment is complete just as it is",
                "You can let your shoulders relax naturally",
                "Breathing becomes easier in this open environment",
                "Take your time… the campsite is here for you"
            };
        }

        if (CurrentScene == "BuildingScene")
        {
            return new string[]
            {
                "Welcome to the park… a familiar and peaceful place",
                "The space around you feels calm and unhurried",
                "You may notice subtle movements of light and shadow",
                "Benches and pathways invite gentle exploration",
                "There is comfort in the quiet structure of this place",
                "Nothing here demands your attention",
                "You can move or pause whenever you like",
                "The environment supports stillness and reflection",
                "Moments pass softly here",
                "You are welcome to simply be present"
            };
        }

        return new string[] { defaultText };
    }
    // ---------- ENVIRONMENT / MANUAL TRIGGER ----------
    public void PlayImmediateNarration(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        // Stop current narration and speak immediately
        ForceStopSpeech();
        StartSpeech(text);
    }
}
