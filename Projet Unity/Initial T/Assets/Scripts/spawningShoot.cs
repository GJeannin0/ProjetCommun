using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawningShoot : MonoBehaviour
{
    [SerializeField] private Transform spawner;
    [SerializeField] private GameObject bulletPrefab;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("y"))
            Instantiate(bulletPrefab, spawner.position, spawner.rotation);
    }
}
