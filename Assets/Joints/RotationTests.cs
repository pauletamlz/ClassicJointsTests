using UnityEngine;

[ExecuteAlways]
public class RotationTests : MonoBehaviour
{
    public Transform sphere1,sphere2;
    
    void Update()
    {
        Vector3 dir = sphere1.position - transform.position;
        transform.right = -dir;

    }
    
    public float gizmoLength = 1f; // Adjust this in the Inspector

    void OnDrawGizmos()
    {
        // Apply the object's transform to the Gizmos
        Gizmos.matrix = transform.localToWorldMatrix;

        // Draw X-axis (Red)
        Gizmos.color = Color.red;
        Gizmos.DrawRay(Vector3.zero, Vector3.right * gizmoLength);

        // Draw Y-axis (Green)
        Gizmos.color = Color.green;
        Gizmos.DrawRay(Vector3.zero, Vector3.up * gizmoLength);

        // Draw Z-axis (Blue)
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(Vector3.zero, Vector3.forward * gizmoLength);

        // Reset Gizmos.matrix to default for other gizmos if needed
        Gizmos.matrix = Matrix4x4.identity;
    }
}
