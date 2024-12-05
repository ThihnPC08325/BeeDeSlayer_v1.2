using System;
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
    [SerializeField] float delayAttack = 1.5f;
    [SerializeField] GameObject laserStartPosition;

    // Bomb setting
    [Header("Bomb Setting")]
    [SerializeField] private GameObject BombPrefabs;
    [SerializeField] private ZoneLaserAttack ZoneLaserAttack;
    [SerializeField] private LineRenderer laserLineRenderer;

    // Private variables
    private bool isFiring = false;
    private float lastFireTime;
    private Transform playerTransform;

    // Unity lifecycle methods
    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Public methods
    public void GetFireLaserAtPlayer()
    {
        if (isFiring == false)
        {
            isFiring = true;
            StartCoroutine(FireLaserAtPlayer());
        }
    }

    public void GetBombtoPlayer()
    {
        if (isFiring == false)
        {
            isFiring = true;
            StartCoroutine(GetBomb());
        }
    }

    // Initialization methods
    private void InitializeLaserLineRenderer()
    {
        laserLineRenderer.startWidth = 0.1f;
        laserLineRenderer.endWidth = 0.1f;
        laserLineRenderer.positionCount = 2;

        laserLineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        laserLineRenderer.material.color = Color.red;

        laserLineRenderer.enabled = false;
    }

    // Coroutine methods
    private IEnumerator FireLaserAtPlayer()
    {
        InitializeLaserLineRenderer();
        Debug.Log("Is Firing!");

        //if (Time.time - lastFireTime < fireCooldown)
        //    yield break;

        //if (Vector3.Distance(transform.position, playerTransform.position) > laserMaxDistance)
        //    yield break;

        Vector3 laserDirection = (playerTransform.position - laserStartPosition.transform.position).normalized;

        yield return new WaitForSeconds(delayAttack); // Delay before laser fires

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
                    playerHealth.TakeDamage(laserDamage, 2);
                }
            }
        }

        lastFireTime = Time.time;

        Invoke(nameof(HideLaser), laserVisibleDuration);

        isFiring = false;
    }

    private IEnumerator GetBomb()
    {
        GameObject[] createBomb = new GameObject[3];
        FireBallController[] fireBall = new FireBallController[3];

        ZoneLaserAttack.GetDrawWithDelay();
        for (int i = 0; i < 3; i++)
        {
            createBomb[i] = Instantiate(BombPrefabs, laserStartPosition.transform.position, Quaternion.identity);
            fireBall[i] = createBomb[i].GetComponent<FireBallController>();

            fireBall[i].Target = ZoneLaserAttack.list[i];
            yield return new WaitForSeconds(3f);
        }

        lastFireTime += Time.time;
        isFiring = false;
    }

    // Utility methods
    private void ShowLaser()
    {
        laserLineRenderer.enabled = true;
    }

    private void HideLaser()
    {
        laserLineRenderer.enabled = false;
    }
}