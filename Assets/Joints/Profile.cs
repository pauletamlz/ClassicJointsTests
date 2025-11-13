using UnityEngine;


public enum HeadMotion
{
    Displacement,
    Velocity,
    DriveDisplacement,
    DriveVelocity,
    DriveAccDisplacement,
    DriveAccVelocity
}

[CreateAssetMenu]
public class Profile : ScriptableObject
{
    [Header("Locks")] 
    public bool xMotionLock;
    public bool yMotionLock;
    public bool zMotionLock;
    public bool xRotationLock;
    public bool yRotationLock;
    public bool zRotationLock;
    
    [Header("Linear Springs")]
    public bool useLinearSprings;
    public float linearStiffness = 5000f;
    public float linearDamper = 100f;
    
    [Header("Linear Limits")]
    public bool useLinearLimits;
    public float linearLimit;
    public float linearBounce;
    public float linearContactDistance;

    [Header("Drive")]
    public bool useDrive;
    public float driverStiffness;
    public float driverDamping;
    public Vector3 targetPosition;
    public Vector3 targetVelocity;
    
    [Header("Rigidibodies")]
    public float mass;
    public float linearDamping;
    public float angularDamping;

    [Header("HeadMotion")]
    public float amplitude;
    public float frequency;
    public HeadMotion headMotionType;

    public bool useLimitRelatedToIndex;
    public float increaseLimitFactor;
    public bool toBreak;
    public float breakForce;
    public bool trail;

}
