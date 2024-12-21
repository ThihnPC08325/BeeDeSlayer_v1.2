using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBehavior : MonoBehaviour
{
    [SerializeField] public Transform target; // Người chơi sẽ được gán vào đây
    [SerializeField] float speed = 70f; // Tốc độ ném viên đá
    [SerializeField] private Vector3 throwDirection; // Hướng ném viên đá
    [SerializeField] private float damage = 10f; // Sát thương khi viên đá va chạm với người chơi
    [SerializeField] private GameObject smokerFrefab;
    void Start()
    {
        if (target != null)
        {
            throwDirection = (target.position - transform.position).normalized;
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Thêm lực ném vào viên đá
                rb.velocity = throwDirection * speed;
            }

            // Tự động xóa viên đá sau 5 giây nếu không va chạm
            StartCoroutine(destroyRock());
        }
    }

    IEnumerator destroyRock()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
        Instantiate(smokerFrefab, transform.position, Quaternion.identity);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            Instantiate(smokerFrefab, transform.position, Quaternion.identity);
        }
    }
}