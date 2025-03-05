using System.Collections;
using UnityEngine;

public class Blaze : MonoBehaviour
{
    [SerializeField] private Transform leftEye, rightEye; // Biến tham chiếu đến mắt trái và mắt phải của boss
    [SerializeField] private Light leftEyeLight, rightEyeLight; // Biến tham chiếu đến đèn sáng của mắt trái và phải của boss
    [SerializeField] private LineRenderer leftLaser, rightLaser; // Biến tham chiếu đến LineRenderer của laser trái và phải

    [SerializeField] private float gunRange = 800f; // Tầm bắn tối đa của súng laser
    [SerializeField] private float fireRate = 50f; // Tốc độ bắn (số lần bắn trong một giây)
    [SerializeField] private float chargeTime = 3f; // Thời gian cần thiết để tụ lực (chuẩn bị bắn laser)
    [SerializeField] private float laserDuration = 20f; // Thời gian tồn tại của laser sau khi bắn
    [SerializeField] private float laserSpeed = 150f; // Tốc độ di chuyển của laser
    [SerializeField] private float laserWidth = 5.0f; // Độ rộng của tia laser

    [SerializeField] private string targetTag = "Player"; // Tag của mục tiêu (mục tiêu là người chơi sẽ có tag này)
    [SerializeField] private AudioClip chargeSound, shootSound; // Âm thanh phát ra khi tụ lực và khi bắn laser
    [SerializeField] private int laserDamage = 10; // Sát thương của laser

    private AudioSource audioSource; // Đối tượng AudioSource để phát âm thanh
    private bool isAttacking = false;
    private float attackTimer = 0f;

    private Transform playerTransform; // Thêm biến để tham chiếu tới người chơi

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        leftLaser.enabled = false;
        rightLaser.enabled = false;
        leftEyeLight.intensity = 0;
        rightEyeLight.intensity = 0;

        // Tìm người chơi trong scene
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (!isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= fireRate)
            {
                attackTimer = 0;
                StartCoroutine(ChargeAndFire());
            }
        }
    }

    IEnumerator ChargeAndFire()
    {
        isAttacking = true;
        Debug.Log("Boss đang tụ lực...");

        leftEyeLight.color = Color.red;
        rightEyeLight.color = Color.red;
        leftEyeLight.intensity = 5f;
        rightEyeLight.intensity = 5f;

        audioSource.PlayOneShot(chargeSound);

        yield return new WaitForSeconds(chargeTime);

        if (playerTransform != null) // Kiểm tra xem người chơi có tồn tại không
        {
            // Điều chỉnh độ rộng laser trước khi bắn
            leftLaser.startWidth = laserWidth;
            leftLaser.endWidth = laserWidth;
            rightLaser.startWidth = laserWidth;
            rightLaser.endWidth = laserWidth;

            // Bắn laser tới vị trí của người chơi
            StartCoroutine(FireLaser(leftEye, leftLaser, playerTransform.position));
            StartCoroutine(FireLaser(rightEye, rightLaser, playerTransform.position));
        }

        // Nếu không tìm thấy người chơi, bạn có thể xử lý ở đây
        else
        {
            Debug.LogWarning("Player not found!");
        }
    }

    IEnumerator FireLaser(Transform eye, LineRenderer laser, Vector3 targetPosition)
    {
        Debug.Log("Boss bắn laser từ " + eye.name);
        audioSource.PlayOneShot(shootSound);

        laser.enabled = true;
        Vector3 startPoint = eye.position;
        Vector3 endPoint = targetPosition;
        float elapsedTime = 0;
        float travelTime = Vector3.Distance(startPoint, endPoint) / laserSpeed;

        // Laser raycasting to deal damage
        RaycastHit hit;

        while (elapsedTime < travelTime)
        {
            elapsedTime += Time.deltaTime;
            Vector3 currentPosition = Vector3.Lerp(startPoint, endPoint, elapsedTime / travelTime);
            laser.SetPosition(0, eye.position);
            laser.SetPosition(1, currentPosition);

            // Raycast to detect collision and apply damage if the target is the player
            if (Physics.Raycast(eye.position, currentPosition - eye.position, out hit, laserSpeed))
            {
                if (hit.collider.CompareTag(targetTag))
                {
                    // Call the TakeDamage method from PlayerHealth to apply damage
                    hit.collider.GetComponent<PlayerHealth>()?.TakeDamage(laserDamage, 0);

                    // Change the color of the player when hit by the laser (you can customize this part)
                    hit.collider.GetComponent<Renderer>()?.material.SetColor("_Color", Color.red);
                }
            }

            yield return null;
        }

        // End laser and reset lights
        yield return new WaitForSeconds(laserDuration);

        laser.enabled = false;
        leftEyeLight.intensity = 0;
        rightEyeLight.intensity = 0;

        yield return new WaitForSeconds(fireRate - laserDuration - chargeTime);
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, gunRange);
    }
}
