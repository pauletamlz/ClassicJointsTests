using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways] // Runs in Edit Mode and Play Mode
public class WaveConductor : MonoBehaviour
{
    [Header("Target")]
    public AnimationClip clipToEdit; // Assign the clip you are editing here
    public int particleCount = 50;
    public GameObject prefab;
    public float timeOffset = 0.1f;
    public float xSpacing = 0.5f;
    public float animScaler;

    [Header("Debug")]
    public bool liveUpdate = true;

    // Internal list
    private List<Transform> _particles = new List<Transform>();

    private void Update()
    {
        // Only run if we have the basics
        if (clipToEdit == null || !liveUpdate) return;

        // 1. Lazy Init: ensure we have particles (Simple pooling for this example)
        if (transform.childCount != particleCount) RebuildParticles();
        if (_particles.Count == 0 && transform.childCount > 0) RefreshList();

#if UNITY_EDITOR
        // 2. LIVE EXTRACTION: Get the curve data directly from the Clip in memory
        // This allows you to drag keys in the Animation Window and see updates INSTANTLY
        AnimationCurve curveX = null;
        AnimationCurve curveY = null;

        // Get all bindings to find the position ones
        var bindings = AnimationUtility.GetCurveBindings(clipToEdit);
        foreach (var b in bindings)
        {
            if (b.propertyName.Contains("LocalPosition.x")) 
                curveX = AnimationUtility.GetEditorCurve(clipToEdit, b);
            
            if (b.propertyName.Contains("LocalPosition.y")) 
                curveY = AnimationUtility.GetEditorCurve(clipToEdit, b);
        }

        // 3. APPLY TO PARTICLES
        if (curveX != null && curveY != null)
        {
            // We use a simulated time variable. 
            // In Edit Mode, we often just want to see the static shape of the wave.
            // If you want it to animate, add (Time.realtimeSinceStartup) to the evaluate time.
            
            for (int i = 0; i < _particles.Count; i++)
            {
                if (_particles[i] == null) continue;

                // The Magic: Evaluate the curve at different times based on index
                float evalTime = i * timeOffset;
                
                // Optional: Add scrolling
                if (Application.isPlaying) evalTime += Time.time;

                float xVal = curveX.Evaluate(evalTime) * animScaler;
                float yVal = curveY.Evaluate(evalTime) * animScaler;

                // Position: X is spacing + animation, Y is animation
                _particles[i].localPosition = new Vector3((i * xSpacing) + xVal, yVal, 0);
            }
        }
#endif
    }

    // Helper to spawn objects
    public void RebuildParticles()
    {
        // Cleanup children
        while (transform.childCount > 0) DestroyImmediate(transform.GetChild(0).gameObject);
        _particles.Clear();

        if (prefab == null) return;

        for (int i = 0; i < particleCount; i++)
        {
            GameObject go = Instantiate(prefab, transform);
            go.name = $"Particle_{i}";
            go.hideFlags = HideFlags.DontSaveInEditor; // Don't clutter scene file
            _particles.Add(go.transform);
        }
    }

    void RefreshList()
    {
        _particles.Clear();
        foreach (Transform t in transform) _particles.Add(t);
    }
}