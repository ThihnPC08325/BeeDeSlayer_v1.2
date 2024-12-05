using System.Collections;
using UnityEngine;

public class FireBallController : MonoBehaviour
{
    public Vector3 Target;
    private float speed = 10f;
    private bool isExplosion = false;
    [SerializeField] float dame = 10f;
    [SerializeField] float penetration = 2;
    [SerializeField] private GameObject BigExplosionEffect;

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, Target, speed * Time.deltaTime);

        if (transform.position == Target)
        {
            transform.localScale = new Vector3(6, 6, 6);
            if (!isExplosion)
            {
                StartCoroutine(DestroyBomb());
                isExplosion = true;
            }
        }
    }

    private IEnumerator DestroyBomb()
    {
        Instantiate(BigExplosionEffect, transform.position, Quaternion.identity);

        gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(dame,penetration);
        }
    }
}
