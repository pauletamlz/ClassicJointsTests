using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class CurveSimulator : MonoBehaviour
{
    [Header("Setup")]
    public AnimationClip sourceClip; 
    public GameObject prefab;
    public int particleCount = 50;
    public float spacing = 0.5f;

    [Header("Simulation")]
    [Range(0,0.5f)] public float timeOffset = 0.1f; 

    private List<Transform> particles = new List<Transform>();

    // --- UPDATE LOOP (SAME AS BEFORE) ---
    void Update()
    {
        if (sourceClip == null || prefab == null) return;
        if (transform.childCount != particleCount) RebuildParticles();

#if UNITY_EDITOR
        AnimationCurve curveY = GetCurve(sourceClip, "LocalPosition.y");
        AnimationCurve curveX = GetCurve(sourceClip, "LocalPosition.x");
        if (curveY != null)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                if (particles[i] == null) continue;
                // We calculate the value based on the "Master" curve
                float t = i * timeOffset; 
                
                // If playing, we scroll. If baking, we don't need Time.time
                if (Application.isPlaying) t += Time.time;

                float yValue = curveY.Evaluate(t);
                float xyValue = curveX.Evaluate(t);
                Vector3 result = new Vector3(i * spacing, yValue, 0);
                result.x = i + xyValue;
                particles[i].localPosition = result;
                
            }
        }
#endif
    }

    // --- HELPER: GET CURVE ---
    AnimationCurve GetCurve(AnimationClip clip, string propertyName)
    {
        foreach (var binding in AnimationUtility.GetCurveBindings(clip))
        {
            if (binding.propertyName.Contains(propertyName))
                return AnimationUtility.GetEditorCurve(clip, binding);
        }
        return null;
    }

    void RebuildParticles()
    {
        particles.Clear();
        while(transform.childCount > 0) DestroyImmediate(transform.GetChild(0).gameObject);
        for (int i = 0; i < particleCount; i++)
        {
            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, transform);
            go.name = $"Particle_{i}"; // Important for Baking
            go.hideFlags = HideFlags.None; // Visible so we can verify
            particles.Add(go.transform);
        }
    }

    // ========================================================
    // 🍞 THE BAKING SYSTEM
    // ========================================================
#if UNITY_EDITOR
    [ContextMenu("Bake to New Animation Clip")]
    public void BakeSimulation()
    {
        if (sourceClip == null) { Debug.LogError("No Source Clip!"); return; }

        // 1. Setup the new Clip
        AnimationClip bakedClip = new AnimationClip();
        bakedClip.name = sourceClip.name + "_Baked";
        
        // 2. Get the Master Curve
        AnimationCurve masterCurve = GetCurve(sourceClip, "LocalPosition.y");
        if (masterCurve == null) return;

        // 3. Iterate over EVERY particle
        for (int i = 0; i < particles.Count; i++)
        {
            string path = particles[i].name; // e.g., "Particle_0"

            // Create curves for X and Y
            AnimationCurve childCurveX = new AnimationCurve();
            AnimationCurve childCurveY = new AnimationCurve();

            // 4. Sample the animation (Create keys)
            // We clone the master curve keys but shift their time!
            foreach (Keyframe k in masterCurve.keys)
            {
                // Shift time by the offset
                float shiftedTime = k.time + (i * timeOffset);
                
                // Handle Looping/Wrapping manually if shiftedTime > clip length
                // For simple baking, we just let the time go forward (Linear)
                // Or we can wrap it: 
                // float wrappedTime = shiftedTime % sourceClip.length; 

                childCurveY.AddKey(new Keyframe(shiftedTime, k.value, k.inTangent, k.outTangent));
                
                // X Position is constant, but we need a curve to keep it in place
                childCurveX.AddKey(new Keyframe(shiftedTime, i * spacing));
            }

            // 5. Bind curves to the new Clip
            bakedClip.SetCurve(path, typeof(Transform), "m_LocalPosition.x", childCurveX);
            bakedClip.SetCurve(path, typeof(Transform), "m_LocalPosition.y", childCurveY);
        }

        // 6. Save to Disk
        string pathName = "Assets/" + bakedClip.name + ".anim";
        pathName = AssetDatabase.GenerateUniqueAssetPath(pathName);
        AssetDatabase.CreateAsset(bakedClip, pathName);
        AssetDatabase.SaveAssets();

        Debug.Log($"✨ Baked successfully to: {pathName}");
    }
#endif
}