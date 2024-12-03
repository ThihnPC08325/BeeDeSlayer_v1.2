using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossVoiceController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> allVoiceLines;
    private List<AudioClip> remainingVoiceLines;
    private bool isPlaying = false;

    [Header("Voice Frequency Settings")]
    [SerializeField] private float minTimeBetweenLines = 8f;  // Thời gian tối thiểu giữa các lines
    [SerializeField] private float maxTimeBetweenLines = 15f; // Thời gian tối đa giữa các lines
    [SerializeField] private bool autoPlayVoiceLines = true;  // Tự động phát theo thời gian

    private float nextVoiceTime;

    private void Start()
    {
        remainingVoiceLines = new List<AudioClip>(allVoiceLines);
        SetNextVoiceTime();

        if (autoPlayVoiceLines)
        {
            StartCoroutine(AutoPlayVoiceLines());
        }
    }

    private void SetNextVoiceTime()
    {
        nextVoiceTime = Time.time + Random.Range(minTimeBetweenLines, maxTimeBetweenLines);
    }

    private IEnumerator AutoPlayVoiceLines()
    {
        while (true)
        {
            if (Time.time >= nextVoiceTime)
            {
                PlayRandomVoiceLine();
                SetNextVoiceTime();
            }
            yield return new WaitForSeconds(0.5f); // Kiểm tra mỗi 0.5 giây
        }
    }

    public void PlayRandomVoiceLine()
    {
        if (remainingVoiceLines.Count == 0 || isPlaying) return;

        int randomIndex = Random.Range(0, remainingVoiceLines.Count);
        AudioClip selectedClip = remainingVoiceLines[randomIndex];

        audioSource.clip = selectedClip;
        audioSource.Play();
        isPlaying = true;

        remainingVoiceLines.RemoveAt(randomIndex);
        StartCoroutine(ResetPlayingFlag(selectedClip.length));
    }

    private IEnumerator ResetPlayingFlag(float delay)
    {
        yield return new WaitForSeconds(delay);
        isPlaying = false;
    }

    // Thêm function để dừng auto play nếu cần
    public void StopAutoPlay()
    {
        autoPlayVoiceLines = false;
        StopAllCoroutines();
    }
}
