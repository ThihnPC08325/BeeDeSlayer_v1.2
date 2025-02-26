using UnityEngine;
using TMPro;

public class GhostProfessor : MonoBehaviour
{
    public string[] hints; // Danh sách câu gợi ý
    public GameObject hintUI; // UI hiển thị hội thoại
    public float npcLifetime = 30f; // Thời gian tồn tại của NPC
    private TextMeshProUGUI _hintText;
    private bool _isBeeNearby = false;
    private Transform _player;
    private float _timer;

    private void Start()
    {
        _hintText = hintUI.GetComponentInChildren<TextMeshProUGUI>();
        hintUI.SetActive(false); // Ẩn UI khi bắt đầu
        _timer = npcLifetime; // Đếm ngược 30 giây
        _player = GameObject.FindGameObjectWithTag("Player").transform; // Lấy vị trí Player
    }

    private void Update()
    {
        _timer -= Time.deltaTime; // Đếm ngược thời gian

        if (_timer <= 0f && !_isBeeNearby)
        {
            Destroy(gameObject); // Nếu hết 30s mà Bee không vào vùng, NPC biến mất
        }

        if (_isBeeNearby)
        {
            LookAtPlayer(); // NPC quay mặt về hướng Bee
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _isBeeNearby = true;
        ShowHint();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _isBeeNearby = false;
        HideHint();
    }

    private void ShowHint()
    {
        if (!_isBeeNearby) return;

        // Chọn ngẫu nhiên một câu gợi ý từ danh sách
        string hint = hints[Random.Range(0, hints.Length)];
        _hintText.text = $"👻 Ghost: {hint}";
        hintUI.SetActive(true);
    }

    private void HideHint()
    {
        hintUI.SetActive(false);
    }

    private void LookAtPlayer()
    {
        if (!_player) return;
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0; // Không xoay theo trục Y để tránh nghiêng đầu
        transform.forward = direction;
    }
}
