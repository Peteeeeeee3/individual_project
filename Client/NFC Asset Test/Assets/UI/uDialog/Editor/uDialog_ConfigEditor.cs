using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System;
using System.Text;
using System.Collections.Generic;

namespace UI.Dialogs
{
    [CustomEditor(typeof(uDialog_Config))]
    public class uDialog_ConfigEditor : Editor
    {
        private uDialog_Config uDialog_Config;
        private SerializedObject SO_Target;
        private SerializedProperty ColorSchemes;
        
        public void OnEnable()
        {
            uDialog_Config = target as uDialog_Config;
            SO_Target = new SerializedObject(target);

            ColorSchemes = SO_Target.FindProperty("ColorSchemes");
        }

        public override void OnInspectorGUI()
        {
            var namesInUse = new List<string>();
            for (var x = 0; x < ColorSchemes.arraySize; x++)
            {
                var colorScheme = ColorSchemes.GetArrayElementAtIndex(x);

                var colorSchemeObject = uDialog_Config.ColorSchemes[x];
                if (namesInUse.Contains(colorSchemeObject.Name))
                {                    
                    colorSchemeObject.Name = colorSchemeObject.Name + "-";
                }

                namesInUse.Add(colorSchemeObject.Name);
                colorScheme.FindPropertyRelative("Name").stringValue = colorSchemeObject.Name;

                EditorGUILayout.PropertyField(colorScheme, true);                
            }

            SO_Target.ApplyModifiedProperties();
            
            uDialog_Utilities.LoadColorSchemes();

            var colorSchemeCount = uDialog_Utilities.ColorSchemes.Count;
            uDialog_Utilities.InitializeDefaultThemes();
            if (colorSchemeCount != uDialog_Utilities.ColorSchemes.Count) uDialog_Utilities.SaveColorSchemes();
        }
    }   
}
