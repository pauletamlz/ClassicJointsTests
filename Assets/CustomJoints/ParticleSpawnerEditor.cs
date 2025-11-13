using UnityEngine;
using UnityEditor;

public class ParticleSpawnerWindow : EditorWindow
{
    public WaveParticlesConfig config;
    private SerializedObject serializedConfig; 
    private Vector2 scrollPos;

    [MenuItem("Tools/Wave Particles Config")]
    public static void ShowWindow()
    {
        GetWindow<ParticleSpawnerWindow>("Wave Particles Config");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("WAVE PARTICLES CONFIG", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUI.BeginChangeCheck();
        config = (WaveParticlesConfig)EditorGUILayout.
            ObjectField("Config File", config, 
                typeof(WaveParticlesConfig), false);
        
        if (EditorGUI.EndChangeCheck())
        {
            serializedConfig = null;
        }
        
        if (config == null)
        {
            EditorGUILayout.HelpBox("Please assign or create a " +
                                    "Wave Particles Config file.",
                                    MessageType.Info);
            if (GUILayout.Button("Create New Config"))
            {
                CreateNewConfig();
            }
            return;
        }
        
        if (serializedConfig == null || 
            serializedConfig.targetObject != config)
        {
            serializedConfig = new SerializedObject(config);
        }
        
        serializedConfig.Update();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        SerializedProperty prop = serializedConfig.GetIterator();
        bool enterChildren = true;
        while (prop.NextVisible(enterChildren))
        {
            enterChildren = false;
            if (prop.name == "m_Script") continue; 

            EditorGUILayout.PropertyField(prop, true);
        }

        EditorGUILayout.EndScrollView();
        serializedConfig.ApplyModifiedProperties();
        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("Create", 
                GUILayout.Height(40)))
        {
            CreateParticles();
        }
    }

    private void CreateParticles()
    {
        if (config.prefab == null)
        {
            Debug.LogError("Please assign a Prefab first.");
            return;
        }
        
        GameObject g = GameObject.Find("Particle_Container");
        if (g!=null)
            DestroyImmediate(g);
        GameObject parent = new GameObject("Particle_Container");
        Animator a = parent.AddComponent<Animator>();
        a.runtimeAnimatorController = config.animController;
        Undo.RegisterCreatedObjectUndo(parent, "Spawn Particles");
        
        
        for (int i = 0; i < config.numOfParticles; i++)
        {
            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(
                config.prefab);
            go.name = "Particle_" + i;
            go.transform.SetParent(parent.transform);
            Vector3 pos = new Vector3(i * config.displaceInterval, 0, 0);
            go.transform.position = pos;
            Undo.RegisterCreatedObjectUndo(go, "Spawn Particles");
        }
        
        for (int i = 0; i < config.numOfParticles; i++)
        {
            if (config.animationClip != null)
            {
                ApplyShiftedAnimation(config,parent.transform);
            }
        }

        Debug.Log($"Spawned {config.numOfParticles} particles.");
    }
    
    
    static void ApplyShiftedAnimation(WaveParticlesConfig config, Transform pParent)
    {
    
        AnimationCurve cx = config.curveX;
        AnimationCurve cy = config.curveY;

        Transform[] transforms = new Transform[pParent.childCount];
        for ( int i = 0; i < transforms.Length; i++ )
            transforms[i] = pParent.transform.GetChild(i);
        
        var numberOfCurves = transforms.Length * 2; // 3 for position, 4 for rotation, 3 for scale

        var bindings = new EditorCurveBinding[numberOfCurves];
        var curves = new AnimationCurve[numberOfCurves];

        float initialTime = 0;
        float initialDisplace = 0;

        for (int i = 0; i < transforms.Length; ++i)
        {
            var startIndex = i * 2;

            var transform = transforms[i];
            var path = AnimationUtility.CalculateTransformPath(transform, 
                pParent.transform);

            var index = startIndex;
            bindings[index++] = EditorCurveBinding.FloatCurve(path, 
                typeof(Transform), "m_LocalPosition.x");
            bindings[index++] = EditorCurveBinding.FloatCurve(path, 
                typeof(Transform), "m_LocalPosition.y");
            
            Keyframe[] keys = cx.keys;
            for (int k = 0; k < keys.Length; k++)
            {
                keys[k].time += initialTime * config.animScaleMultiplier;
                keys[k].value = initialDisplace + (keys[k].value * config.animScaleMultiplier);
            }
                

            AnimationCurve cxshift = new AnimationCurve(keys);
            cxshift.preWrapMode = cx.preWrapMode;
            cxshift.postWrapMode = cx.postWrapMode;
            
            keys = cy.keys;
            for (int k = 0; k < keys.Length; k++)
            {
                keys[k].time += initialTime * config.animScaleMultiplier;
                keys[k].value *= config.animScaleMultiplier;
            }
                

            AnimationCurve cyshift = new AnimationCurve(keys);
            cyshift.preWrapMode = cy.preWrapMode;
            cyshift.postWrapMode = cy.postWrapMode;
            
            index = startIndex;
            curves[index++] = cxshift;
            curves[index++] = cyshift;
            initialTime += config.timeInterval;
            initialDisplace += config.displaceInterval;

        }
        
        AnimationUtility.SetEditorCurves(config.animationClip, bindings, curves);
        EditorUtility.SetDirty(config.animationClip);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
    private void CreateNewConfig()
    {
        // Helper to create a file quickly if one doesn't exist
        WaveParticlesConfig newAsset = ScriptableObject.
            CreateInstance<WaveParticlesConfig>();
        string path = "Assets/CustomJoints/WaveJointsConfig.asset";
        path = AssetDatabase.GenerateUniqueAssetPath(path);
        
        AssetDatabase.CreateAsset(newAsset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        config = newAsset; // Auto-assign
    }
}