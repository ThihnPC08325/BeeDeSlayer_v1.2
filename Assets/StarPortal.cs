
using System.Collections;
using UnityEngine;

public class Por : MonoBehaviour
{
    public Transform player, destination;
    public GameObject players; // Nhân vật hoặc đối tượng cần dịch chuyển
    public float delayTime = 2.0f;
    public AudioClip teleportSound;
    private AudioSource audioSource;

    private void Start()
    {
        // Kiểm tra và gán AudioSource nếu không có sẵn
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(TeleportPlayer());
        }
    }



    private IEnumerator TeleportPlayer()
    {
        // Phát âm thanh dịch chuyển
        if (teleportSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(teleportSound);
        }


        yield return new WaitForSeconds(delayTime);

        players.SetActive(false);
        player.position = destination.position;
        players.SetActive(true);
    }
}

