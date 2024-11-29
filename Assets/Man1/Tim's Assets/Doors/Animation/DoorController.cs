using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Animator doorAnimator; // Kéo thả Animator của cửa vào đây trong Unity
    private bool isPlayerNearby = false; // Kiểm tra xem người chơi có ở gần cửa không

    void Update()
    {
        // Kiểm tra xem người chơi có nhấn phím E khi đang ở gần cửa
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        // Kích hoạt animation mở cửa
        doorAnimator.SetTrigger("OpenDoor");
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
