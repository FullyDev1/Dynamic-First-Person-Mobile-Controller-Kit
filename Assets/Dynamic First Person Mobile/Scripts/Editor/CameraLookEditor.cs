using UnityEditor;
using FirstPersonMobileTools.DynamicFirstPerson;

[CustomEditor(typeof(CameraLook))]
public class CameraLookEditor : Editor {

    CameraLook.TouchDetectMode TouchDetectMode;

    public override void OnInspectorGUI () {

        serializedObject.Update();

        CameraLook cameraLook = (CameraLook)target;
        
        EditorGUILayout.LabelField("Camera Look Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_TouchDetectMode"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Sensitivity"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_BottomClamp"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_TopClamp"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_InvertX"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_InvertY"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_TouchLimit"));

        if (TouchDetectMode != cameraLook.m_TouchDetectMode)
        {
            TouchDetectMode = cameraLook.m_TouchDetectMode;
            cameraLook.OnChangeSettings();
        }

        serializedObject.ApplyModifiedProperties();
        
    }

}