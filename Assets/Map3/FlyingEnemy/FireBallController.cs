using System.Collections;
using UnityEngine;

public class FireBallController : MonoBehaviour
{
    public Vector3 Target;
    [SerializeField] private float speed = 10f;
    [SerializeField] private Vector3 explosionScale = new Vector3(10, 7, 10);
    [SerializeField] private GameObject BigExplosionEffect;
    [SerializeField] private float dame = 10;
    [SerializeField] private float penetration = 2;
    private MeshRenderer MeshRenderer;
    private bool isExplosion = false;

    private void Start()
    {
        MeshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, Target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, Target) < 0.1f)
        {
            transform.localScale = explosionScale;
            MeshRenderer.enabled = false;
            if (!isExplosion)
            {
                StartCoroutine(DestroyBomb());
                isExplosion = true;
            }
        }
    }

    private IEnumerator DestroyBomb()
    {
        if (BigExplosionEffect != null)
        {
            Instantiate(BigExplosionEffect, transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(dame,penetration);
            }
        }
    }
}