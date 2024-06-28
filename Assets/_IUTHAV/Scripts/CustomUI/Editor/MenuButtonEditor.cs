using UnityEditor;
using UnityEditor.UI;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _IUTHAV.Scripts.CustomUI.Editor
{
    [CustomEditor(typeof(MenuButton))]
    public class MenuButtonEditor : ButtonEditor
    {
        private SerializedProperty selectionAction;
        private SerializedProperty deselectionAction;
        private SerializedProperty moveAmount;
        private SerializedProperty _recTransform; 
        private SerializedProperty basePosition; 
        private SerializedProperty animationTime; 

        protected override void OnEnable()
        {
            base.OnEnable();
            selectionAction = serializedObject.FindProperty("selectionAction");
            deselectionAction = serializedObject.FindProperty("deselectionAction"); // Initialize if needed
            moveAmount = serializedObject.FindProperty("moveAmount");
            _recTransform = serializedObject.FindProperty("_rectTransform");
            basePosition = serializedObject.FindProperty("basePosition");
            animationTime = serializedObject.FindProperty("animationTime");
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(selectionAction);
            EditorGUILayout.PropertyField(deselectionAction);
            EditorGUILayout.PropertyField(moveAmount);
            EditorGUILayout.PropertyField(_recTransform);
            EditorGUILayout.PropertyField(basePosition);
            EditorGUILayout.PropertyField(animationTime);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
