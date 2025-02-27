using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Import để chuyển scene

public class BossManager : MonoBehaviour
{
    [SerializeField] private GameObject bossPhase1;  // Boss Phase 1
    [SerializeField] private GameObject bossPhase2;  // Boss Phase 2
    [SerializeField] private GameObject fakePortal;  // Cổng giả
    [SerializeField] private GameObject realPortal;  // Cổng thật
    [SerializeField] private ParticleSystem fakePortalParticles;
    [SerializeField] private ParticleSystem realPortalParticles;
    [SerializeField] private float delay = 10f;      // Thời gian chờ trước khi xuất hiện Boss Phase 2
    [SerializeField] private string nextSceneName;   // Tên Scene tiếp theo

    private bool hasTriggeredPhase2 = false;
    private bool hasTriggeredRealPortal = false;
    private bool hasBossPhase2Spawned = false;  // Check xem Boss Phase 2 đã từng xuất hiện chưa

    void Start()
    {
        if (fakePortal) fakePortal.SetActive(false);
        if (realPortal) realPortal.SetActive(false);
        if (bossPhase2) bossPhase2.SetActive(false);
        if (fakePortalParticles) fakePortalParticles.Stop();
        if (realPortalParticles) realPortalParticles.Stop();
    }

    void Update()
    {
        // Khi Boss Phase 1 chết -> Hiện cổng giả
        if (!hasTriggeredPhase2 && bossPhase1 && !bossPhase1.activeInHierarchy)
        {
            hasTriggeredPhase2 = true;
            if (fakePortal) fakePortal.SetActive(true);
            if (fakePortalParticles) fakePortalParticles.Play();
            StartCoroutine(HandlePhase2Spawn());
        }

        // Khi Boss Phase 2 xuất hiện, đánh dấu là đã spawn
        if (bossPhase2.activeInHierarchy)
        {
            hasBossPhase2Spawned = true;
        }

        // Khi Boss Phase 2 chết (và đã từng xuất hiện) -> Hiện cổng thật
        if (hasBossPhase2Spawned && !hasTriggeredRealPortal && bossPhase2 && !bossPhase2.activeInHierarchy)
        {
            hasTriggeredRealPortal = true;
            if (realPortal) realPortal.SetActive(true);
            if (realPortalParticles) realPortalParticles.Play();
        }
    }

    private IEnumerator HandlePhase2Spawn()
    {
        yield return new WaitForSeconds(delay);

        // Xóa cổng giả, xuất hiện Boss Phase 2
        if (fakePortal) fakePortal.SetActive(false);
        if (fakePortalParticles) fakePortalParticles.Stop();
        if (bossPhase2) bossPhase2.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && realPortal.activeInHierarchy)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
