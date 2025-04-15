

#if UNITY_EDITOR

namespace Game.Editor
{
    using System;
    using UnityEditor;
    using UnityEngine;
    public class StringEditWindow : EditorWindow
    {
        private string tip;
        private string initialString;
        private string editedString = "";
        private ValidationDelegate validator;
        private Action<string> callback;

        private bool focusInputField = false;

        public static void OpenWindow(string tip, string initialString, ValidationDelegate validator,
            Action<string> callback, string title = "Input")
        {
            var window = GetWindow<StringEditWindow>();
            window.Init(tip, initialString, validator, callback);
            window.titleContent = new GUIContent(title);
            window.ShowModalUtility();
        }

        private void Init(string tip, string initialString, ValidationDelegate validator,
            System.Action<string> callback)
        {
            this.tip = tip;
            this.initialString = initialString;
            this.editedString = initialString;
            this.validator = validator;
            this.callback = callback;
        }

        private void OnGUI()
        {
            const string ctrlName = "InputField";
            if (!focusInputField)
            {
                GUI.SetNextControlName(ctrlName);
            }

            editedString = EditorGUILayout.TextField($"{tip}:", editedString);

            if (!focusInputField)
            {
                focusInputField = true;
                EditorGUI.FocusTextInControl(ctrlName);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Save"))
            {
                Save();
            }

            EditorGUILayout.HelpBox("Press Enter to save, Esc to cancel", MessageType.None);

            if (Event.current.isKey && Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.KeyUp)
            {
                Event.current.Use();
                Save();
            }

            if (Event.current.isKey && Event.current.keyCode == KeyCode.Escape && Event.current.type == EventType.KeyUp)
            {
                Event.current.Use();
                Close();
            }
        }

        private void Save()
        {
            if (!string.IsNullOrWhiteSpace(initialString) && (initialString != editedString))
            {
                if (!EditorUtility.DisplayDialog("Warning",
                        $"Are you sure to save the changes?\n\n    {initialString} => {editedString}", "Yes", "No"))
                {
                    return;
                }
            }

            if (validator != null)
            {
                var validationResult = validator.Invoke(editedString);
                if (!validationResult.IsValid)
                {
                    EditorUtility.DisplayDialog("Validation Failed", validationResult.Message, "OK");
                    return;
                }
            }

            callback?.Invoke(editedString);
            Close();
        }
    }
}

#endif