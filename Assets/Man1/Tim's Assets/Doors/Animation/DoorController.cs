using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Animator doorAnimator; // Kéo thả Animator của cửa vào đây trong Unity
    public GameObject doorEffectPrefab; // Prefab của hiệu ứng cửa mở
    public Transform effectSpawnPosition; // Vị trí phát sinh hiệu ứng
    private bool isPlayerNearby = false; // Kiểm tra xem người chơi có ở gần cửa không
    private bool isDoorOpen = false; // Kiểm tra cửa đã mở hay chưa
    private GameObject effectInstance; // Instance của hiệu ứng cửa

    void Start()
    {
        // Ẩn hiệu ứng khi cửa chưa mở
        if (doorEffectPrefab != null && effectSpawnPosition != null)
        {
            effectInstance = Instantiate(doorEffectPrefab, effectSpawnPosition.position, effectSpawnPosition.rotation);
            effectInstance.SetActive(false); // Ẩn hiệu ứng ngay từ đầu
        }
    }

    void Update()
    {
        // Kiểm tra xem người chơi có nhấn phím F khi đang ở gần cửa và cửa chưa mở
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F) && !isDoorOpen)
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        // Kích hoạt animation mở cửa
        doorAnimator.SetTrigger("OpenDoor");

        // Đặt trạng thái cửa là đã mở
        isDoorOpen = true;

        // Tạo và kích hoạt hiệu ứng cửa mở
        if (effectInstance != null)
        {
            effectInstance.SetActive(true); // Kích hoạt hiệu ứng khi cửa mở
        }
    }

    private void CloseDoor()
    {
        // Kích hoạt animation đóng cửa (nếu có)
        doorAnimator.SetTrigger("CloseDoor");

        // Đặt trạng thái cửa là đã đóng
        isDoorOpen = false;

        // Ẩn hiệu ứng cửa khi cửa đóng
        if (effectInstance != null)
        {
            effectInstance.SetActive(false); // Ẩn hiệu ứng khi cửa đóng
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem đối tượng tiếp xúc có phải là người chơi
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Người chơi rời khỏi vùng cửa
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }
}
