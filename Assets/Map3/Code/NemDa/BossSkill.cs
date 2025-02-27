﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class BossSkill : MonoBehaviour
{
    [SerializeField] public List<GameObject> rockPrefabs; // Danh sách prefab của các viên đá
    [SerializeField] public Transform throwPoint; // Vị trí xuất phát của viên đá
    [SerializeField] private float throwForce = 100f; // Lực ném đá
    [SerializeField] private float throwInterval = 5f; // Thời gian giữa các lần ném
    [SerializeField] private float delayBeforeThrow = 7f; // Thời gian trì hoãn trước khi bắt đầu ném đá
    [SerializeField] private Animator animator; // Animator của boss 
    private float _timeReloadThrow;
    private Transform _player;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null) return;
        _player = playerObject.transform;
        StartCoroutine(WaitAndThrowRock());

        RuntimeAnimatorController controller = animator.runtimeAnimatorController;
        AnimationClip[] clips = controller.animationClips;
        foreach (AnimationClip clip in clips)
        {
            _timeReloadThrow = clip.length;
        }
    }

    // Coroutine trì hoãn việc ném đá
    private IEnumerator WaitAndThrowRock()
    {
        yield return new WaitForSeconds(delayBeforeThrow);
        StartCoroutine(ThrowRockRoutine());
    }

    IEnumerator ThrowRockRoutine()
    {
        while (true)
        {
            StartCoroutine(timeToThrow());
            yield return new WaitForSeconds(throwInterval);
        }
    }

    private IEnumerator timeToThrow()
    {
        if (rockPrefabs.Count <= 0 || !_player || !throwPoint) yield break;
        // Lấy ngẫu nhiên một prefab từ danh sách
        GameObject selectedRockPrefab = rockPrefabs[Random.Range(0, rockPrefabs.Count)];

        animator.Play("ThrowRock");

        yield return new WaitForSeconds(_timeReloadThrow -= 1.85f);
        // Tạo viên đá
        GameObject rock = Instantiate(selectedRockPrefab, throwPoint.position, throwPoint.rotation);
            
        // Gán mục tiêu cho viên đá nếu có script RockBehavior
        RockBehavior rockBehavior = rock.GetComponent<RockBehavior>();
        if (rockBehavior)
        {
            rockBehavior.target = _player;
        }
    }
}
