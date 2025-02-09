using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOnDame : MonoBehaviour
{
    [SerializeField] private GameObject zoneDame;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(OnDame());
    }

    private IEnumerator OnDame()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
