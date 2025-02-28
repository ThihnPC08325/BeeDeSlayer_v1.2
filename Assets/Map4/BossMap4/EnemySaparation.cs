using UnityEngine;

public class EnemySeparation : MonoBehaviour
{
    public float separationRadius = 2f; // Bán kính phát hiện va chạm
    public float pushForce = 5f; // Lực đẩy quái ra xa

    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, separationRadius);
        foreach (Collider col in colliders)
        {
            if (col.gameObject != gameObject && col.CompareTag("Enemy"))
            {
                Vector3 pushDirection = transform.position - col.transform.position;
                pushDirection.y = 0; // Không đẩy theo trục Y
                transform.position += pushDirection.normalized * (pushForce * Time.deltaTime);
            }
        }
    }
}
