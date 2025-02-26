using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    [SerializeField] private GameObject targetObject; // Object cần kiểm tra
    [SerializeField] private GameObject assignedObject; // Object được chỉ định hiển thị
    [SerializeField] private float delay = 5f; // Thời gian chờ sau khi Object biến mất

    private bool _hasTriggered = false; // Để đảm bảo chỉ kích hoạt 1 lần

    void Update()
    {
        // Kiểm tra nếu targetObject không tồn tại hoặc bị vô hiệu, và chưa kích hoạt
        if (!_hasTriggered && (!targetObject || !targetObject.activeInHierarchy))
        {
            _hasTriggered = true; // Đánh dấu là đã kích hoạt
            StartCoroutine(ShowAssignedObjectAfterDelay());
        }
    }

    private IEnumerator ShowAssignedObjectAfterDelay()
    {
        // Đợi thời gian chỉ định
        yield return new WaitForSeconds(delay);

        // Hiển thị Object được chỉ định
        if (assignedObject)
        {
            assignedObject.SetActive(true);
        }
    }
}
