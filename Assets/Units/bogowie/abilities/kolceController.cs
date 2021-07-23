using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kolceController : MonoBehaviour
{
    public float desrtoyTime;
    float dTime;
    private void Start()
    {
        dTime = Time.time + desrtoyTime;
    }

    private void Update()
    {
        if(dTime < Time.time)
        {
            Destroy(gameObject);
        }
    }
}
