// 1. Define the struct. Must match C# and Compute Shader.
struct ParticleData
{
    float3 position;
    float3 velocity;
    float life;
};

// 2. The HLSL function to get position
//    The input buffer name 'buffer' must match the input
//    slot created on the node.
float3 GetSimulatedPosition(StructuredBuffer<ParticleData> _ParticleBuffer, uint id)
{
    return _ParticleBuffer[id].position;
}

// 3. A separate function to get velocity
float3 GetSimulatedVelocity(StructuredBuffer<ParticleData> _ParticleBuffer, uint id)
{
    return _ParticleBuffer[id].velocity;
}