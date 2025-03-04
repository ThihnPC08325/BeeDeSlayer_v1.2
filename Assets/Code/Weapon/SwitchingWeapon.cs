using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwitchingWeapon : MonoBehaviour
{
    private static readonly int Reload = Animator.StringToHash("Reload");
    private PlayerInput _playerInput;
    private InputAction _bullet;

    public enum AmmoType
    {
        Light,    // Đạn nhẹ (pistol, SMG)
        Medium,   // Đạn trung (AR)
        Heavy,    // Đạn nặng (Sniper)
        Shotgun   // Đạn shotgun
    }

    [System.Serializable]
    public class WeaponAmmo
    {
        public string weaponName;
        public AmmoType ammoType;
        public int currentAmmo;
        public int maxAmmo;
        public Animator animator;
        public int AddAmmo(int ammoToAdd)
        {
            int ammoBeforePickup = currentAmmo;
            currentAmmo = Mathf.Min(currentAmmo + ammoToAdd, maxAmmo);
            return currentAmmo - ammoBeforePickup; // Số đạn thực sự được thêm vào
        }
    }

    [SerializeField] private int selectedWeaponIndex = 0;
    [SerializeField] private float scrollCooldown = 0.2f;
    [SerializeField] private WeaponAmmo[] weaponAmmos;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI ammoText;

    [Header("Color Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color criticalColor = Color.red;
    [SerializeField] private float warningThreshold = 0.25f; // Ngưỡng cảnh báo khi người chơi gần hết đạn (%)

    private float _lastScrollTime;
    private int _weaponCount;
    private const string MouseScrollAxis = "Mouse ScrollWheel";
    private const float ScrollThreshold = 0f;
    private Transform[] _weaponTransforms;
    private Dictionary<AmmoType, List<WeaponAmmo>> _ammoTypeMap;
    private Dictionary<AmmoType, WeaponAmmo[]> _ammoCache;
    private Queue<StringBuilder> _stringBuilderPool;
    private const int PoolSize = 5;
    private float _timeReloadBullet;

    //void OnEnable()
    //{
    //    GameEvents.OnAmmoPickup += HandleAmmoPickup;
    //    bullet = playerInput.Player.Reload;
    //    bullet.Enable();
    //}

    void OnDisable()
    {
        GameEvents.OnAmmoPickup -= HandleAmmoPickup;
    }

    private void Awake()
    {
        // Cache weapon transforms
        _weaponCount = transform.childCount;
        _weaponTransforms = new Transform[_weaponCount];
        for (int i = 0; i < _weaponCount; i++)
        {
            _weaponTransforms[i] = transform.GetChild(i);
        }

        // Tạo map cho ammo types
        InitializeAmmoTypeMap();
        InitializePooling();
        CacheComponents();
        ValidateInitialWeapon();
        SelectWeapon(selectedWeaponIndex);
    }

    private void Update()
    {
        if (!CanSwitchWeapon()) return;

        int newWeaponIndex = GetNewWeaponIndex();
        if (newWeaponIndex != selectedWeaponIndex)
        {
            SelectWeapon(newWeaponIndex);
        }
    }

    private bool CanSwitchWeapon() => Time.time - _lastScrollTime >= scrollCooldown;

    private int GetNewWeaponIndex()
    {
        // Xử lý scroll input
        float scrollInput = Input.GetAxis(MouseScrollAxis);
        if (Mathf.Abs(scrollInput) > ScrollThreshold)
        {
            _lastScrollTime = Time.time;
            return HandleScrollInput(scrollInput);
        }

        // Xử lý numeric input
        for (int i = 0; i < _weaponCount; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                return i;
        }

        return selectedWeaponIndex;
    }

    private void InitializePooling()
    {
        _stringBuilderPool = new Queue<StringBuilder>(PoolSize);
        for (int i = 0; i < PoolSize; i++)
        {
            _stringBuilderPool.Enqueue(new StringBuilder(20)); // Predefine capacity
        }
    }

    private void CacheComponents()
    {
        // Cache weapon transforms - Chỉ thực hiện 1 lần
        _weaponCount = transform.childCount;
        _weaponTransforms = new Transform[_weaponCount];
        for (int i = 0; i < _weaponCount; i++)
        {
            _weaponTransforms[i] = transform.GetChild(i);
        }

        // Tối ưu dictionary bằng cách nhóm theo AmmoType
        _ammoCache = weaponAmmos
            .GroupBy(w => w.ammoType)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }

    private StringBuilder GetStringBuilder()
    {
        return _stringBuilderPool.Count > 0
            ? _stringBuilderPool.Dequeue()
            : new StringBuilder(20);
    }

    private void ReleaseStringBuilder(StringBuilder sb)
    {
        if (_stringBuilderPool.Count >= PoolSize) return;
        sb.Clear();
        _stringBuilderPool.Enqueue(sb);
    }

    private void InitializeAmmoTypeMap()
    {
        _ammoTypeMap = new Dictionary<AmmoType, List<WeaponAmmo>>();
        foreach (WeaponAmmo ammo in weaponAmmos)
        {
            if (!_ammoTypeMap.ContainsKey(ammo.ammoType))
            {
                _ammoTypeMap[ammo.ammoType] = new List<WeaponAmmo>();
            }
            _ammoTypeMap[ammo.ammoType].Add(ammo);
        }
    }

    private int HandleScrollInput(float scrollValue)
    {
        int newIndex = selectedWeaponIndex + (scrollValue > 0 ? 1 : -1);

        // Wrap around
        if (newIndex >= _weaponCount) return 0;
        if (newIndex < 0) return _weaponCount - 1;

        return newIndex;
    }

    private void SelectWeapon(int index)
    {
        if (!IsValidWeaponIndex(index)) return;

        selectedWeaponIndex = index;

        for (int i = 0; i < _weaponCount; i++)
        {
            _weaponTransforms[i].gameObject.SetActive(i == selectedWeaponIndex);
        }

        UpdateBulletDisplay();
    }

    private bool IsValidWeaponIndex(int index)
    {
        return index >= 0 && index < _weaponCount;
    }

    public void ValidateInitialWeapon()
    {
        if (_weaponCount == 0)
        {
            Debug.LogError("No weapons found!");
            return;
        }

        selectedWeaponIndex = Mathf.Clamp(selectedWeaponIndex, 0, _weaponCount - 1);
    }

    private void UpdateAmmoDisplay()
    {
        if (ammoText == null || !IsValidWeaponIndex(selectedWeaponIndex)) return;

        WeaponAmmo currentWeapon = weaponAmmos[selectedWeaponIndex];
        var sb = GetStringBuilder();

        try
        {
            sb.Append(currentWeapon.currentAmmo)
              .Append('/')
              .Append(currentWeapon.maxAmmo);
            ammoText.text = sb.ToString();
        }
        finally
        {
            ReleaseStringBuilder(sb);
        }

        UpdateAmmoColor(currentWeapon);
    }

    private void UpdateAmmoColor(WeaponAmmo weapon)
    {
        if (weapon.currentAmmo == 0)
        {
            ammoText.color = criticalColor;
        }
        else if (weapon.currentAmmo <= weapon.maxAmmo * warningThreshold)
        {
            ammoText.color = warningColor;
        }
        else
        {
            ammoText.color = normalColor;
        }
    }

    public void UseAmmo(int amount = 1)
    {
        if (!IsValidWeaponIndex(selectedWeaponIndex)) return;
        WeaponAmmo currentWeapon = weaponAmmos[selectedWeaponIndex];
        currentWeapon.currentAmmo = Mathf.Max(0, currentWeapon.currentAmmo - amount);
        UpdateBulletDisplay();
    }

    public bool HasAmmo()
    {
        return IsValidWeaponIndex(selectedWeaponIndex) &&
               weaponAmmos[selectedWeaponIndex].currentAmmo > 0;
    }

    void HandleAmmoPickup(AmmoType ammoType, int amount)
    {
        RestoreAmmo(ammoType, amount);
    }

    public int RestoreAmmo(AmmoType ammoType, int ammo)
    {
        if (!_ammoCache.TryGetValue(ammoType, out var weapons)) return 0;
        int totalAmmoAdded = weapons.Sum(weapon => weapon.AddAmmo(ammo));

        if (weapons.Any(w => Array.IndexOf(weaponAmmos, w) == selectedWeaponIndex))
        {
            UpdateBulletDisplay();
        }

        return totalAmmoAdded;
    }


    private void OnReload()
    {
        WeaponAmmo currentWeapon = weaponAmmos[selectedWeaponIndex];
        if (currentWeapon != null)
        {
            if (currentWeapon.currentAmmo != currentWeapon.maxAmmo)
            {
                currentWeapon.animator.SetTrigger(Reload);
                StartCoroutine(Rebullet());
            }
            else
            {
                Debug.Log("Đạn đã đầy");
            }
        }
        else
        {
            System.Diagnostics.Debug.Assert(currentWeapon != null, nameof(currentWeapon) + " != null");
        }
    }

    public IEnumerator Rebullet()
    {
        WeaponAmmo currentWeapon = weaponAmmos[selectedWeaponIndex];

        RuntimeAnimatorController controller = currentWeapon.animator.runtimeAnimatorController;
        AnimationClip[] clips = controller.animationClips;
        foreach (AnimationClip clip in clips)
        {
            _timeReloadBullet = clip.length;
        }
        Debug.Log(_timeReloadBullet);
        yield return new WaitForSeconds(_timeReloadBullet);
        currentWeapon.currentAmmo = currentWeapon.maxAmmo;
        UpdateBulletDisplay();
    }
    private void UpdateBulletDisplay()
    {
        if (!ammoText || !IsValidWeaponIndex(selectedWeaponIndex)) return;

        WeaponAmmo currentWeapon = weaponAmmos[selectedWeaponIndex];
        currentWeapon.animator.Play("Idle");
        StopAllCoroutines();
        var sb = GetStringBuilder();

        try
        {
            sb.Append(currentWeapon.currentAmmo)
              .Append('/')
              .Append(currentWeapon.maxAmmo);
            ammoText.text = sb.ToString();
        }
        finally
        {
            ReleaseStringBuilder(sb);
        }

        UpdateAmmoColor(currentWeapon);
    }
}
