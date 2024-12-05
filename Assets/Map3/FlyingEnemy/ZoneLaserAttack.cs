using System.Collections;
using UnityEngine;

public class ZoneLaserAttack : MonoBehaviour
{
    // Constants
    private const int PointCount = 30;
    private const int PositionRange = 10;
    private const float AngleStep = 360f / PointCount;
    private const float LineWidth = 0.2f;

    // Serialized fields
    [SerializeField] private float heightOffset = 0.25f;
    [SerializeField] private float expandSpeed = 2f;
    [SerializeField] private LineRenderer line;
    [SerializeField] float delayAttack = 2f;

    // Private variables
    private float maxRadius = 10f;
    private Vector3 randPos = Vector3.zero;
    private Vector3[] points = new Vector3[PointCount];
    private Transform player;
    private bool isRun = false;

    public Vector3[] list = null;

    // Unity lifecycle methods
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        PrecomputePoints();
        HideLine();
    }

    private void InitLine()
    {
        line.enabled = true;
        line.positionCount = PointCount;
        line.widthMultiplier = LineWidth;
    }

    private void PrecomputePoints()
    {
        float step = AngleStep * Mathf.Deg2Rad;
        for (int i = 0; i < PointCount; i++)
        {
            float angle = step * i;
            points[i] = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
        }
        points[PointCount - 1] = points[0];
    }

    public void Draw(Vector3 center, float radius)
    {
        for (int i = 0; i < PointCount; i++)
        {
            Vector3 scaled = points[i] * radius;
            line.SetPosition(i, center + new Vector3(scaled.x, heightOffset, scaled.z));
        }
    }

    public Vector3 RandomNearPlayer(Transform player)
    {
        float randX = Random.Range(player.position.x - PositionRange,
            player.position.x + PositionRange);

        float randZ = Random.Range(player.position.z - PositionRange,
            player.position.z + PositionRange);

        return new Vector3(randX, heightOffset, randZ);
    }

    public Vector3[] RandomPositionInCircle(Vector3 center, float maxRadius)
    {
        Vector3[] vector3s = new Vector3[3];
        for (int i = 0; i < 3; i++)
        {
            float radius = Random.Range(0f, maxRadius);

            float angle = Random.Range(0f, 2 * Mathf.PI);

            float x = center.x + radius * Mathf.Cos(angle);
            float z = center.z + radius * Mathf.Sin(angle);

            vector3s[i] = new Vector3(x, heightOffset, z);
        }
        return vector3s;
    }


    public void DestroyAfter()
    {
        StartCoroutine(DestroyWithDelay());
    }

    public void GetDrawWithDelay()
    {
        if (isRun == false)
        {
            isRun = true;
            StartCoroutine(DrawWithDelay());
        }
    }

    private IEnumerator DrawWithDelay()
    {
        InitLine();

        randPos = RandomNearPlayer(player);
        list = RandomPositionInCircle(randPos, maxRadius);
        ShowLine();

        Draw(randPos, maxRadius);
        yield return new WaitForSeconds(delayAttack);
        HideLine();
    }

    private IEnumerator DestroyWithDelay()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    private void HideLine()
    {
        if (line.enabled)
        {
            line.enabled = false;
        }
    }

    private void ShowLine()
    {
        if (!line.enabled)
        {
            line.enabled = true;
        }
    }
}
