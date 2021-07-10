using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;

namespace UnityEditor {

    [System.Serializable]
    [CustomPropertyDrawer (typeof (TagContainer))]
    public class TagContainerDrawer : ListWrapperDrawer {
    
        protected GenericMenu   _menu;

        public GenericMenu menu {
            get { return _menu; }
            protected set { _menu = value; }
        }
        


        void DropdownClickHandler(object oTarget) {
            if (property != null && oTarget != null) {
                SerializedProperty oTagList = property.FindPropertyRelative("_list");
                oTagList.arraySize += 1;
                oTagList.GetArrayElementAtIndex(oTagList.arraySize - 1).objectReferenceValue = (Tag) oTarget;

                property.serializedObject.ApplyModifiedProperties();
                Initialize(property);
            }
        }


        protected override void DrawElementCallback (Rect rRect, int iIndex, bool isActive, bool isFocused) {
            SerializedProperty  spList  = property.FindPropertyRelative("_list");
            SerializedProperty  spTag   = spList.GetArrayElementAtIndex(iIndex);

            EditorGUI.PropertyField(rRect, spTag, GUIContent.none);
        }

        protected override void AddCallback (ReorderableList oList) {
            if (menu != null) { menu.ShowAsContext(); }
        }

        protected override void RemoveCallback (ReorderableList oList) {
            property.FindPropertyRelative("_list").GetArrayElementAtIndex(oList.index).objectReferenceValue = null;
            ReorderableList.defaultBehaviours.DoRemoveButton(oList);
        }

        

        protected override void OnInitialize(SerializedProperty spProperty) {
                        menu    = new GenericMenu();
            List<Tag>   oTags   = new List<Tag>(Resources.FindObjectsOfTypeAll(typeof(Tag)) as Tag[]);
                        oTags.Sort();

            for (int i = 0; i < oTags.Count; ++i) {
                if (oTags[i].Depth > -1) {
                    menu.AddItem(new GUIContent(oTags[i].GetFullPath() + " "), false, DropdownClickHandler, oTags[i]);
                }
            }
        }
        
        protected override bool CanAddCallback(ReorderableList oList) {
            return menu != null && menu.GetItemCount() > 0;
        }

    }
}