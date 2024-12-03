using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioManagers : MonoBehaviour
{
    public static AudioManagers Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip deathMusic;
    [SerializeField] private AudioClip victoryMusic;

    [Header("Audio Settings")]
    [SerializeField] private float bgmVolume = 0.5f;
    [SerializeField] private float sfxVolume = 0.8f;
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private float crossFadeDuration = 2f;
    [SerializeField] private AnimationCurve fadeCurve;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    private bool isFading = false;

    private void Awake()
    {
        SetupSingleton();
        InitializeAudioSources();
    }

    private void SetupSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        // Setup BGM Source
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
        }
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = bgmVolume;

        // Setup SFX Source
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;
    }

    private void Start()
    {
        PlayBackgroundMusic();
        LoadAudioSettings();
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && !bgmSource.isPlaying)
        {
            bgmSource.clip = backgroundMusic;
            StartCoroutine(FadeIn(bgmSource));
        }
    }

    public void SwitchToDeathMusic()
    {
        if (!isFading)
        {
            StartCoroutine(CrossFadeMusic(deathMusic));
        }
    }

    public void SwitchToVictoryMusic()
    {
        if (!isFading)
        {
            StartCoroutine(CrossFadeMusic(victoryMusic));
        }
    }

    private IEnumerator CrossFadeMusic(AudioClip newClip)
    {
        isFading = true;

        // Fade out current music
        float timeElapsed = 0;
        float startVolume = bgmSource.volume;

        while (timeElapsed < crossFadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float percentageComplete = timeElapsed / crossFadeDuration;
            bgmSource.volume = Mathf.Lerp(startVolume, 0, fadeCurve.Evaluate(percentageComplete));
            yield return null;
        }

        // Change clip and fade in new music
        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.Play();
        timeElapsed = 0;

        while (timeElapsed < crossFadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float percentageComplete = timeElapsed / crossFadeDuration;
            bgmSource.volume = Mathf.Lerp(0, startVolume, fadeCurve.Evaluate(percentageComplete));
            yield return null;
        }

        isFading = false;
    }

    private IEnumerator FadeIn(AudioSource audioSource)
    {
        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < bgmVolume)
        {
            audioSource.volume += Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    private IEnumerator FadeOut(AudioSource audioSource)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    // Volume Control Methods
    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        bgmSource.volume = volume;
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    private void LoadAudioSettings()
    {
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        SetBGMVolume(bgmVolume);
        SetSFXVolume(sfxVolume);
    }
}