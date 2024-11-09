using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    [Header("Laser Settings")]
    [SerializeField] float laserWidth = 0.2f;
    [SerializeField] float maxLength = 100f;
    [SerializeField] float damagePerSecond = 30f;
    [SerializeField] float penetration = 0;
    [SerializeField] Material laserMaterial;
    [SerializeField] Color laserColor = Color.red;

    [Header("References")]
    [SerializeField] Transform laserOrigin;
    [SerializeField] ParticleSystem startVFX;
    [SerializeField] ParticleSystem endVFX;

    private LineRenderer lineRenderer;
    private bool isActive = false;

    private void Awake()
    {
        // Thiết lập LineRenderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = laserWidth;
        lineRenderer.endWidth = laserWidth;
        lineRenderer.material = laserMaterial;
        lineRenderer.startColor = laserColor;
        lineRenderer.endColor = laserColor;
        lineRenderer.enabled = false;
    }

    public void StartLaser()
    {
        isActive = true;
        lineRenderer.enabled = true;
        if (startVFX) startVFX.Play();
        if (endVFX) endVFX.Play();
    }

    public void StopLaser()
    {
        isActive = false;
        lineRenderer.enabled = false;
        if (startVFX) startVFX.Stop();
        if (endVFX) endVFX.Stop();
    }

    private void Update()
    {
        if (!isActive) return;

        RaycastHit hit;
        Vector3 endPosition;

        if (Physics.Raycast(laserOrigin.position, laserOrigin.forward, out hit, maxLength))
        {
            endPosition = hit.point;
            if (endVFX) endVFX.transform.position = endPosition;

            // Gây sát thương nếu trúng player
            if (hit.collider.CompareTag("Player"))
            {
                GameEvents.TriggerPlayerHit(damagePerSecond * Time.deltaTime, penetration);
            }
        }
        else
        {
            endPosition = laserOrigin.position + laserOrigin.forward * maxLength;
            if (endVFX) endVFX.transform.position = endPosition;
        }

        // Cập nhật vị trí laser
        lineRenderer.SetPosition(0, laserOrigin.position);
        lineRenderer.SetPosition(1, endPosition);
    }
}
