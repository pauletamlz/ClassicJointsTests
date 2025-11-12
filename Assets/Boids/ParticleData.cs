using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.VFX;

// This struct's memory layout must match the layout 
// defined in the Compute Shader and VFX Graph HLSL.
// is non-negotiable for this.

[VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
public struct ParticleData
{
    public Vector3 position;
    public Vector3 velocity;
    public float life;
    // Other attributes (e.g., color) can be added here
}