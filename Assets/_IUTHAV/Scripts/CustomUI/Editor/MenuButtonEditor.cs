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

        protected override void OnEnable()
        {
            base.OnEnable();
            selectionAction = serializedObject.FindProperty("selectionAction");
            deselectionAction = serializedObject.FindProperty("deselectionAction"); // Initialize if needed
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(selectionAction);
            EditorGUILayout.PropertyField(deselectionAction);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
