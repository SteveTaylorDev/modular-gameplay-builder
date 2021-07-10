using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer (typeof (Tag))]
public class TagDrawer : PropertyDrawer {
    
    private SerializedProperty _serializedProperty;
    public SerializedProperty serializedProperty {
        get { return _serializedProperty; }
        private set { _serializedProperty = value; }
    }


    public static GenericMenu GetTagMenu (GenericMenu.MenuFunction2 function) {

        GenericMenu oMenu   = new GenericMenu();
        if (function != null) {
            List<Tag>   oTags   = new List<Tag>(Resources.FindObjectsOfTypeAll(typeof(Tag)) as Tag[]);
                        oTags.Sort();

            oMenu.AddItem(new GUIContent("None"), false, function, null);

            for (int i = 0; i < oTags.Count; ++i) {
                if (oTags[i].Depth > -1) {
                    oMenu.AddItem(new GUIContent(oTags[i].GetFullPath() + " "), false, function, oTags[i]);
                }
            }
        }
        return oMenu;
    }



    private void DropdownClickHandler (object oObject) {
        serializedProperty.objectReferenceValue = oObject as Tag;
        serializedProperty.serializedObject.ApplyModifiedProperties();
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
        serializedProperty = property;

        Tag         oTag                    = property.objectReferenceValue as Tag;
        Rect        rLabel                  = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
        Rect        rButton                 = new Rect(position.x + rLabel.width, position.y, position.width - rLabel.width, position.height);

        GUIStyle    oButtonStyle            = new GUIStyle("toolbarDropDown");
                    oButtonStyle.alignment  = TextAnchor.MiddleCenter;

        if (label != GUIContent.none) {
            EditorGUI.LabelField(rLabel, label);
        }
        else {
            rButton = position;
        }

        if (EditorGUI.DropdownButton(rButton, new GUIContent (oTag ? oTag.GetFullPath () : "None"), FocusType.Keyboard)) {

            GenericMenu oMenu   = new GenericMenu();
                        oMenu.AddItem(new GUIContent("None"), false, DropdownClickHandler, null);

            List<Tag>   oTags   = new List<Tag>(Resources.FindObjectsOfTypeAll(typeof(Tag)) as Tag[]);
                        oTags.Sort();

            for (int i = 0; i < oTags.Count; ++i) {
                if (oTags[i].Depth > -1) {
                    oMenu.AddItem(new GUIContent(oTags[i].GetFullPath() + " "), false, DropdownClickHandler, oTags[i]);
                }
            }

            oMenu.ShowAsContext();
        }
    }

}
