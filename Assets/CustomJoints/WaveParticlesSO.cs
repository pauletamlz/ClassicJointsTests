using UnityEngine;

[CreateAssetMenu(fileName = "NewSpawnerConfig", menuName = "Tools/Spawner Config")]
public class WaveParticlesConfig : ScriptableObject
{
    [Header("Assets")]
    public GameObject prefab;
    public AnimationClip animationClip;
    public RuntimeAnimatorController animController;

    [Header("Curves")]
    public AnimationCurve curveX = AnimationCurve.Linear(0, 0, 1, 0);
    public AnimationCurve curveY = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Settings")]
    public int numOfParticles = 10;
    public float timeInterval = 0.1f;
    public float displaceInterval = 1.0f;
    public float animScaleMultiplier = 1.0f;
}