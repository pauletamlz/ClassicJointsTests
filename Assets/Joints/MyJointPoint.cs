using System;
using UnityEngine;

public class MyJointPoint : MonoBehaviour
{
    public int index;
    public Rigidbody rb;
    public ConfigurableJoint cj;
    public bool broke;
    public Profile profile;
    public GameObject trail;
    
    public void OnJointBreak(float breakForce)
    {
        broke = true;
    }
    
    public void SetLocks()
    {
        if (broke)
            return;
        
        cj.xMotion = ConfigurableJointMotion.Limited;
        cj.yMotion = ConfigurableJointMotion.Limited;
        cj.zMotion = ConfigurableJointMotion.Limited;
        cj.angularXMotion = ConfigurableJointMotion.Locked;
        cj.angularYMotion = ConfigurableJointMotion.Locked;
        cj.angularZMotion = ConfigurableJointMotion.Limited;
        
        if (profile.xMotionLock)
            cj.xMotion = ConfigurableJointMotion.Locked;
        if (profile.yMotionLock)
            cj.yMotion = ConfigurableJointMotion.Locked;
        if (profile.zMotionLock)
            cj.zMotion = ConfigurableJointMotion.Locked;
        if (profile.xRotationLock)
            cj.angularXMotion = ConfigurableJointMotion.Locked;
        if (profile.yRotationLock)
            cj.angularYMotion = ConfigurableJointMotion.Locked;
        if (profile.zRotationLock)
            cj.angularZMotion = ConfigurableJointMotion.Locked;
    }
    
    public void SetLimitSpring()
    {
        if (broke)
            return;
        
        float tempSpringStiffness = 0;
        float tempSpringDamper = 0;
        if (profile.useLinearSprings)
        {
            tempSpringStiffness = profile.linearStiffness;
            tempSpringDamper = profile.linearDamper;
        }
        SoftJointLimitSpring limitSpring = new SoftJointLimitSpring();
        limitSpring.spring = tempSpringStiffness;
        limitSpring.damper = tempSpringDamper;
        cj.linearLimitSpring = limitSpring;
    }
    
    public void SetLinearLimit()
    {
        if (broke)
            return;
        
        float tempLinearLimit = 0;
        float tempLinearBounce = 0;
        float tempoLinearContactDistance = 0;
        
        if (profile.useLinearLimits)
        {
            tempLinearLimit = profile.linearLimit;
            if (profile.useLimitRelatedToIndex)
                tempLinearLimit = profile.linearLimit + 
                                  index / (1+profile.increaseLimitFactor);;
            tempLinearBounce = profile.linearBounce;
            tempoLinearContactDistance = profile.linearContactDistance;
        }
        
        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = tempLinearLimit;
        limit.bounciness = tempLinearBounce;
        limit.contactDistance = tempoLinearContactDistance;
        cj.linearLimit = limit;
    }

    public void SetDrive()
    {
        if (broke)
            return;
        
        float tempDriveStiffness = 0;
        float tempDriveDamper = 0;
        Vector3 tempTargetPosition = Vector3.zero;
        Vector3 tempTargetVelocity = Vector3.zero;

        if (profile.useDrive)
        {
            tempDriveStiffness = profile.driverStiffness;
            tempDriveDamper = profile.driverDamping;
            tempTargetPosition = profile.targetPosition;
            tempTargetVelocity = profile.targetVelocity;
        }
        
        JointDrive jd = new JointDrive();
        jd.positionSpring = tempDriveStiffness;
        jd.positionDamper = tempDriveDamper;
        
        cj.xDrive = jd;
        cj.yDrive = jd;
        cj.targetPosition = tempTargetPosition;
        cj.targetVelocity = tempTargetVelocity;
    }
    
    public void SetBreakForce()
    {
        if (broke)
            return;
        if(profile.toBreak)
            cj.breakForce = profile.breakForce;
    }

    private void FixedUpdate()
    {
        rb.mass = profile.mass;
        rb.angularDamping = profile.angularDamping;
        rb.linearDamping = profile.linearDamping;
        
        SetLocks();
        SetLimitSpring();
        SetLinearLimit();
        SetDrive();

        SetBreakForce();
        trail.SetActive(profile.trail);
    }
}
