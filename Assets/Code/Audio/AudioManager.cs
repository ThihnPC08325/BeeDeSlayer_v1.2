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
        [Range(0f, 1f)]
        [SerializeField] float _volume = 1f;
        [SerializeField] bool _is3D;
        [Range(1f, 500f)]
        [SerializeField] float _maxDistance;
        [Range(0f, 50f)]
        [SerializeField] float _minDistance = 1f;

        public SoundType SoundType => _soundType;
        public AudioClip Clip => _clip;
        public float Volume => _volume;
        public bool Is3D => _is3D;
        public float MaxDistance => _maxDistance;
        public float MinDistance => _minDistance;
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
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSources = new AudioSource[soundEffects.Length];
        for (int i = 0; i < soundEffects.Length; i++)
        {
            audioSources[i] = gameObject.AddComponent<AudioSource>();
            audioSources[i].clip = soundEffects[i].Clip;
            audioSources[i].volume = soundEffects[i].Volume;
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