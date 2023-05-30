using UnityEditor;
using FirstPersonMobileTools;

[CustomEditor(typeof(MobileButton))]
public class MobileButtonEditor : Editor {

    MobileButton.ButtonMode buttonMode;

    public override void OnInspectorGUI() {

        serializedObject.Update();

        MobileButton mobileButton = (MobileButton)target;
        
        EditorGUILayout.LabelField("Button Settings", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Image"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_OnPressedTransition"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_OnReleasedTransition"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_TransitionDuration"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ButtonMode")); 

        switch (mobileButton.m_ButtonMode)
        {
            case MobileButton.ButtonMode.SingleTap:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_OnClicked"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_OnAfterClicked"));
                break;

            case MobileButton.ButtonMode.Toggle:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_OnToggleOn"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_OnToggleOff"));
                break;
                
            case MobileButton.ButtonMode.PressAndRelease:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_OnButtonPressed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_OnButtonReleased"));
                break;
        }   

        if (buttonMode != mobileButton.m_ButtonMode) 
        {
            mobileButton.OnChangeSettings();
            buttonMode = mobileButton.m_ButtonMode;
        }

        if (mobileButton.m_Image == null)
        {
            mobileButton.GetImage();
        }

        serializedObject.ApplyModifiedProperties();
        
    }
}