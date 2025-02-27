using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Import để chuyển scene

public class BossManager : MonoBehaviour
{
    [Header("Boss Settings")] [SerializeField]
    private GameObject bossPhase1; // Boss Phase 1

    [SerializeField] private GameObject bossPhase2; // Boss Phase 2

    [Header("Portal Settings")] [SerializeField]
    private GameObject fakePortal; // Cổng giả

    [SerializeField] private GameObject realPortal; // Cổng thật
    [SerializeField] private ParticleSystem fakePortalParticles;
    [SerializeField] private ParticleSystem realPortalParticles;

    [Header("Timing Settings")] [SerializeField]
    private float delay = 10f; // Thời gian chờ trước khi hiện Boss Phase 2

    [SerializeField] private string nextSceneName; // Tên Scene mới khi qua cổng thật

    private bool _hasTriggeredPhase2 = false;
    private bool _hasTriggeredRealPortal = false;
    private bool _hasBossPhase2Spawned = false; // Check xem Boss Phase 2 đã từng xuất hiện chưa


    private void Start()
    {
        // Vô hiệu hóa tất cả các đối tượng cần thiết ban đầu
        SetGameObjectState(fakePortal, false);
        SetGameObjectState(realPortal, false);
        SetGameObjectState(bossPhase2, false);
        StopParticleSystem(fakePortalParticles);
        StopParticleSystem(realPortalParticles);
    }

    private void Update()
    {
        // Khi Boss Phase 1 chết -> Hiện cổng giả
        if (!_hasTriggeredPhase2 && bossPhase1 && !bossPhase1.activeInHierarchy)
        {
            _hasTriggeredPhase2 = true;
            if (fakePortal) fakePortal.SetActive(true);
            if (fakePortalParticles) fakePortalParticles.Play();
            StartCoroutine(HandlePhase2Spawn());
        }

        // Khi Boss Phase 2 xuất hiện, đánh dấu là đã spawn
        if (bossPhase2.activeInHierarchy)
        {
            _hasBossPhase2Spawned = true;
        }

        // Khi Boss Phase 2 chết (và đã từng xuất hiện) -> Hiện cổng thật
        if (_hasBossPhase2Spawned && !_hasTriggeredRealPortal && bossPhase2 && !bossPhase2.activeInHierarchy)
        {
            _hasTriggeredRealPortal = true;
            if (realPortal) realPortal.SetActive(true);
            if (realPortalParticles) realPortalParticles.Play();
        }
    }

    private IEnumerator HandlePhase2Spawn()
    {
        yield return new WaitForSeconds(delay);

        // Xóa cổng giả, chuyển qua xuất hiện Boss Phase 2
        DeactivateFakePortal();
        SetGameObjectState(bossPhase2, true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && realPortal.activeInHierarchy)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void DeactivateFakePortal()
    {
        SetGameObjectState(fakePortal, false);
        StopParticleSystem(fakePortalParticles);
    }

    private static void SetGameObjectState(GameObject obj, bool isActive)
    {
        if (obj)
        {
            obj.SetActive(isActive);
        }
    }

    private static void StopParticleSystem(ParticleSystem particleSystem)
    {
        if (particleSystem)
        {
            particleSystem.Stop();
        }
    }
}