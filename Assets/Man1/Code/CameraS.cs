using System.Collections;
using UnityEngine;

public class CameraS : MonoBehaviour
{
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition; // Lưu vị trí ban đầu của camera
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Tạo vị trí ngẫu nhiên trong một bán kính nhỏ
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            // Cập nhật vị trí của camera
            transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;

            yield return null; // Đợi frame tiếp theo
        }

        // Trả về vị trí ban đầu của camera
        transform.localPosition = originalPosition;
    }
}
