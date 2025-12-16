using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music Source")]
    [SerializeField] private AudioSource musicSource;

    [Header("Scene Music")]
    [SerializeField] private AudioClip hubMusic;
    [SerializeField] private AudioClip campMusic;
    [SerializeField] private AudioClip buildingMusic;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Prevent auto-play
        musicSource.playOnAwake = false;

        // Low background music volume
        musicSource.volume = 0.2f;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        // Handle initial scene
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "HubScene":
                PlayMusic(hubMusic);
                break;

            case "CampScene":
                PlayMusic(campMusic);
                break;

            case "BuildingScene":
                PlayMusic(buildingMusic);
                break;

            default:
                StopMusic();
                break;
        }
    }

    void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            StopMusic();
            return;
        }

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    void StopMusic()
    {
        musicSource.Stop();
        musicSource.clip = null;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
