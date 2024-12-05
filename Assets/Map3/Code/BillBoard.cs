using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    [SerializeField] Transform tranformCamera;
    private void LateUpdate()
    {
        transform.LookAt(transform.position + tranformCamera.forward        );
    }
}
