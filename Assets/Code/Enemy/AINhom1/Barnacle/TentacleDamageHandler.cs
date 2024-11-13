using UnityEngine;

public class TentacleDamageHandler : MonoBehaviour
{
    private TentacleHead tentacleHead;

    private void Awake()
    {
        tentacleHead = GetComponent<TentacleHead>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object hitting the tentacle head is a damaging object
        if (collision.gameObject.CompareTag("Bullet") || collision.gameObject.CompareTag("PlayerAttack"))
        {
            Debug.Log("Blah");
            // Assuming each hit does a certain amount of damage, you can adjust this
            float damage = 10f;
            tentacleHead.TakeDamage(damage);
        }
    }
}
