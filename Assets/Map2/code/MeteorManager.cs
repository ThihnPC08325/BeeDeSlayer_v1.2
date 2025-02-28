using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorManager : MonoBehaviour
{
    [SerializeField] private GameObject meteorPrefab; // Prefab thiên thạch
    [SerializeField] private Vector3 spawnPosition = new Vector3(47.96927f, 36.1261f, 71.4544f); // Vị trí spawn cố định
    [SerializeField] private float spawnDelay = 100f; // Delay xuất hiện

    [Header("Damage Settings")]
    [SerializeField] private float meteorDamage = 50f;
    [SerializeField] private float dotDamage = 5f;
    [SerializeField] private int dotTicks = 3;
    [SerializeField] private float dotInterval = 1f;

    void Start()
    {
        StartCoroutine(SpawnMeteorWithDelay());
    }

    private IEnumerator SpawnMeteorWithDelay()
    {
        yield return new WaitForSeconds(spawnDelay); // Đợi 10 giây

        GameObject meteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);
        Meteor meteorScript = meteor.GetComponent<Meteor>();

        if (meteorScript != null)
        {
            meteorScript.Activate(meteorDamage, dotDamage, dotTicks, dotInterval);
        }

        Debug.Log($"☄️ Thiên thạch xuất hiện sau {spawnDelay} giây tại {spawnPosition}");
    }
}

