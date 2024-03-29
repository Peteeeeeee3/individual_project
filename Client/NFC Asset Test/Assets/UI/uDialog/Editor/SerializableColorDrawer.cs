using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System;
using System.Text;
using System.Collections.Generic;

namespace UI.Dialogs
{
    [CustomPropertyDrawer(typeof(SerializableColor))]
    public class SerializableColorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {            
            EditorGUI.BeginProperty(position, label, property);                        

            var r = property.FindPropertyRelative("r");
            var g = property.FindPropertyRelative("g");
            var b = property.FindPropertyRelative("b");
            var a = property.FindPropertyRelative("a");            

            var colorValue = new Color(r.floatValue, g.floatValue, b.floatValue, a.floatValue);            

            position = EditorGUI.PrefixLabel(position, label);
            colorValue = EditorGUI.ColorField(position, colorValue);

            r.floatValue = colorValue.r;
            g.floatValue = colorValue.g;
            b.floatValue = colorValue.b;
            a.floatValue = colorValue.a;

            EditorGUI.EndProperty();
        }
    }
}
