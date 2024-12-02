using UnityEngine;

public class BatAttack : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float damagePen = 0f;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameEvents.TriggerPlayerHit(damage, damagePen);
        }
    }
}
