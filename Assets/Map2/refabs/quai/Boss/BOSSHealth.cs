using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Thêm thư viện để chuyển Scene

public class BOSSHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private GameObject smokePrefab;
    [SerializeField] private Slider bossHealthBar;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private GameObject exitZone; // Vùng chuyển scene
    [SerializeField] private GameObject meteorPrefab;
    [SerializeField] private float meteorDamage = 15f;
    [SerializeField] private float dotDamage = 5f;
    [SerializeField] private int dotTicks = 3;
    [SerializeField] private float dotInterval = 1f;
    [SerializeField] private float meteorSpawnDelay = 3f;


    private AudioSource audioSource;
    private AudioSource backgroundAudioSource;
    private BoxCollider boxCollider;
    private bool isVictoryMusicPlaying = false;
    private bool triggered70 = false;
    private bool triggered30 = false;
        
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        audioSource = gameObject.AddComponent<AudioSource>();
        backgroundAudioSource = gameObject.AddComponent<AudioSource>();

        backgroundAudioSource.loop = true;
        backgroundAudioSource.volume = 0.5f;

        currentHealth = maxHealth;

        if (exitZone != null)
        {
            exitZone.SetActive(false); // Ẩn vùng chuyển scene ban đầu
        }
    }

    void Start()
    {
        currentHealth = maxHealth;

        if (bossHealthBar != null)
        {
            bossHealthBar.maxValue = maxHealth;
            bossHealthBar.value = currentHealth;
        }

        if (backgroundMusic != null)
        {
            backgroundAudioSource.clip = backgroundMusic;
            backgroundAudioSource.Play();
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            boxCollider.enabled = false;
            Die();
        }
        if (bossHealthBar != null)
        {
            bossHealthBar.value = currentHealth;
        }
        float healthPercent = (currentHealth / maxHealth) * 100;
        Debug.Log($"Boss HP: {currentHealth} ({healthPercent}%)");

        if (!triggered70 && healthPercent <= 70)
        {
            Debug.Log("Gọi mưa thiên thạch 70% HP!");
            StartCoroutine(TriggerMeteorRain());
            triggered70 = true;
        }
        else if (!triggered30 && healthPercent <= 30)
        {
            Debug.Log("Gọi mưa thiên thạch 30% HP!");
            StartCoroutine(TriggerMeteorRain());
            triggered30 = true;
        }
    }
    public void Activate(float meteorDamage, float dotDamage, int dotTicks, float dotInterval)
    {
        Debug.Log($"Thiên thạch gây {meteorDamage} sát thương tức thì và {dotDamage} mỗi giây trong {dotTicks} giây.");
        // Viết logic gây sát thương ở đây
    }


    private void Die()
    {
        if (backgroundAudioSource.isPlaying)
        {
            backgroundAudioSource.Stop();
        }

        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        if (smokePrefab != null)
        {
            Instantiate(smokePrefab, transform.position, Quaternion.identity);
        }

        StartCoroutine(TimeToDie(2f));
    }

    private IEnumerator TriggerMeteor()
    {
        Debug.Log("🌩️ Bắt đầu gọi thiên thạch!");

        Vector3 spawnPosition = GetRandomPosition();
        GameObject meteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);

        if (meteor == null)
        {
            Debug.LogError("⚠️ Không thể tạo thiên thạch!");
            yield break;
        }

        Debug.Log("✅ Thiên thạch đã spawn tại " + spawnPosition);

        Meteor meteorScript = meteor.GetComponent<Meteor>();
        if (meteorScript != null)
        {
            meteorScript.Activate(meteorDamage, dotDamage, dotTicks, dotInterval);
            Debug.Log("🔥 Đã gọi Activate trên Meteor!");
        }
        else
        {
            Debug.LogError("⚠️ Không tìm thấy script Meteor trên Prefab!");
        }
    }
    private IEnumerator TriggerMeteorRain()
    {
        Debug.Log("Bắt đầu triệu hồi thiên thạch!");

        for (int i = 0; i < 5; i++) // 5 thiên thạch
        {
            Vector3 fixedMeteorPosition = new Vector3(47.96927f, 36.1261f, 71.4544f);
            GameObject meteor = Instantiate(meteorPrefab, fixedMeteorPosition, Quaternion.identity);
            Debug.Log("Meteor Spawned at " + fixedMeteorPosition);
            Meteor meteorScript = meteor.GetComponent<Meteor>();
            if (meteorScript != null)
            {

                meteorScript.Activate(meteorDamage, dotDamage, dotTicks, dotInterval);
                Debug.Log("Gọi Activate trên Meteor!");
            }
            else
            {
                Debug.LogError("Prefab Meteor không có script!");
            }

            yield return new WaitForSeconds(1.5f);
        }
    }
    private Vector3 GetRandomPosition()
    {
        float randomX = Random.Range(-5f, 5f);
        float randomZ = Random.Range(-5f, 5f);
        return new Vector3(randomX, 0, randomZ);
    }
    private IEnumerator TimeToDie(float duration)
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false); // Ẩn Boss

        if (exitZone != null)
        {
            exitZone.SetActive(true); // Hiện vùng chuyển scene
        }
    }
}
