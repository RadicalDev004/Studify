using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinSkin : MonoBehaviour
{
    public int x = 100;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
            transform.Rotate(Vector3.forward, Time.deltaTime * x);
        
    }
}
