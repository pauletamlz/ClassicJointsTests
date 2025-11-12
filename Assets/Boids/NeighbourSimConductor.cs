using UnityEngine;
using UnityEngine.VFX;
using System.Runtime.InteropServices;


public class NeighborSimConductor : MonoBehaviour
{
    public int particleCount = 10000;
    public ComputeShader simulationShader;
    public VisualEffect vfxGraph;

    private GraphicsBuffer particleBuffer;
    private int kernelSimulate;
    private int particleStride;

    // 3. Define uniforms set from C#
    private float _DeltaTime;
    private int _ParticleCount;

// 4. Parameters for Boids/Flocking logic [30, 31]
    public float _NeighborRadius;
    public float _SeparationWeight;
    public float _AlignmentWeight;
    public float _CohesionWeight;
    public float _MaxSpeed;


    // A string ID is used to bind the buffer to the shader and graph
    private static readonly int _particleBufferProp = Shader.PropertyToID("_ParticleBuffer");
     

    void OnEnable()
    {
        // 1. Initialize particle data
        ParticleData[] initialData = new ParticleData[particleCount];
        for (int i = 0; i < particleCount; i++)
        {
            initialData[i] = new ParticleData
            {
                position = Random.insideUnitSphere * 10f,
                velocity = Random.onUnitSphere * 0.1f,
                life = 1.0f
            };
        }

        // 2. Create the GraphicsBuffer 
        particleStride = Marshal.SizeOf(typeof(ParticleData));
        particleBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured,
            particleCount, particleStride);
        particleBuffer.SetData(initialData); // 

        // 3. Find the Compute Shader kernel(s)
        // (A full implementation would find all kernels: BuildHash, Sort, etc.)
        kernelSimulate = simulationShader.FindKernel("SimulateNeighbors");

        // 4. Bind the buffer to both systems
        simulationShader.SetBuffer(kernelSimulate, _particleBufferProp, particleBuffer);
        
        // This is the C# -> VFX Graph link
        vfxGraph.SetGraphicsBuffer(_particleBufferProp, particleBuffer);
        
    }

    void OnDisable()
    {
        // Clean up the GPU memory
        particleBuffer?.Release();
        particleBuffer = null;
    }
    
    //... Update loop follows...
    //... (Continuing the NeighborSimConductor class)...

    void Update()
    {
        if (particleBuffer == null) return;

        // 1. Set any dynamic properties (e.g., time, mouse position)
        simulationShader.SetFloat("_DeltaTime", Time.deltaTime);
        simulationShader.SetFloat("_ParticleCount", particleCount);
        simulationShader.SetFloat("_NeighborRadius", _NeighborRadius);
        simulationShader.SetFloat("_SeparationWeight", _SeparationWeight);
        simulationShader.SetFloat("_AlignmentWeight", _AlignmentWeight);
        simulationShader.SetFloat("_CohesionWeight", _CohesionWeight);
        simulationShader.SetFloat("_MaxSpeed", _MaxSpeed);


        
        // 2. Run the simulation "Brain" 
        // This dispatches the simulation kernel.
        // A full implementation would dispatch its kernels in sequence.
        int threadGroups = Mathf.CeilToInt(particleCount / 256.0f);
        simulationShader.Dispatch(kernelSimulate, threadGroups, 1, 1);

        // 3. The VFX Graph will automatically read the updated buffer
        //    for its own simulation step. No further action is needed
        //    because the buffer was bound in OnEnable.
    }
}
