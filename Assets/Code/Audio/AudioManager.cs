using UnityEngine;
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
        [SerializeField] SoundType _soundType;
        [SerializeField] AudioClip _clip;

        // Thêm các thuộc tính điều chỉnh âm thanh
        [Range(0f, 1f)]
        [SerializeField] float _volume = 1f;
        [Range(0.5f, 1.5f)]
        [SerializeField] float _pitch = 1f;
        [Range(-1f, 1f)]
        [SerializeField] float _stereoPan = 0f;
        [SerializeField] bool _loop = false;

        // 3D sound settings
        [SerializeField] bool _is3D;
        [Range(0f, 360f)]
        [SerializeField] float _spread = 0f;
        [Range(0f, 5f)]
        [SerializeField] float _dopplerLevel = 1f;
        [Range(0f, 50f)]
        [SerializeField] float _minDistance = 1f;
        [Range(1f, 500f)]
        [SerializeField] float _maxDistance = 100f;

        // Properties
        public SoundType SoundType => _soundType;
        public AudioClip Clip => _clip;
        public float Volume => _volume;
        public float Pitch => _pitch;
        public float StereoPan => _stereoPan;
        public bool Loop => _loop;
        public bool Is3D => _is3D;
        public float Spread => _spread;
        public float DopplerLevel => _dopplerLevel;
        public float MinDistance => _minDistance;
        public float MaxDistance => _maxDistance;
    }

    [SerializeField] private SoundEffect[] soundEffects;

    private AudioSource[] audioSources;
    private static AudioManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            audioSources = new AudioSource[soundEffects.Length];
            for (int i = 0; i < soundEffects.Length; i++)
            {
                audioSources[i] = gameObject.AddComponent<AudioSource>();
                InitializeAudioSource(audioSources[i], soundEffects[i]);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        GameEvents.OnAmmoPickup += PlayAmmoPickupSound;
        GameEvents.OnHealthPickup += PlayHealthPickupSound;
        GameEvents.OnWeaponFire += PlayWeaponFireSound;
        GameEvents.OnPlayerHit += PlayPlayerHitSound;
        GameEvents.OnEnemyHit += PlayEnemyHitSound;
    }

    void OnDisable()
    {
        GameEvents.OnAmmoPickup -= PlayAmmoPickupSound;
        GameEvents.OnHealthPickup -= PlayHealthPickupSound;
        GameEvents.OnWeaponFire -= PlayWeaponFireSound;
        GameEvents.OnPlayerHit -= PlayPlayerHitSound;
        GameEvents.OnEnemyHit -= PlayEnemyHitSound;
    }

    private void InitializeAudioSource(AudioSource source, SoundEffect soundEffect)
    {
        // Basic settings
        source.clip = soundEffect.Clip;
        source.volume = soundEffect.Volume;
        source.pitch = soundEffect.Pitch;
        source.panStereo = soundEffect.StereoPan;
        source.loop = soundEffect.Loop;

        // 3D sound settings
        source.spatialBlend = soundEffect.Is3D ? 1f : 0f;
        if (soundEffect.Is3D)
        {
            source.spread = soundEffect.Spread;
            source.dopplerLevel = soundEffect.DopplerLevel;
            source.minDistance = soundEffect.MinDistance;
            source.maxDistance = soundEffect.MaxDistance;
            source.rolloffMode = AudioRolloffMode.Linear;
        }
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
                return audioSources[i];
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
        audioSources[soundIndex].Play();
    }

    private void PlaySoundAtPosition(int soundIndex, Vector3 position)
    {
        AudioSource source = audioSources[soundIndex];
        source.transform.position = position;
        float distance = Vector3.Distance(Camera.main.transform.position, position);
        float volumeScale = CalculateVolumeByDistance(distance, soundEffects[soundIndex]);
        source.volume = soundEffects[soundIndex].Volume * volumeScale;
        source.Play();
    }
    #endregion

    #region CALCULATE VOLUME
    private float CalculateVolumeByDistance(float distance, SoundEffect soundEffect)
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