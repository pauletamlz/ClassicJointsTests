using UnityEditor;
using UnityEngine;

public class CurveEditorWindow : EditorWindow
{
    public AnimationCurve curveToEdit = new AnimationCurve();

    [MenuItem("Tools/Standalone Curve Editor")]
    public static void ShowWindow()
    {
        GetWindow<CurveEditorWindow>("Curve Editor");
    }

    private void OnGUI()
    {
        // This draws the curve editor filling the available space
        // The Rect defines the size (filling the window minus some padding)
        Rect rect = GUILayoutUtility.GetRect(position.width, position.height - 20);
        
        // This draws the curve using Unity's internal curve rendering style
        curveToEdit = EditorGUI.CurveField(rect, curveToEdit);
    }
}