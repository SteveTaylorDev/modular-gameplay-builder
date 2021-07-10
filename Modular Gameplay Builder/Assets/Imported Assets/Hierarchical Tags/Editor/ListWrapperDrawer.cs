using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;

namespace UnityEditor {
    
    [CustomPropertyDrawer(typeof(ListWrapper), true)]
    public class ListWrapperDrawer : PropertyDrawer {

        [SerializeField]    private SerializedProperty  _property;
        [SerializeField]    private SerializedProperty  _listProperty;
        [SerializeField]    private ReorderableList     _list;
        [SerializeField]    private int                 _selection      = -1;


        public SerializedProperty   property        { get { return _property; }     private set { _property     = value; } }
        public SerializedProperty   listProperty    { get { return _listProperty; } private set { _listProperty = value; } }
        public ReorderableList      list            { get { return _list; }         private set { _list         = value; } }
        public bool                 initialized     { get { return listProperty != null && list != null; } }
        public int                  selection       { get { return _selection; }    private set { _selection    = value; } }

        public virtual bool displayFoldout      { get { return true; } }
        public virtual bool draggable           { get { return true; } }
        public virtual bool displayHeader       { get { return true; } }
        public virtual bool displayAddButton    { get { return true; } }
        public virtual bool displayRemoveButton { get { return true; } }
        


        public override float GetPropertyHeight(SerializedProperty spProperty, GUIContent oLabel) {
            if (!initialized) { Initialize(spProperty); }
            
            if (initialized) {
                if (displayFoldout) {
                    if (spProperty.isExpanded) { return EditorGUIUtility.singleLineHeight + list.GetHeight(); }
                    else { return EditorGUIUtility.singleLineHeight; }
                }
                else { return list.GetHeight(); }
            }
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect rPosition, SerializedProperty spProperty, GUIContent oLabel) {
            if (!initialized) { Initialize(spProperty); }

            if (initialized) {
                if (displayFoldout) {
                    Rect rFoldout = new Rect(rPosition.x, rPosition.y, rPosition.width, EditorGUIUtility.singleLineHeight);
                    if (spProperty.isExpanded = EditorGUI.Foldout(rFoldout, spProperty.isExpanded, new GUIContent(spProperty.displayName))) {
                        
                        Rect    rList   = new Rect(rPosition.x, rPosition.y + EditorGUIUtility.singleLineHeight, rPosition.width, list.GetHeight());
                                rList   = EditorGUI.IndentedRect(rList);

                        list.DoList(rList);
                    }
                }
                else {
                    list.DoList(rPosition);
                }
            }
        }



        protected void Initialize(SerializedProperty spProperty) {
            SerializedProperty spList = spProperty.FindPropertyRelative("_list");

            if (spList != null && spList.isArray) {
                property        = spProperty;
                listProperty    = spList;

                List<SerializedProperty> oElements = new List<SerializedProperty>();
                if (spList.isArray) {
                    for (int i = 0; i < spList.arraySize; ++i) {
                        oElements.Add(spList.GetArrayElementAtIndex(i));
                    }
                }

                if (oElements != null) {
                    list = new ReorderableList(oElements, typeof(SerializedProperty), draggable, displayHeader, displayAddButton, displayRemoveButton);

                    list.drawHeaderCallback     = DisplayHeaderCallback;
                    list.drawElementCallback    = DrawElementCallback;
                    list.onAddCallback          = AddCallback;
                    list.onRemoveCallback       = RemoveCallback;
                    list.onCanAddCallback       = CanAddCallback;
                    list.onCanRemoveCallback    = CanRemoveCallback;
                    //list.onReorderCallback  = ReorderCallback;
                    list.elementHeightCallback  = ElementHeightCallback;
                    list.onSelectCallback       = SelectCallback;
                }

                OnInitialize(spProperty);
            }
        }



        protected virtual void OnInitialize(SerializedProperty spProperty) { }

        protected virtual void DisplayHeaderCallback (Rect rRect) {
            if (property == null) { return; }
            int iIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUI.LabelField(rRect, property.displayName);
            EditorGUI.indentLevel = iIndent;
        }
        protected virtual void DrawElementCallback (Rect rRect, int iIndex, bool bActive, bool bFocused) {
            if (listProperty == null) { return; }
            int iIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 1;
            EditorGUI.PropertyField(rRect, listProperty.GetArrayElementAtIndex(iIndex));
            EditorGUI.indentLevel = iIndent;
        }
        protected virtual void AddCallback(ReorderableList oList) {
            if (listProperty == null) { return; }
            listProperty.arraySize++;
            listProperty.serializedObject.ApplyModifiedProperties();
            Initialize(property);
        }
        protected virtual void RemoveCallback (ReorderableList oList) {
            if (listProperty == null) { return; }
            listProperty.DeleteArrayElementAtIndex (oList.index);
            listProperty.serializedObject.ApplyModifiedProperties();
            Initialize(property);
        }
        protected virtual bool CanAddCallback (ReorderableList oList) {
            if (listProperty == null) { return false; }
            return true;
        }
        protected virtual bool CanRemoveCallback (ReorderableList oList) {
            if (listProperty == null) { return false; }
            return true;
        }
        protected virtual void ReorderCallback(ReorderableList oList) {
            if (listProperty == null) { return; }
        }
        protected virtual float ElementHeightCallback (int iIndex) {
            return EditorGUIUtility.singleLineHeight + 2.0f;
        }
        protected virtual void SelectCallback (ReorderableList oList) {
            selection = oList.index;
        }
    }

}