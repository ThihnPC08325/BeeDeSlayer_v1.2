using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAutoDestroy : MonoBehaviour
{
    private ParticleSystem[] particleSystems;
    // Start is called before the first frame update
    void Start()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }
    void Update()
    {
        // Kiểm tra nếu tất cả các Particle Systems đã dừng
        bool allStopped = true;
        foreach (var ps in particleSystems)
        {
            if (ps.IsAlive())
            {
                allStopped = false;
                break;
            }
        }

        // Nếu tất cả đều đã dừng, hủy game object
        if (allStopped)
        {
            Destroy(gameObject);
        }
    }
}
