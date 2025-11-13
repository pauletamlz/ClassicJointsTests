using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;

public class JointListManager : MonoBehaviour
{
    public int numOfPoints;
    public float interval;
    public GameObject pointPrefab;
    public Transform pointsParent;
    public Profile profile;
    private Profile[] profileList;
    public bool initialized;
    public List<MyJointPoint> pList;
    public event Action OnInit;
    
    void OnEnable()
    {
        Initialize();
    }

    void SetInit()
    {
        initialized = true;
    }
    
    private int debug = 0;
    [ContextMenu("Initialize")]
    public void Initialize()
    {
        initialized = false;
        timer = 0;
        Invoke("SetInit", 1);
        if (pointsParent != null)
            DestroyImmediate(pointsParent.gameObject);
        GameObject goParent = new GameObject();
        goParent.name = "pParent" + debug++;
        pointsParent = goParent.transform;
        pList = new List<MyJointPoint>();
        
        for (int i = 0; i < numOfPoints; i++)
        {
            Vector3 pos = new Vector3( i * interval, 0, 0);
            GameObject go = Instantiate(pointPrefab, pos, 
                Quaternion.identity, pointsParent);
            MyJointPoint mjp = go.GetComponent<MyJointPoint>();
            mjp.index = i;
            mjp.rb = go.GetComponent<Rigidbody>();
            mjp.cj = go.GetComponent<ConfigurableJoint>();
            mjp.profile = profile;
            if (i > 0)
                mjp.cj.connectedBody = pList[i - 1].rb;
            pList.Add(mjp);
        }
        OnInit?.Invoke();
    }

    float timer = 0;
    public float loopTime;
    
    void FixedUpdate()
    {
        if (!initialized)
            return;
        
        timer += Time.deltaTime;
        if(timer > loopTime)
            Initialize();
    }
    



}
