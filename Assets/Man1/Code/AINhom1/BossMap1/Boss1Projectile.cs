using UnityEngine;
using System.Collections;

public class Boss1Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float damage = 15f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float penetration = 0.5f;
    [SerializeField] private float duration = 3f; // Total duration of the debuff
    [SerializeField] private float dotDamage = 5f; // Damage per DOT tick
    [SerializeField] private int dotTicks = 3; // Number of times DOT applies
    [SerializeField] private float dotInterval = 1f; // Time between each DOT tick

    private Vector3 direction;
    private bool dotEnabled = true; // Toggle for DOT effect

    private void Start()
    {
        Destroy(gameObject, lifetime); // Destroy the projectile after a set time
    }

    private void Update()
    {
        if (direction != Vector3.zero)
        {
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            PlayerDebuffEffect playerDebuff = other.GetComponent<PlayerDebuffEffect>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, penetration);
                playerHealth.ApplyDOT(dotDamage, dotTicks, dotInterval);
                playerDebuff.ApplyDebuff(duration);
            }

            
            
            Destroy(gameObject);
        }
    }
}
