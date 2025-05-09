﻿using UnityEngine;
using UnityEngine.Serialization;
using static SwitchingWeapon;

public enum SoundType
{
    AmmoPickup,
    HealthPickup,
    WeaponFire,
    PlayerHit,

    EnemyHit
    // Thêm các sound type khác nếu cần
}

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class SoundEffect
    {
        [SerializeField] private SoundType soundType;
        [SerializeField] private AudioClip clip;

        // Thêm các thuộc tính điều chỉnh âm thanh
        [Range(0f, 1f)] [SerializeField] private float volume = 1f;
        [Range(0.5f, 1.5f)] [SerializeField] private float pitch = 1f;
        [Range(-1f, 1f)] [SerializeField] private float stereoPan = 0f;
        [SerializeField] private bool loop = false;

        // 3D sound settings
        [SerializeField] private bool is3D;
        [Range(0f, 360f)] [SerializeField] private float spread = 0f;
        [Range(0f, 5f)] [SerializeField] private float dopplerLevel = 1f;
        [Range(0f, 50f)] [SerializeField] private float minDistance = 1f;
        [Range(1f, 500f)] [SerializeField] private float maxDistance = 100f;

        // Properties
        public SoundType SoundType => soundType;
        public AudioClip Clip => clip;
        public float Volume => volume;
        public float Pitch => pitch;
        public float StereoPan => stereoPan;
        public bool Loop => loop;
        public bool Is3D => is3D;
        public float Spread => spread;
        public float DopplerLevel => dopplerLevel;
        public float MinDistance => minDistance;
        public float MaxDistance => maxDistance;
    }

    [SerializeField] private SoundEffect[] soundEffects;

    private AudioSource[] _audioSources;
    private Camera _camera;
    private static AudioManager _instance;

    private void Awake()
    {
        _camera = Camera.main;
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            _audioSources = new AudioSource[soundEffects.Length];
            for (int i = 0; i < soundEffects.Length; i++)
            {
                _audioSources[i] = gameObject.AddComponent<AudioSource>();
                InitializeAudioSource(_audioSources[i], soundEffects[i]);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnAmmoPickup += PlayAmmoPickupSound;
        GameEvents.OnHealthPickup += PlayHealthPickupSound;
        GameEvents.OnWeaponFire += PlayWeaponFireSound;
        GameEvents.OnPlayerHit += PlayPlayerHitSound;
        GameEvents.OnEnemyHit += PlayEnemyHitSound;
    }

    private void OnDisable()
    {
        GameEvents.OnAmmoPickup -= PlayAmmoPickupSound;
        GameEvents.OnHealthPickup -= PlayHealthPickupSound;
        GameEvents.OnWeaponFire -= PlayWeaponFireSound;
        GameEvents.OnPlayerHit -= PlayPlayerHitSound;
        GameEvents.OnEnemyHit -= PlayEnemyHitSound;
    }

    private static void InitializeAudioSource(AudioSource source, SoundEffect soundEffect)
    {
        // Basic settings
        source.clip = soundEffect.Clip;
        source.volume = soundEffect.Volume;
        source.pitch = soundEffect.Pitch;
        source.panStereo = soundEffect.StereoPan;
        source.loop = soundEffect.Loop;

        // 3D sound settings
        source.spatialBlend = soundEffect.Is3D ? 1f : 0f;
        if (!soundEffect.Is3D) return;
        source.spread = soundEffect.Spread;
        source.dopplerLevel = soundEffect.DopplerLevel;
        source.minDistance = soundEffect.MinDistance;
        source.maxDistance = soundEffect.MaxDistance;
        source.rolloffMode = AudioRolloffMode.Linear;
    }

    #region SOUND CONTROL

    public void SetVolume(SoundType soundType, float volume)
    {
        var source = GetAudioSource(soundType);
        if (source != null)
        {
            source.volume = Mathf.Clamp01(volume);
        }
    }

    public void SetPitch(SoundType soundType, float pitch)
    {
        var source = GetAudioSource(soundType);
        if (source != null)
        {
            source.pitch = Mathf.Clamp(pitch, 0.5f, 1.5f);
        }
    }

    public void StopSound(SoundType soundType)
    {
        var source = GetAudioSource(soundType);
        if (source != null && source.isPlaying)
        {
            source.Stop();
        }
    }

    private AudioSource GetAudioSource(SoundType soundType)
    {
        for (int i = 0; i < soundEffects.Length; i++)
        {
            if (soundEffects[i].SoundType == soundType)
                return _audioSources[i];
        }

        return null;
    }

    #endregion

    #region PLAY SOUND

    private void PlaySoundEffect(SoundType soundType, Vector3? position = null)
    {
        for (int i = 0; i < soundEffects.Length; i++)
        {
            if (soundEffects[i].SoundType == soundType)
            {
                if (soundEffects[i].Is3D && position.HasValue)
                {
                    PlaySoundAtPosition(i, position.Value);
                }
                else
                {
                    PlaySound(i);
                }

                return;
            }
        }
    }

    private void PlaySound(int soundIndex)
    {
        _audioSources[soundIndex].Play();
    }

    private void PlaySoundAtPosition(int soundIndex, Vector3 position)
    {
        AudioSource source = _audioSources[soundIndex];
        source.transform.position = position;
        float distance = Vector3.Distance(_camera.transform.position, position);
        float volumeScale = CalculateVolumeByDistance(distance, soundEffects[soundIndex]);
        source.volume = soundEffects[soundIndex].Volume * volumeScale;
        source.Play();
    }

    #endregion

    #region CALCULATE VOLUME

    private static float CalculateVolumeByDistance(float distance, SoundEffect soundEffect)
    {
        if (distance <= soundEffect.MinDistance) return 1f;
        if (distance >= soundEffect.MaxDistance) return 0f;

        return 1f - ((distance - soundEffect.MinDistance) /
                     (soundEffect.MaxDistance - soundEffect.MinDistance));
    }

    #endregion

    #region EVENT

    private void PlayAmmoPickupSound(AmmoType ammoType, int amount)
    {
        PlaySoundEffect(SoundType.AmmoPickup);
    }

    private void PlayHealthPickupSound(float amount)
    {
        PlaySoundEffect(SoundType.HealthPickup);
    }

    private void PlayWeaponFireSound(Vector3 firePosition, Vector3 direction, float muzzleVelocity)
    {
        PlaySoundEffect(SoundType.WeaponFire);
    }

    private void PlayPlayerHitSound(float damage, float penetration)
    {
        PlaySoundEffect(SoundType.PlayerHit);
    }

    private void PlayEnemyHitSound(float damage, GameObject enemy)
    {
        PlaySoundEffect(SoundType.EnemyHit);
    }

    #endregion
}