using UnityEngine;
using TMPro;

public class GhostProfessor : MonoBehaviour
{
    public string[] hints; // Danh sách câu gợi ý
    public GameObject hintUI; // UI hiển thị hội thoại
    public float npcLifetime = 30f; // Thời gian tồn tại của NPC
    private TextMeshProUGUI hintText;
    private bool isBeeNearby = false;
    private Transform player;
    private float timer;

    void Start()
    {
        hintText = hintUI.GetComponentInChildren<TextMeshProUGUI>();
        hintUI.SetActive(false); // Ẩn UI khi bắt đầu
        timer = npcLifetime; // Đếm ngược 30 giây
        player = GameObject.FindGameObjectWithTag("Player").transform; // Lấy vị trí Player
    }

    void Update()
    {
        timer -= Time.deltaTime; // Đếm ngược thời gian

        if (timer <= 0f && !isBeeNearby)
        {
            Destroy(gameObject); // Nếu hết 30s mà Bee không vào vùng, NPC biến mất
        }

        if (isBeeNearby)
        {
            LookAtPlayer(); // NPC quay mặt về hướng Bee
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isBeeNearby = true;
            ShowHint();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isBeeNearby = false;
            HideHint();
        }
    }

    void ShowHint()
    {
        if (!isBeeNearby) return;

        // Chọn ngẫu nhiên một câu gợi ý từ danh sách
        string hint = hints[Random.Range(0, hints.Length)];
        hintText.text = $"👻 Ghost: {hint}";
        hintUI.SetActive(true);
    }

    void HideHint()
    {
        hintUI.SetActive(false);
    }

    void LookAtPlayer()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0; // Không xoay theo trục Y để tránh nghiêng đầu
            transform.forward = direction;
        }
    }
}
