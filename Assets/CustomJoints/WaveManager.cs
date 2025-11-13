using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject waveJointPrefab;
    public Transform pointsParent;
    public int numPoints;
    public List<WaveJoint> wList;
    public float dt, currentDt, animScale;
    public AnimationCurve curve,curveX,curve2;
    public float counter;
    public float interval;

    void OnEnable()
    {
        Initialize();
    }
    
    [ContextMenu("Initialize")]
    public void Initialize()
    {
        if (pointsParent != null)
            DestroyImmediate(pointsParent.gameObject);
        GameObject goParent = new GameObject();
        goParent.name = "pParent";
        pointsParent = goParent.transform;
        wList = new List<WaveJoint>();
        
        for (int i = 0; i < numPoints; i++)
        {
            Vector3 pos = new Vector3( 0, 0, 0);
            GameObject go = Instantiate(waveJointPrefab, pos, 
                Quaternion.identity, pointsParent);
            WaveJoint wJoint = go.GetComponent<WaveJoint>();
            go.name = "p" + i;
            if (i == 0)
                wJoint.head = true;
            else
                wJoint.link = wList[i - 1];
            wJoint.Initialize();
            wList.Add(wJoint);
        }
    }
    
    [ContextMenu("Reset")]
    public void Reset()
    {
        for ( int i = 0; i < wList.Count; i++ )
            wList[i].Reset();   
        
        currentDt = 0;
    }
    
    [ContextMenu("Step")]
    public void Step()
    {
        float eval = curve.Evaluate(currentDt) * animScale;
        float evalX = curveX.Evaluate(currentDt) * animScale;
        float evalX2 = -curve2.Evaluate(currentDt) * animScale;
        Vector3 p = new Vector3(evalX2, eval, 0);
        wList[0].DisplaceHead(p);
        for ( int i = 0; i < wList.Count; i++ )
            wList[i].Step();  
        currentDt += dt;
    }

    public void Update()
    {
        counter += Time.deltaTime;
        if (counter >= interval)
        {
            counter = 0;
            Step();
            if (currentDt>=1)
                Reset();
        }
            
    }
}
