using System;
using UnityEngine;

public class HeadBehaviour : MonoBehaviour
{
    public JointListManager manager;
    public Profile headProfile;
    public MyJointPoint head;
    public float counter;
    public float frequency;
    public float amplitude;
    
    private void Awake()
    {
        manager.OnInit += OnInit;
    }

    void OnInit()
    {
        head = manager.pList[0]; 
        manager.pList[0].profile = headProfile;
        Debug.Log(head);
    }
    
    public void Update()
    {
        if (!manager.initialized)
            return;

        counter += Time.deltaTime;
        float x = Mathf.Sin(counter * frequency) * amplitude;
        head.transform.position = new Vector3(x, head.transform.position.y, 
            head.transform.position.z);
        
    }
}
