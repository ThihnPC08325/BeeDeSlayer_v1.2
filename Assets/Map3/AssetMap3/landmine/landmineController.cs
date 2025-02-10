using UnityEngine;

public class landmineController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float detectRange;
    [SerializeField] private float dangerousRange;
    [SerializeField] private float explodeRange;
    [SerializeField] private Light pointLight;
    [SerializeField] private GameObject explodeEffect;

    private enum State { Idle, Alert, Explode }
    private State currentState;
    private Transform player;
    private bool increasing = true;

    void Start()
    {
        currentState = State.Idle;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        pointLight.intensity = 0;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                if (getDistance() <= detectRange)
                    ChangeState(State.Alert);
                else pointLight.intensity = 0;
                break;

            case State.Alert:
                if (getDistance() <= dangerousRange)
                    ChangeState(State.Explode);
                else ChangeState(State.Idle);
                break;

            case State.Explode:
                if (getDistance() <= explodeRange)
                    Explode();
                else ChangeState(State.Alert);
                break;
        }
        UpdatePointLight();
    }

    private float getDistance()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        return distance;

    }

    private void Explode()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        playerHealth.TakeDamage(10, 0);
        Instantiate(explodeEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void ChangeState(State newState)
    {
        currentState = newState;
    }

    private void SetPointLight(int minIntensity, int maxIntensity, float changeSpeed)
    {
        if (increasing)
        {
            pointLight.intensity += changeSpeed * Time.deltaTime;
            if (pointLight.intensity >= maxIntensity)
                increasing = false;
        }

        if (!increasing)
        {
            pointLight.intensity -= changeSpeed * Time.deltaTime;
            if (pointLight.intensity <= minIntensity)
                increasing = true;
        }
    }

    private void UpdatePointLight()
    {
        if (currentState == State.Explode)
        {
            SetPointLight(30, 80, 200);
        }

        if (currentState == State.Alert)
        {
            SetPointLight(0, 50, 100);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dangerousRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explodeRange);
    }

}
