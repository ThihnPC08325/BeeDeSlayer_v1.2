using System.Collections;
using UnityEngine;

public class RedHeal : MonoBehaviour
{
    public int HP = 100; // Điểm sức khỏe của rồng
    public Animator animator; // Animator để điều khiển hoạt ảnh
    public GameObject[] effects; // Hiệu ứng liên quan đến rồng

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(10);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("BigExplosion1"))
        {
            TakeDamage(30);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;
        if (HP <= 0)
        {
            StartCoroutine(DestroyDragon());
        }
        else
        {
            animator.SetTrigger("damage");
        }
    }

    private IEnumerator DestroyDragon()
    {
        animator.SetTrigger("die"); // Chạy animation chết
        GetComponent<Collider>().enabled = false; // Vô hiệu hóa va chạm
        foreach (GameObject effect in effects)
        {
            if (effect != null)
                Destroy(effect); // Xóa hiệu ứng liên quan
        }

        // Đợi animation chết hoàn tất (dùng AnimatorStateInfo)
        float deathAnimLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(deathAnimLength);

        // Kiểm tra lại nếu rồng chưa bị xóa
        if (gameObject != null)
        {
            yield return new WaitForSeconds(3f); // Đợi 2 giây sau animation chết
            Destroy(gameObject); // Xóa rồng
        }

        Debug.Log("Bắt đầu xóa rồng..."); // Debug kiểm tra

        animator.SetTrigger("die");
        GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(2f);

        Debug.Log("Đang xóa rồng...");
        Destroy(gameObject);
    }
}
