using UnityEngine;
using UnityEngine.PostProcessing;
//using UnityEngine.MinAttribute;


namespace UnityEditor.PostProcessing
{
    // [CustomPropertyDrawer(typeof(RangeAttribute))]
    // sealed class MinDrawer : PropertyDrawer
    // {
    //     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //     {
    //         RangeAttribute attribute = (RangeAttribute)base.attribute;

    //         if (property.propertyType == SerializedPropertyType.Integer)
    //         {
    //             int v = EditorGUI.IntField(position, label, property.intValue);
    //             property.intValue = (int)Mathf.Max(v, attribute.min);
    //         }
    //         else if (property.propertyType == SerializedPropertyType.Float)
    //         {
    //             float v = EditorGUI.FloatField(position, label, property.floatValue);
    //             property.floatValue = Mathf.Max(v, attribute.min);
    //         }
    //         else
    //         {
    //             EditorGUI.LabelField(position, label.text, "Use Min with float or int.");
    //         }
    //    }
    // }
}
