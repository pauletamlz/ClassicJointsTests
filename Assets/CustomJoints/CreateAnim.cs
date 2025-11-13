using UnityEditor;
using UnityEngine;

static class AnimationClipWithAnimationCurvesExample
{
    // This example creates an AnimationClip with a single frame that captures the pose of a GameObject hierarchy.
    // The clip is saved as an asset in the project.
    [MenuItem("Example/Create Animation Clip Pose From GameObject")]
    static void CreateAnimationClipPoseFromGameObject()
    {
        var selectedGameObject = Selection.activeGameObject;
        if (selectedGameObject == null)
        {
            Debug.LogError("Please select a GameObject to create a clip for.");
            return;
        }
        
        AnimationCurve cx = selectedGameObject.GetComponent<AnimatioCurvesUtils>().curveX;
        AnimationCurve cy = selectedGameObject.GetComponent<AnimatioCurvesUtils>().curveY;

        AnimationClip clip =  new AnimationClip();

        Transform[] transforms = new Transform[selectedGameObject.transform.childCount];
        for ( int i = 0; i < transforms.Length; i++ )
            transforms[i] = selectedGameObject.transform.GetChild(i);
        
        var numberOfCurves = transforms.Length * 2; // 3 for position, 4 for rotation, 3 for scale

        var bindings = new EditorCurveBinding[numberOfCurves];
        var curves = new AnimationCurve[numberOfCurves];

        float initialTime = 0;
        float interval = 0.5f;

        for (int i = 0; i < transforms.Length; ++i)
        {
            var startIndex = i * 2;

            var transform = transforms[i];
            var path = AnimationUtility.CalculateTransformPath(transform, selectedGameObject.transform);

            var index = startIndex;
            bindings[index++] = EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalPosition.x");
            bindings[index++] = EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalPosition.y");
            
            Keyframe[] keys = cx.keys;
            for (int k = 0; k < keys.Length; k++)
            {
                keys[k].time += initialTime;
                keys[k].value += initialTime;
            }
                

            AnimationCurve cxshift = new AnimationCurve(keys);
            cxshift.preWrapMode = cx.preWrapMode;
            cxshift.postWrapMode = cx.postWrapMode;
            
            keys = cy.keys;
            for (int k = 0; k < keys.Length; k++)
                keys[k].time += initialTime;

            AnimationCurve cyshift = new AnimationCurve(keys);
            cyshift.preWrapMode = cy.preWrapMode;
            cyshift.postWrapMode = cy.postWrapMode;
            
            index = startIndex;
            curves[index++] = cxshift;
            curves[index++] = cyshift;
            initialTime += interval;
            //bindings[index++] = EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalPosition.x");
            //bindings[index++] = EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalPosition.y");
            
            //bindings[index++] = EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalPosition.z");

            //bindings[index++] = EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalRotation.x");
            //bindings[index++] = EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalRotation.y");
            //bindings[index++] = EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalRotation.z");
            //bindings[index++] = EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalRotation.w");

            transform.GetLocalPositionAndRotation(out var localPosition, out var localRotation);
            var localScale = transform.localScale;

            index = startIndex;
            //curves[index++] = AnimationCurve.Constant(0f, 1f, localPosition.x);
            //curves[index++] = AnimationCurve.Constant(0f, 1f, localPosition.y);
            //curves[index++] = AnimationCurve.Constant(0f, 1f, localPosition.z);

            //curves[index++] = AnimationCurve.Constant(0f, 1f, localRotation.x);
            //curves[index++] = AnimationCurve.Constant(0f, 1f, localRotation.y);
            //curves[index++] = AnimationCurve.Constant(0f, 1f, localRotation.z);
            //curves[index++] = AnimationCurve.Constant(0f, 1f, localRotation.w);
        }

        
        
        AnimationUtility.SetEditorCurves(clip, bindings, curves);

        //AssetDatabase.CreateAsset(clip, AssetDatabase.GenerateUniqueAssetPath($"Assets/{selectedGameObject.name}-Pose.anim"));
    }
}
