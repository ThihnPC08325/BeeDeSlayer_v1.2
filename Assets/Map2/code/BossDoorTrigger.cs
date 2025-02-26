using UnityEngine;
using UnityEngine.SceneManagement;

public class BossDoorTrigger : MonoBehaviour
{
    [SerializeField] private string bossSceneName = "BossScene"; // Tên scene boss
    [SerializeField] private Animator playerAnimator; // Animator của Player
    [SerializeField] private float transitionDelay = 1.5f; // Thời gian chờ trước khi chuyển scene
    [SerializeField] private Transform player; // Transform của Player
    [SerializeField] private float activationDistance = 2.0f; // Khoảng cách để bắt đầu hút

    private bool _isTriggered = false; // Tránh kích hoạt nhiều lần
    private static readonly int Effect = Animator.StringToHash("StartEffect");

    private void Update()
    {
        if (_isTriggered) return;

        // Kiểm tra khoảng cách giữa Player và TV
        if (!(Vector3.Distance(transform.position, player.position) <= activationDistance)) return;
        _isTriggered = true; // Đánh dấu đã kích hoạt để tránh lặp lại
        StartEffect();
    }

    private void StartEffect()
    {
        // Kích hoạt hiệu ứng thu nhỏ
        if (playerAnimator)
        {
            playerAnimator.SetTrigger(Effect);
        }

        // Chuyển sang scene boss sau khi hiệu ứng kết thúc
        Invoke(nameof(LoadBossScene), transitionDelay);
    }

    private void LoadBossScene()
    {
        SceneManager.LoadScene(bossSceneName);
    }
}