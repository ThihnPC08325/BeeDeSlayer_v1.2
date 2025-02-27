using UnityEngine;

public class ResetPlayerPosition : MonoBehaviour
{
    public Transform player;  // Tham chiếu đến người chơi
    private Vector3 initialPosition; // Lưu vị trí ban đầu

    private void Start()
    {
        if (player != null)
        {
            initialPosition = player.position; // Lưu vị trí ban đầu của người chơi
        }
        else
        {
            Debug.LogError("Chưa gán đối tượng người chơi!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == player)
        {
            ResetPosition();
        }
    }

    private void ResetPosition()
    {
        CharacterController controller = player.GetComponent<CharacterController>();

        if (controller != null)
        {
            controller.enabled = false; // Tắt CharacterController để đặt vị trí mới
            player.position = initialPosition;
            controller.enabled = true; // Bật lại CharacterController
        }
        else
        {
            player.position = initialPosition;
        }
    }
}
