using System.Collections;
using UnityEngine;
using System;

public class ARGun : MonoBehaviour
{
    [Header("System")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private SwitchingWeapon weaponSwitcher;
    [SerializeField] private BulletPool.PoolType bulletType;

    [Header("Shooting")]
    [SerializeField] private float shootingDelay;
    [SerializeField] private ShootMode currentShootMode;

    private readonly Vector3 _gravity = Physics.gravity; // Sử dụng trọng lực mặc định của Unity
    private void UpdateBulletTrajectory(Rigidbody bulletRb, float deltaTime)
    {
        // Áp dụng trọng lực
        bulletRb.velocity += _gravity * deltaTime;
    }

    private WeaponRecoilSystem _recoilSystem;
    private CalculateMuzzleVelocity _calculateMuzzleVelocity;
    private bool _isShooting, _readyToShoot;
    private bool _allowReset = true;
    private const int BulletPerBurst = 3;
    private int _currentBurst;
    private readonly float _lastShotTime;

    private enum ShootMode
    {
        Single,
        Burst,
        Auto
    }

    private void UpdateShootingParameters()
    {
        switch (currentShootMode)
        {
            case ShootMode.Single:
                shootingDelay = 0.2f;

                break;
            case ShootMode.Burst:
                shootingDelay = 0.1f;

                break;
            case ShootMode.Auto:
                shootingDelay = 0.2f;

                break;
        }
    }

    private void Awake()
    {
        _readyToShoot = true;
        _currentBurst = BulletPerBurst;
        _recoilSystem = GetComponent<WeaponRecoilSystem>();
        _calculateMuzzleVelocity = GetComponent<CalculateMuzzleVelocity>();
    }

    private void Update()
    {
        _isShooting = currentShootMode switch
        {
            // Kiểm tra đầu vào để bắn
            ShootMode.Single or ShootMode.Burst => Input.GetButtonDown("Fire1"),
            ShootMode.Auto => Input.GetButton("Fire1"),
            _ => _isShooting
        };

        // Kiểm tra để đổi chế độ bắn
        if (Input.GetKeyDown(KeyCode.B))
        {
            SwitchShootMode();
        }

        if (_readyToShoot && _isShooting && weaponSwitcher.HasAmmo())
        {
            _currentBurst = BulletPerBurst;
            FireWeapon();
            _recoilSystem.ApplyRecoil();
            weaponSwitcher.UseAmmo();
        }

        _recoilSystem.HandleRecoil();
        _recoilSystem.HandleSpread();
    }

    private void SwitchShootMode()
    {
        currentShootMode = (ShootMode)(((int)currentShootMode + 1) % Enum.GetValues(typeof(ShootMode)).Length);

        UpdateShootingParameters();

        // Hiển thị thông báo chế độ bắn mới
        StartCoroutine(ShowShootModeMessage());
    }

    private IEnumerator ShowShootModeMessage()
    {
        string modeMessage = $"Chế độ bắn: {currentShootMode}";
        // Hiển thị modeMessage trên UI
        Debug.Log(modeMessage);

        yield return new WaitForSeconds(2f); // Hiển thị trong 2 giây
    }

    private void FireWeapon()
    {
        if (!_readyToShoot) return;

        _readyToShoot = false;
        PlayMuzzleEffect();
        FireBullet();
        HandleShootingModes();
    }

    private void PlayMuzzleEffect()
    {
        muzzleFlash?.Play();
    }

    private void FireBullet()
    {
        Vector3 shootingDirection = BulletDirectionCalculator.CalculateDirection(bulletSpawn);
        float muzzleVelocity = _calculateMuzzleVelocity.MuzzleVelocity();

        GameEvents.TriggerWeaponFire(bulletSpawn.position, shootingDirection, muzzleVelocity);

        GameObject bullet = BulletPool.Instance.SpawnFromPool(bulletType, bulletSpawn.position, bulletSpawn.rotation);

        var bulletRb = bullet.GetComponent<Rigidbody>();

        ApplyBulletPhysics(bulletRb, shootingDirection, muzzleVelocity);
    }

    private void ApplyBulletPhysics(Rigidbody bulletRb, Vector3 direction, float velocity)
    {
        bulletRb.velocity = direction * velocity;

        StartCoroutine(HandleBulletPhysics(bulletRb));
    }

    private IEnumerator HandleBulletPhysics(Rigidbody bulletRb)
    {
        while (bulletRb && bulletRb.gameObject.activeInHierarchy)
        {
            UpdateBulletTrajectory(bulletRb, Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
    }

    private void HandleShootingModes()
    {
        if (_allowReset)
        {
            Invoke(nameof(ResetShoot), shootingDelay);
            _allowReset = false;
        }

        if (currentShootMode != ShootMode.Burst || _currentBurst <= 1) return;
        _currentBurst--;
        Invoke(nameof(FireWeapon), shootingDelay);
    }

    private void ResetShoot()
    {
        _readyToShoot = true;
        _allowReset = true;
    }

}