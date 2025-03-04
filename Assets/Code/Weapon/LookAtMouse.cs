using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMouse : MonoBehaviour
{
    [SerializeField] private Transform weapon;

    private void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out var raycastHit)) return;
        if (!raycastHit.collider) return;
        Vector3 direction = raycastHit.point - weapon.transform.position;
        weapon.rotation = Quaternion.LookRotation(direction);
    }
}
