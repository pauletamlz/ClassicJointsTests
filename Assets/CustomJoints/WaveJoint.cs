using System;
using UnityEngine;

[ExecuteAlways]
public class WaveJoint : MonoBehaviour
{
    public bool head;
    public WaveJoint link;
    public Vector3 position, oldPosition;
    public Vector3 velocity, oldVelocity;
    private Vector3 initializedPosition;

    public void Initialize()
    {
        initializedPosition = transform.position;
    }
    
    public void DisplaceHead(Vector3 p)
    {

        transform.position = p;
    }
    
    public void Step()
    {
        if (!head)
            transform.position = link.oldPosition;
        
        oldPosition = position;
        oldVelocity = velocity;
        position = transform.position;
        velocity = position - oldPosition;
    }

    public void Reset()
    {
        position = initializedPosition;
        oldPosition = initializedPosition;
        transform.position = initializedPosition;
    }

    private void OnDrawGizmos()
    {
        if(link == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position,link.transform.position-transform.position);
    }
}
