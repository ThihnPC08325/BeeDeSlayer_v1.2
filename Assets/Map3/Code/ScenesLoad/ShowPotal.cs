using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPotal : MonoBehaviour
{
    [SerializeField] private float targetY = 5f; // Tọa độ Y muốn dừng lại
    [SerializeField] private float speed = 2f;  // Tốc độ di chuyển

    private void Start()
    {
        StartCoroutine(RiseUp());
    }

    private IEnumerator RiseUp()
    {
        while (transform.position.y < targetY)
        {
            // Di chuyển object lên trên theo thời gian
            transform.position += Vector3.up * speed * Time.deltaTime;
            yield return null;
        }

        // Đảm bảo object dừng chính xác ở targetY
        transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
    }
}
