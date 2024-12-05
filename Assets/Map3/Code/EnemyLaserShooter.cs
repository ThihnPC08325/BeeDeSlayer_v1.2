using System.Collections;
using UnityEngine;

public class EnemyLaserShooter : MonoBehaviour
{
    // Laser settings
    [Header("Laser Settings")]
    [SerializeField] private float fireCooldown = 2f;
    [SerializeField] private float laserDamage = 5f;
    [SerializeField] private float laserMaxDistance = 100f;
    [SerializeField] private float laserVisibleDuration = 0.5f;
    [SerializeField] GameObject laserStartPosition;


    // Private variables
    private bool isFiring = false;
    private float lastFireTime;
    private Transform playerTransform;
    private LineRenderer laserLineRenderer;

    // Unity lifecycle methods
    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        InitializeLaserLineRenderer();
    }

    // Public methods
    public void GetFireLaserAtPlayer()
    {
        if (!isFiring)
        {
            StartCoroutine(FireLaserAtPlayer());
        }
    }

    // Initialization methods
    private void InitializeLaserLineRenderer()
    {
        if (laserLineRenderer == null)
        {
            laserLineRenderer = gameObject.AddComponent<LineRenderer>();
            laserLineRenderer.startWidth = 0.1f;
            laserLineRenderer.endWidth = 0.1f;
            laserLineRenderer.positionCount = 2;

            laserLineRenderer.material = new Material(Shader.Find("Unlit/Color"));
            laserLineRenderer.material.color = Color.red;

            laserLineRenderer.enabled = false;
        }
    }

    // Coroutine methods
    private IEnumerator FireLaserAtPlayer()
    {
        if (isFiring || Time.time - lastFireTime < fireCooldown)
            yield break;

        if (Vector3.Distance(transform.position, playerTransform.position) > laserMaxDistance)
            yield break;

        isFiring = true;

        Vector3 laserDirection = (playerTransform.position - laserStartPosition.transform.position).normalized;

        yield return new WaitForSeconds(1.5f); // Delay before laser fires

        if (Physics.Raycast(laserStartPosition.transform.position, laserDirection, out RaycastHit hit, laserMaxDistance))
        {
            laserLineRenderer.SetPosition(0, laserStartPosition.transform.position);
            laserLineRenderer.SetPosition(1, hit.point);

            ShowLaser();

            if (hit.collider.CompareTag("Player"))
            {
                PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    //playerHealth.TakeDamage(laserDamage);
                }
            }
        }

        lastFireTime = Time.time;

        Invoke(nameof(HideLaser), laserVisibleDuration);

        isFiring = false;
    }

    // Utility methods
    private void ShowLaser()
    {
        if (laserLineRenderer != null)
        {
            laserLineRenderer.enabled = true;
        }
    }

    private void HideLaser()
    {
        if (laserLineRenderer != null)
        {
            laserLineRenderer.enabled = false;
        }
    }
}
