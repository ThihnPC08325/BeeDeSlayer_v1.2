using UnityEngine;

public class LightGrow : MonoBehaviour
{
    // Tham chiếu đến Point Light (nếu chưa gán, sẽ lấy từ component Light trên GameObject)
    public Light pointLight;

    // Giá trị ban đầu và mục tiêu của intensity
    [SerializeField] private float startintensity = 0f;
    [SerializeField] private float endintensity = 90000f;

    // Thời gian (giây) để chuyển từ startintensity đến endintensity
    public float duration = 5f;

    // Thời gian đã trôi qua kể từ khi bắt đầu hiệu ứng
    private float elapsedTime = 0f;

    void Start()
    {
        // Đặt giá trị intensity ban đầu
        pointLight.intensity = startintensity;
        elapsedTime = 0f;
    }

    void Update()
    {
        // Nếu thời gian trôi qua chưa đủ duration, tăng dần intensity
        if (elapsedTime < duration)
        {
            // Cộng thêm thời gian đã trôi qua
            elapsedTime += Time.deltaTime;

            // Tính tỉ lệ thời gian (0 đến 1)
            float t = Mathf.Clamp01(elapsedTime / duration);

            // Sử dụng SmoothStep để chuyển đổi mượt mà
            t = Mathf.SmoothStep(0f, 1f, t);

            // Lerp giá trị range từ startintensity đến endintensity dựa trên t
            pointLight.intensity = Mathf.Lerp(startintensity, endintensity, t);
        }
    }
}
