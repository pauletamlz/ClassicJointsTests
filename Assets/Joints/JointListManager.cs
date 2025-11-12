using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

[System.Serializable]
public struct MyJointPoint
{
    public GameObject go;
    public Rigidbody rb;
    public ConfigurableJoint cj;
}

public class JointListManager : MonoBehaviour
{
    public int numOfPoints;
    public float interval;
    public GameObject pointPrefab;
    public Transform pointsParent;
    public Transform trail;

    [Header("Springs")]
    public bool useSprings;
    public float springStiffness = 5000f;
    public float springDamper = 100f;
    public float spacing;
    public float angularLimit;
    public float angularBounciness;
    public float angularContactDistance = 0.01f;

    [Header("Drive")]
    public float driverStiffness;
    public float driverDamping;
    public bool useDrive;
    //public bool driveUseAcc;
    public Vector3 targetPosition;
    public Vector3 targetRotation;
    public Vector3 targetVelocity;
    public Vector3 targetAngularVelocity;
    
    [Header("Rigidibodies")]
    public float mass;
    public float linearDamping;
    public float angularDamping;

    [Header("HeadMotion")]
    public float amplitude;
    public float frequency;
    public bool initialized;
    public HeadMotion headMotionType;

    public List<MyJointPoint> pList;
    

    public enum HeadMotion
    {
        Displacement,
        Velocity,
        DriveDisplacement,
        DriveVelocity,
        DriveAccDisplacement,
        DriveAccVelocity
    }
        
    
    
    void OnEnable()
    {
        Initialize();
        Invoke("SetInit", 1);
    }

    void SetInit()
    {
        initialized = true;
    }

    [ContextMenu("Initialize")]
    public void Initialize()
    {
        if (pointsParent != null)
            DestroyImmediate(pointsParent.gameObject);
        GameObject goParent = new GameObject();
        goParent.name = "pParent";
        pointsParent = goParent.transform;
        pList = new List<MyJointPoint>();

        for (int i = 0; i < numOfPoints; i++)
        {
            GameObject go = Instantiate(pointPrefab, pointsParent);
            go.transform.position = new Vector3(i * interval, 0, 0);
            MyJointPoint mjp = new MyJointPoint();
            mjp.go = go;
            mjp.rb = go.GetComponent<Rigidbody>();
            mjp.cj = go.GetComponent<ConfigurableJoint>();
            if (i>0)
                mjp.cj.connectedBody = pList[i - 1].rb;
            pList.Add(mjp);
            UpdateJointSettings();

        }

    }
    

    public void UpdateJointSettings()
    {
        for (int i = 0; i < pList.Count; i++)
        {
            pList[i].rb.mass = mass;
            pList[i].rb.angularDamping = angularDamping;
            pList[i].rb.linearDamping = linearDamping;

            if (useSprings)
            {
                SoftJointLimitSpring spring = pList[i].cj.linearLimitSpring;
                spring.spring = springStiffness;
                spring.damper = springDamper;
                pList[i].cj.linearLimitSpring = spring;

                // We also need to define the limit itself
                SoftJointLimit limit = pList[i].cj.linearLimit;
                limit.limit = spacing; // Set the resting distance
                pList[i].cj.linearLimit = limit;

                //angular limit spring
                //pList[i].cj.angularXLimitSpring = copyJoint.angularXLimitSpring;
                //pList[i].cj.angularYZLimitSpring = copyJoint.angularYZLimitSpring;
                //pList[i].cj.angularYZLimitSpring = copyJoint.angularYZLimitSpring;

                //angular limits
                SoftJointLimit joint = new SoftJointLimit();
                joint.bounciness = angularBounciness;
                joint.contactDistance = angularContactDistance;
                joint.limit = angularLimit;
                pList[i].cj.lowAngularXLimit = joint;
                pList[i].cj.angularYLimit = joint;
                pList[i].cj.angularZLimit = joint;
                pList[i].cj.highAngularXLimit = joint;

            }

            if (useDrive)
            {
                //linear drives
                JointDrive jd = new JointDrive();
                jd.positionSpring = driverStiffness;
                jd.positionDamper = driverDamping;
                jd.maximumForce = Mathf.Infinity;
                //jd.useAcceleration = driveUseAcc;
                pList[i].cj.xDrive = jd;
                pList[i].cj.yDrive = jd;

                //angular drives

                //pList[i].cj.angularXDrive = copyJoint.angularXDrive;
                //pList[i].cj.angularYZDrive = copyJoint.angularYZDrive;

                //targets
                pList[i].cj.targetAngularVelocity = targetAngularVelocity;
                pList[i].cj.targetPosition = targetPosition;
                pList[i].cj.targetRotation = Quaternion.Euler(targetRotation);
                pList[i].cj.targetVelocity = targetVelocity;

                //pList[i].cj.rotationDriveMode = copyJoint.rotationDriveMode;
                //pList[i].cj.slerpDrive = copyJoint.slerpDrive;
            }

            //linear motion
            pList[i].cj.xMotion = ConfigurableJointMotion.Limited;
            pList[i].cj.yMotion = ConfigurableJointMotion.Limited;
            pList[i].cj.zMotion = ConfigurableJointMotion.Locked;

            //angular motion
            pList[i].cj.angularXMotion = ConfigurableJointMotion.Locked;
            pList[i].cj.angularYMotion = ConfigurableJointMotion.Locked;
            pList[i].cj.angularZMotion = ConfigurableJointMotion.Limited; 

            //misc
            //pList[i].cj.breakForce = copyJoint.breakForce;
            //pList[i].cj.breakTorque = copyJoint.breakTorque;
            //pList[i].cj.configuredInWorldSpace = copyJoint.configuredInWorldSpace;
            
            //pList[i].cj.projectionMode = copyJoint.projectionMode;
            //pList[i].cj.swapBodies = copyJoint.swapBodies;
            pList[i].cj.anchor = Vector3.zero;
            pList[i].cj.connectedAnchor = new Vector3(spacing, 0, 0);
        }

        
    }

    float timer = 0;
    void FixedUpdate()
    {
        if (!initialized)
            return;

        timer += Time.deltaTime;
        UpdateJointSettings();
        ApplyHeadMotion();
        
    }



    public void ApplyHeadMotion()
    {
        trail.position = pList[0].rb.position;
        pList[0].cj.angularZMotion = ConfigurableJointMotion.Locked;

        float yVelocity = Mathf.Sin(timer * frequency) * amplitude;
        float xVelocity = Mathf.Cos(timer * frequency) * amplitude;
        Vector3 result = new Vector3(xVelocity, yVelocity, 0);
        ChangeDrive(false);

        switch (headMotionType)
        {
            case HeadMotion.Displacement:
                pList[0].rb.isKinematic = true;
                pList[0].rb.MovePosition(result);
                break;
            case HeadMotion.Velocity:
                pList[0].rb.linearVelocity = result;
                break;
            case HeadMotion.DriveDisplacement:
                pList[0].cj.targetPosition = result;
                break;
            case HeadMotion.DriveVelocity:
                pList[0].cj.targetVelocity = result;
                break;
            case HeadMotion.DriveAccDisplacement:
                ChangeDrive(true);
                pList[0].cj.targetPosition = result;
                break;
            case HeadMotion.DriveAccVelocity:
                ChangeDrive(true);
                pList[0].cj.targetVelocity = result;
                break;
        }
    }

    void ChangeDrive(bool acc)
    {
        JointDrive jdx = pList[0].cj.xDrive;
        JointDrive jdy = pList[0].cj.yDrive;
        jdx.useAcceleration = acc;
        jdy.useAcceleration = acc;
        pList[0].cj.xDrive = jdx;
        pList[0].cj.yDrive = jdy;
    }
}
