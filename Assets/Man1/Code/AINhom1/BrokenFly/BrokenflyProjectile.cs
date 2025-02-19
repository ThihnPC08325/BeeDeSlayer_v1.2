using UnityEngine;

public class BrokenflyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float damage = 15f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float penertration = 0.5f;

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
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, penertration);
                cameraController.GetComponent<CameraController>().ApplyConfusion(3f); // 5 seconds of confusion

            }
            Debug.Log($"Projectile hit player for {damage} damage.");
            Destroy(gameObject);
        }
    }
}
