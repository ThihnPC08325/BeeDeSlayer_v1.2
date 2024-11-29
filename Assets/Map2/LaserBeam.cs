using UnityEngine;

public class LaserFire : MonoBehaviour
{
    public Transform player; // Tham chiếu đến Transform của người chơi
    [SerializeField] private ParticleSystem laserParticle; // Particle System để bắn laser
    [SerializeField] private float activeDuration = 3f; // Thời gian tồn tại của particle (giây)
    [SerializeField] private float cooldownDuration = 5f; // Thời gian chờ trước khi tái kích hoạt (giây)
    private bool isFiring = false;

    void Start()
    {
        // Kiểm tra xem Particle System có được gán hay không
        if (laserParticle == null)
        {
            Debug.LogError("Không tìm thấy Particle System trên GameObject.");
        }

        // Kiểm tra xem Transform của người chơi có được gán hay không
        if (player == null)
        {
            Debug.LogError("Chưa gán Transform của người chơi.");
        }

        // Bắt đầu kích hoạt Particle System lần đầu
        StartFiring();
    }

    void Update()
    {
        if (player != null && laserParticle != null && isFiring)
        {
            // Tính toán vector hướng đến người chơi
            Vector3 directionToPlayer = player.position - transform.position;

            // Tính toán góc quay để hướng về phía người chơi
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);

            // Áp dụng hướng mới cho GameObject chứa Particle System
            transform.rotation = lookRotation;
        }
    }

    void StartFiring()
    {
        // Kích hoạt Particle System
        laserParticle.Play();
        isFiring = true;

        // Dừng Particle System sau activeDuration
        Invoke(nameof(StopFiring), activeDuration);
    }

    void StopFiring()
    {
        // Dừng Particle System
        laserParticle.Stop();
        isFiring = false;

        // Tái kích hoạt Particle System sau cooldownDuration
        Invoke(nameof(StartFiring), cooldownDuration);
    }
}
