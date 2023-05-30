using UnityEditor;
using FirstPersonMobileTools;

[CustomEditor(typeof(Joystick))]
public class JoystickEditor : Editor {

    Joystick.JoystickMode joystickMode;

    public override void OnInspectorGUI () {

        serializedObject.Update();

        Joystick joystick = (Joystick)target;
        EditorGUILayout.LabelField("Joystick Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_JoystickMode"));
        
        if (joystick.m_JoystickMode != Joystick.JoystickMode.Fixed)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_JoystickTouchArea"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_HandleRectTransform"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_HandleBaseRectTransform"));

        if (joystickMode != joystick.m_JoystickMode)
        {
            joystickMode = joystick.m_JoystickMode;
            joystick.OnChangeSettings();
        }

        serializedObject.ApplyModifiedProperties();
        
    }
    
}