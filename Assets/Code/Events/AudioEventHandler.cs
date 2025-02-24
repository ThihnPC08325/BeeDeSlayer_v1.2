using UnityEngine;
using static SwitchingWeapon;

[System.Serializable]
public class AudioEventMapping
{
    [SerializeField] private string _soundName;
    [SerializeField] private AudioSource _source;
    [SerializeField] private bool _useFade;
    [SerializeField] private float _fadeTime = 1f;
    [SerializeField][Range(0f, 1f)] private float _volumeMultiplier = 1f;
    [SerializeField][Range(-3f, 3f)] private float _pitchVariation = 0f;

    public string SoundName => _soundName;
    public AudioSource Source => _source;
    public bool UseFade => _useFade;
    public float FadeTime => _fadeTime;
    public float VolumeMultiplier => _volumeMultiplier;
    public float PitchVariation => _pitchVariation;
}

public class AudioEventHandler : MonoBehaviour
{
    [System.Serializable]
    private class EventSoundMappings
    {
        public AudioEventMapping ammoPickup;
        public AudioEventMapping healthPickup;
        public AudioEventMapping PlayerFire;
        public AudioEventMapping playerHit;
        public AudioEventMapping enemyHit;
    }

    [SerializeField] private EventSoundMappings _soundMappings;

    private void OnEnable()
    {
        // Đăng ký các events
        GameEvents.OnAmmoPickup += HandleAmmoPickup;
        GameEvents.OnHealthPickup += HandleHealthPickup;
        GameEvents.OnWeaponFire += HandleWeaponFire;
        GameEvents.OnPlayerHit += HandlePlayerHit;
        GameEvents.OnEnemyHit += HandleEnemyHit;
    }

    private void OnDisable()
    {
        // Hủy đăng ký các events
        GameEvents.OnAmmoPickup -= HandleAmmoPickup;
        GameEvents.OnHealthPickup -= HandleHealthPickup;
        GameEvents.OnWeaponFire -= HandleWeaponFire;
        GameEvents.OnPlayerHit -= HandlePlayerHit;
        GameEvents.OnEnemyHit -= HandleEnemyHit;
    }

    private void HandleAmmoPickup(AmmoType ammoType, int amount)
    {
        PlaySoundWithVariation(_soundMappings.ammoPickup);
    }

    private void HandleHealthPickup(float amount)
    {
        PlaySoundWithVariation(_soundMappings.healthPickup);
    }

    void HandleWeaponFire(Vector3 firePosition, Vector3 direction, float muzzleVelocity)
    {
        PlaySoundWithVariation(_soundMappings.PlayerFire);
    }

    private void HandlePlayerHit(float damage, float penetration)
    {
        // Có thể điều chỉnh pitch dựa trên damage
        float pitchMultiplier = Mathf.Lerp(0.8f, 1.2f, damage / 100f);
        PlaySoundWithVariation(_soundMappings.playerHit);
    }

    private void HandleEnemyHit(float damage, GameObject enemy)
    {
        PlaySoundWithVariation(_soundMappings.enemyHit);
    }

    private static void PlaySoundWithVariation(AudioEventMapping mapping)
    {
        if (string.IsNullOrEmpty(mapping.SoundName)) return;

        mapping.Source.Play();
    }
}