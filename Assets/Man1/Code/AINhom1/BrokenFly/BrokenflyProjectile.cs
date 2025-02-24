using UnityEngine;

public class BrokenflyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float damage = 15f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float penertration = 0.5f;
    [SerializeField] private float duration = 3f; // Total duration of the debuff


    private Vector3 direction;

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
            // Deal damage to the player
            PlayerHealth playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
            PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            CameraController cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
            BrokenFlyDebuff playerDebuff = GameObject.FindGameObjectWithTag("Player").GetComponent<BrokenFlyDebuff>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, penertration);
                cameraController.GetComponent<CameraController>().ApplyConfusion(2f); // 5 seconds of confusion
                Debug.Log($"Projectile hit caused debuff");
                playerDebuff.ApplyDebuff(duration);

            }
            Debug.Log($"Projectile hit player for {damage} damage.");
            Destroy(gameObject);
        }
    }
}
