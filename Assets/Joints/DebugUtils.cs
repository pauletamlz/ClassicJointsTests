using System;
using UnityEngine;

public class DebugUtils : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform target;

    private Vector3[] posList;
    
    private void Update()
    {
        posList = new Vector3[2];
        posList[0] = transform.position;
        posList[1] = target.position;
        lineRenderer.SetPositions(posList);
    }
}
