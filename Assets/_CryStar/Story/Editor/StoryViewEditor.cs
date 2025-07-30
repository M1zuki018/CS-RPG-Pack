#if UNITY_EDITOR
using System.Reflection;
using System.Text;
using CryStar.Story.UI;
using UnityEditor;
using UnityEngine;

namespace CryStar.Story.Editor
{
    /// <summary>
    /// StoryViewのカスタムインスペクター
    /// </summary>
    [CustomEditor(typeof(StoryView))]
    public class StoryViewEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Debug Tools", EditorStyles.boldLabel);

            if (GUILayout.Button("Validate Components"))
            {
                var storyView = target as StoryView;
                ValidateStoryView(storyView);
            }
        }
        
        private void ValidateStoryView(StoryView storyView)
        {
            var report = new StringBuilder();
            report.AppendLine("=== StoryView Validation ===");

            // SerializeFieldの検証をReflectionで行う
            var fields = typeof(StoryView).GetFields(
                BindingFlags.NonPublic |
                BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (field.GetCustomAttributes(typeof(SerializeField), false).Length > 0)
                {
                    var value = field.GetValue(storyView);
                    if (value == null)
                    {
                        report.AppendLine($"✗ {field.Name} is null");
                    }
                    else
                    {
                        report.AppendLine($"✓ {field.Name} is assigned");
                    }
                }
            }

            Debug.Log(report.ToString());
        }
    }
}
#endif