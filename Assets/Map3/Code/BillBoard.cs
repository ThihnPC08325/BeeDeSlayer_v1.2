using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    [SerializeField] private Transform transformCamera;
    private void LateUpdate()
    {
        transform.LookAt(transform.position + transformCamera.forward        );
    }
}
