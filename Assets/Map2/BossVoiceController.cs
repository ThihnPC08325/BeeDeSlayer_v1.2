using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossVoiceController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> allVoiceLines;
    
    private List<AudioClip> _remainingVoiceLines;
    private bool _isPlaying = false;

    [Header("Voice Frequency Settings")]
    [SerializeField] private float minTimeBetweenLines = 8f;  // Thời gian tối thiểu giữa các lines
    [SerializeField] private float maxTimeBetweenLines = 15f; // Thời gian tối đa giữa các lines
    [SerializeField] private bool autoPlayVoiceLines = true;  // Tự động phát theo thời gian

    private float _nextVoiceTime;

    private void Start()
    {
        _remainingVoiceLines = new List<AudioClip>(allVoiceLines);
        SetNextVoiceTime();

        if (autoPlayVoiceLines)
        {
            StartCoroutine(AutoPlayVoiceLines());
        }
    }

    private void SetNextVoiceTime()
    {
        _nextVoiceTime = Time.time + Random.Range(minTimeBetweenLines, maxTimeBetweenLines);
    }

    private IEnumerator AutoPlayVoiceLines()
    {
        while (true)
        {
            if (Time.time >= _nextVoiceTime)
            {
                PlayRandomVoiceLine();
                SetNextVoiceTime();
            }
            yield return new WaitForSeconds(0.5f); // Kiểm tra mỗi 0.5 giây
        }
    }

    private void PlayRandomVoiceLine()
    {
        if (_remainingVoiceLines.Count == 0 || _isPlaying) return;

        int randomIndex = Random.Range(0, _remainingVoiceLines.Count);
        AudioClip selectedClip = _remainingVoiceLines[randomIndex];

        audioSource.clip = selectedClip;
        audioSource.Play();
        _isPlaying = true;

        _remainingVoiceLines.RemoveAt(randomIndex);
        StartCoroutine(ResetPlayingFlag(selectedClip.length));
    }

    private IEnumerator ResetPlayingFlag(float delay)
    {
        yield return new WaitForSeconds(delay);
        _isPlaying = false;
    }
}
