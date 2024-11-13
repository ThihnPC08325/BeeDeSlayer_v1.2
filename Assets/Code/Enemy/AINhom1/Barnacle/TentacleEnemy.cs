using System;
using UnityEngine;

public class TentacleEnemy : MonoBehaviour
{
    public enum EnemyState { Idle, Extending, Catch, Retract }
    private EnemyState currentState = EnemyState.Idle;

    [SerializeField] private float revealRadius = 5f; // Horizontal range
    [SerializeField] private float revealHeight = 10f; // Vertical range
    [SerializeField] private float catchRange = 1f;
    [SerializeField] private float catchDamage = 10f;
    [SerializeField] private float catchPen = 0f;
    [SerializeField] private float totalCatchDamage = 40f;
    [SerializeField] private float damageInterval = 1f;
    [SerializeField] private float extensionSpeed = 2f;
    [SerializeField] private float maxExtensionDistance = 5f;
    [SerializeField] private float cooldownTime = 3f; // Cooldown duration after retraction
    [SerializeField] private Transform tentacleHead;
    [SerializeField] private Transform tentacleStart;

    private Transform player;
    private PlayerController playerMovement;
    private float currentDamage = 0f;
    private float damageTimer;
    private float cooldownTimer = 0f;
    private Vector3 originalPosition;
    private bool isCaught;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerMovement = player.GetComponent<PlayerController>();
        originalPosition = tentacleHead.position;
        damageTimer = damageInterval;
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                if (cooldownTimer > 0f)
                {
                    cooldownTimer -= Time.deltaTime;
                }
                else if (IsPlayerWithinRevealRange())
                {
                    currentState = EnemyState.Extending;
                }
                break;

            case EnemyState.Extending:
                ExtendTentacle();
                break;

            case EnemyState.Catch:
                CatchPlayer();
                break;

            case EnemyState.Retract:
                RetractTentacle();
                break;
        }
    }

    bool IsPlayerWithinRevealRange()
    {
        // Check horizontal distance (ignores Y-axis for radius check)
        float horizontalDistance = Vector3.Distance(
            new Vector3(tentacleStart.position.x, 0, tentacleStart.position.z),
            new Vector3(player.position.x, 0, player.position.z)
        );

        // Check vertical distance separately
        float verticalDistance = Mathf.Abs(tentacleStart.position.y - player.position.y);

        // Check if within both radius and height limits
        return horizontalDistance <= revealRadius && verticalDistance <= revealHeight;
    }

    void ExtendTentacle()
    {
        // Move the tentacle head directly downward
        tentacleHead.position += Vector3.down * extensionSpeed * Time.deltaTime;

        float currentExtensionDistance = Mathf.Abs(tentacleStart.position.y - tentacleHead.position.y);

        if (Vector3.Distance(tentacleHead.position, player.position) <= catchRange && !isCaught)
        {
            currentState = EnemyState.Catch;
            playerMovement.enabled = false;
            isCaught = true;
        }
        else if (currentExtensionDistance >= maxExtensionDistance)
        {
            currentState = EnemyState.Retract;
        }
    }

    public void CatchPlayer()
    {
        Vector3 newTentacleHeadPos = Vector3.MoveTowards(tentacleHead.position, tentacleStart.position, extensionSpeed * Time.deltaTime);
        Vector3 offset = tentacleHead.position - newTentacleHeadPos;

        player.position -= offset;
        tentacleHead.position = newTentacleHeadPos;

        if (currentDamage >= totalCatchDamage)
        {
            ReleasePlayer();
            return;
        }

        damageTimer -= Time.deltaTime;
        if (damageTimer <= 0f)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(catchDamage,catchPen);
            currentDamage += catchDamage;
            damageTimer = damageInterval;
        }
    }

    void RetractTentacle()
    {
        tentacleHead.position = Vector3.MoveTowards(tentacleHead.position, originalPosition, extensionSpeed * Time.deltaTime);

        if (Vector3.Distance(tentacleHead.position, originalPosition) <= 0.1f)
        {
            currentState = EnemyState.Idle;
            isCaught = false;
            currentDamage = 0f;
            cooldownTimer = cooldownTime;
        }
    }

    public void OnTentacleHeadHit()
    {
        if (currentState == EnemyState.Extending || currentState == EnemyState.Catch)
        {
            currentState = EnemyState.Retract;
        }
    }

    private void ReleasePlayer()
    {
        Debug.Log("Player released from trap!");
        playerMovement.enabled = true;
        currentState = EnemyState.Retract;
    }
    void OnDrawGizmosSelected()
    {
        // Set the gizmo color
        Gizmos.color = new Color(0f, 1f, 0f, 0.25f); // Semi-transparent green

        // Draw a cylinder-like base to represent the reveal radius
        Vector3 basePosition = new Vector3(tentacleStart.position.x, tentacleStart.position.y - revealHeight / 2, tentacleStart.position.z);
        Gizmos.DrawWireSphere(basePosition, revealRadius);

        // Draw the top circle for the cylinder height
        Vector3 topPosition = new Vector3(tentacleStart.position.x, tentacleStart.position.y + revealHeight / 2, tentacleStart.position.z);
        Gizmos.DrawWireSphere(topPosition, revealRadius);

        // Draw lines to connect the base and top, forming a cylinder shape
        Gizmos.DrawLine(new Vector3(basePosition.x + revealRadius, basePosition.y, basePosition.z),
                        new Vector3(topPosition.x + revealRadius, topPosition.y, topPosition.z));
        Gizmos.DrawLine(new Vector3(basePosition.x - revealRadius, basePosition.y, basePosition.z),
                        new Vector3(topPosition.x - revealRadius, topPosition.y, topPosition.z));
        Gizmos.DrawLine(new Vector3(basePosition.x, basePosition.y, basePosition.z + revealRadius),
                        new Vector3(topPosition.x, topPosition.y, topPosition.z + revealRadius));
        Gizmos.DrawLine(new Vector3(basePosition.x, basePosition.y, basePosition.z - revealRadius),
                        new Vector3(topPosition.x, topPosition.y, topPosition.z - revealRadius));
    }

}
