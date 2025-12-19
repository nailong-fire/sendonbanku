using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;

    [Header("Clips")]
    public AudioClip mapMusic;
    public AudioClip battleMusic;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();

        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
    }

    public void PlayMapMusic() => Play(musicSource, mapMusic);

    public void PlayBattleMusic() => Play(musicSource, battleMusic);

    public void PlayClip(AudioClip clip) => Play(musicSource, clip);

    public void StopMusic()
    {
        if (musicSource == null)
            return;

        musicSource.Stop();
        musicSource.clip = null;
    }

    private static void Play(AudioSource source, AudioClip clip)
    {
        if (source == null)
            return;

        if (clip == null)
            return;

        if (source.clip == clip && source.isPlaying)
            return;

        source.Stop();
        source.clip = clip;
        source.Play();
    }
}
