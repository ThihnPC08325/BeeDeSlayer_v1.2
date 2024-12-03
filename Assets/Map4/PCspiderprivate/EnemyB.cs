using UnityEngine;

public class EnemyAttack1 : MonoBehaviour
{
    [Header("Laser Attack Settings")]
    [SerializeField] private float laserAttackCooldown = 2f;
    [SerializeField] private float laserDuration = 0.5f;
    [SerializeField] private Transform laserSpawnPoint;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private float attackRange = 10f; 

    private float lastLaserAttackTime;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        TryLaserAttack();
    }

    private void TryLaserAttack()
    {
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange && Time.time - lastLaserAttackTime >= laserAttackCooldown)
        {
            ShootLaser();
            lastLaserAttackTime = Time.time;
        }
    }

    private void ShootLaser()
    {
        GameObject laserGO = Instantiate(laserPrefab, laserSpawnPoint.position, Quaternion.identity);
        LaserScript laserScript = laserGO.GetComponent<LaserScript>();
        laserScript.SetTarget(player.position);
        laserScript.StartLaser(laserDuration);
    }
}