using UnityEngine;

public class WeaponRecoilSystem : MonoBehaviour
{
    [System.Serializable]
    public struct RecoilPattern
    {
        public Vector2[] pattern;
        public float[] zPattern;
    }

    #region Serialized Fields
    [SerializeField] private RecoilPattern recoilPattern;

    [Header("Recoil Settings")]
    [SerializeField, Range(0f, 20f)] float recoilSpeed = 10f;
    [SerializeField, Range(0f, 10f)] float returnSpeed = 5f;
    [SerializeField, Range(0f, 1f)] float recoilDecayRate = 0.9f;
    [SerializeField, Range(0f, 1f)] float randomness = 0.1f;
    [SerializeField, Range(0f, 5f)] float maxRecoilZ = 0.5f;

    [Header("Spread Settings")]
    [SerializeField, Range(0f, 10f)] float maxSpread = 5f;
    [SerializeField, Range(0f, 1f)] float spreadIncreaseRate = 0.1f;
    [SerializeField, Range(0f, 5f)] float spreadDecreaseRate = 1f;
    #endregion

    #region Private Fields
    private Vector3 _currentRotation;
    private Vector3 _targetRotation;
    private Vector3 _initialPosition;
    private float _currentRecoilZ;
    private float _targetRecoilZ;
    private float _currentSpread;
    private float _lastShotTime;
    private int _currentPatternIndex;

    // Cached components
    private Transform _cachedTransform;
    private readonly Vector3 _forwardVector = Vector3.forward;
    private Vector2 _cachedPerlinOffset;
    private float _lastPerlinUpdateTime;
    private const float PerlinUpdateInterval = 0.05f; // Update Perlin noise mỗi 50ms
    #endregion

    private void Awake()
    {
        _cachedTransform = transform;
        _initialPosition = _cachedTransform.localPosition;
        UpdatePerlinOffset();
    }

    public void ApplyRecoil()
    {
        if (_currentPatternIndex >= recoilPattern.pattern.Length)
            _currentPatternIndex = 0;

        Vector2 recoil = recoilPattern.pattern[_currentPatternIndex];
        recoil += GetPerlinNoiseOffset() * randomness;

        float recoilZ = Mathf.Min(recoilPattern.zPattern[_currentPatternIndex], maxRecoilZ);

        ApplyRecoilForces(recoil, recoilZ);
        UpdateSpread();

        _currentPatternIndex++;
        _lastShotTime = Time.time;
    }

    private void UpdatePerlinOffset()
    {
        float time = Time.time;
        _cachedPerlinOffset.x = Mathf.PerlinNoise(time, 0) * 2 - 1;
        _cachedPerlinOffset.y = Mathf.PerlinNoise(0, time) * 2 - 1;
        _lastPerlinUpdateTime = time;
    }

    private Vector2 GetPerlinNoiseOffset()
    {
        if (Time.time - _lastPerlinUpdateTime >= PerlinUpdateInterval)
        {
            UpdatePerlinOffset();
        }
        return _cachedPerlinOffset;
    }

    private void ApplyRecoilForces(Vector2 recoil, float recoilZ)
    {
        _targetRotation *= recoilDecayRate;
        _targetRotation += new Vector3(-recoil.y, recoil.x, 0f);

        _targetRecoilZ = -recoilZ;
        _cachedTransform.localPosition -= _forwardVector * recoilZ;
    }

    private void UpdateSpread()
    {
        _currentSpread = Mathf.Min(_currentSpread + spreadIncreaseRate, maxSpread);
    }

    public void HandleRecoil()
    {
        float deltaTime = Time.deltaTime;
        float lerpFactor = recoilSpeed * deltaTime;
        float returnFactor = returnSpeed * deltaTime;

        _currentRotation = Vector3.Slerp(_currentRotation, _targetRotation, lerpFactor);
        _targetRotation = Vector3.Slerp(_targetRotation, Vector3.zero, returnFactor);

        _currentRecoilZ = Mathf.Lerp(_currentRecoilZ, 0f, returnFactor);

        _cachedTransform.SetLocalPositionAndRotation(Vector3.Lerp(_cachedTransform.localPosition, _initialPosition, returnFactor), Quaternion.Euler(_currentRotation.x, _currentRotation.y, _currentRecoilZ));
    }

    public void HandleSpread()
    {
        if (Time.time - _lastShotTime > 0.1f)
            _currentSpread = Mathf.Max(0, _currentSpread - spreadDecreaseRate * Time.deltaTime);
    }
}