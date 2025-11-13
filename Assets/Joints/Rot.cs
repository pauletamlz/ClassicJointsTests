using System;
using UnityEngine;

[System.Serializable]
public class MyTest
{
    public Transform t;
    public Vector3 initialPosition;
    public Rigidbody rb;

    public void SetIniti()
    {
        initialPosition = t.position;
    }
    
    public void Initialize()
    {
        rb.isKinematic = false;
    }
    
    public void Reset()
    {
        rb.isKinematic = true;
        rb.ResetInertiaTensor();
        t.position = initialPosition;
        rb.rotation = Quaternion.identity;
    }
}

public class Rot : MonoBehaviour
{
    public MyTest test1,test2,test3;

    private void Awake()
    {
        test1.SetIniti();
        test2.SetIniti();
        test3.SetIniti();
        
    }

    public void Initialize()
    {
        test1.Initialize();
        test2.Initialize();
        test3.Initialize();
    }
    
    public void Reset()
    {
        test1.Reset();
        test2.Reset();
        test3.Reset();
    }

    public float timeScale;
    public void Update()
    {
        Time.timeScale = timeScale;
    }
}
