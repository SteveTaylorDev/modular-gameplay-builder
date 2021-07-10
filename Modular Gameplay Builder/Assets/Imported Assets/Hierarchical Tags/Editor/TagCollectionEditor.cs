using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace UnityEditor {

    /// <summary>
    /// Compares tags for sorting into a TreeView format.
    /// </summary>
    public class TagViewSetupComparer : IComparer<Tag> {
        public int Compare(Tag x, Tag y) {
            if (!x || !y) { return 0; }
            int iCompare = x.Depth.CompareTo (y.Depth);
            if (iCompare == 0) {
                int xIndex = x.parent != null ? x.parent.children.IndexOf(x) : 0;
                int yIndex = y.parent != null ? y.parent.children.IndexOf(y) : 0;
                iCompare = xIndex.CompareTo(yIndex);
            }
            return iCompare;
        }
    }



    /// <summary>
    /// Editor script for Tag Collections, utilizing Unity's TreeView system.
    /// </summary>
    [CustomEditor(typeof(TagCollection))]
    public class TagCollectionEditor : Editor {
        [SerializeField]    private TagCollectionView   _treeView;
        [SerializeField]    private TreeViewState       _treeViewState;

        public TagCollectionView    treeView        { get { return _treeView; }         protected set { _treeView = value; } }
        public TreeViewState        treeViewState   { get { return _treeViewState; }    protected set { _treeViewState = value; } }



        [MenuItem("Assets/Create/Tag Collection")]
        public static void CreateTagCollection() {
            TagCollection oAsset = ScriptableObject.CreateInstance<TagCollection> ();
 
		    string sPath = AssetDatabase.GetAssetPath (Selection.activeObject);
		    if (sPath == "") {
                sPath = "Assets";
		    } 
		    else if (Path.GetExtension (sPath) != "")  {
			    sPath = sPath.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
		    }
 
		    string sAssetPathAndName = AssetDatabase.GenerateUniqueAssetPath (sPath + "/New " + typeof(TagCollection).ToString() + ".asset");
 
		    AssetDatabase.CreateAsset (oAsset, sAssetPathAndName);
		    AssetDatabase.SaveAssets ();
            AssetDatabase.Refresh();
		    //EditorUtility.FocusProjectWindow ();
		    Selection.activeObject = oAsset;
        }


        //Initialization
        void OnEnable () {
            TagCollection oCollection = target as TagCollection;

            if (treeViewState == null) {
                treeViewState = new TreeViewState();
            }
            treeView = new TagCollectionView(oCollection, treeViewState) as TagCollectionView;

            if (oCollection.elements.Count == 0) { AddTag(); }
        }


        //Display options and 
        public override void OnInspectorGUI() {
            float fToolbarHeight = EditorGUIUtility.singleLineHeight;

            SerializedProperty spElements = serializedObject.FindProperty("_elements");
            if (spElements.arraySize > 1) {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent("Add"))) {
                    AddTag();
                }
                if (GUILayout.Button(new GUIContent("Remove"))) {
                    RemoveTag();
                }
                GUILayout.EndHorizontal();

                Rect rHeader = GUILayoutUtility.GetLastRect();
                float fTreeStart = rHeader.y + rHeader.height + 5.0f;

                Rect rTree = new Rect(0.0f, fTreeStart, Screen.width, Mathf.Min(treeView != null ? treeView.totalHeight : 0.0f, Screen.height - fTreeStart));

                if (treeView != null) {
                    treeView.OnGUI(rTree);
                }
            }
            else {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent ("Create Tag"))) {
                    AddTag();
                }
                GUILayout.EndHorizontal();
            }
        }






        public void AddTag() {
            SerializedProperty spList = serializedObject.FindProperty ("_elements");

            int iID     = treeView.GetMainSelectionID();

            Tag oNewTag             = ScriptableObject.CreateInstance<Tag> ();
                oNewTag.name        = spList.arraySize == 0 ? "Root" : "Tag";
                oNewTag.hideFlags   = HideFlags.HideInHierarchy;

            AssetDatabase.AddObjectToAsset(oNewTag, target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            spList.arraySize++;
            SerializedProperty  spElement   = spList.GetArrayElementAtIndex(spList.arraySize - 1);
            SerializedObject    soNewTag    = new SerializedObject (oNewTag);
            spElement.objectReferenceValue  = oNewTag;


            //Apply serialization.
            if (spList.arraySize == 1) {
                serializedObject.FindProperty("_root").objectReferenceValue = oNewTag;
            }
            else {
                List<Tag> oTags = treeView.GetElementsAsList();
                Tag oParent = oTags.Find(o => o.GetInstanceID () == iID);
                if (!oParent) { oParent = oTags.Find(o => o.Depth == -1); }
                
                if (oParent != null) {
                    SerializedObject    soParent       = new SerializedObject (oParent);
                    SerializedProperty  spChildList = soParent.FindProperty("_children");

                    spChildList.arraySize++;
                    spChildList.GetArrayElementAtIndex(spChildList.arraySize - 1).objectReferenceValue = oNewTag;
                    soParent.ApplyModifiedProperties();
                }
                soNewTag.FindProperty("_parent").objectReferenceValue = oParent;
            }

            soNewTag.ApplyModifiedProperties();
            serializedObject.ApplyModifiedProperties();
            treeView.Reload();
        }

        public void RemoveTag() {
            if (treeView == null) { return; }

            List<Tag> oTags = treeView.GetElementsAsList();
            if (oTags.Count <= 1) { return; }

            oTags.Sort(new TagViewSetupComparer());
            oTags.RemoveAll(o => o == null);
            
            int[] iSelections   = treeView.GetSelection() as int[];

            //Find selection.
            for (int i = 0; i < iSelections.Length; ++i) {
                Tag oRemoveTag = oTags.Find(o => o.GetInstanceID() == iSelections[i]);
                
                if (oRemoveTag) {
                    //Remove all children of selected tag.
                    List<Tag> oRemoveTags = oTags.FindAll(o => o && o.IsChildOf(oRemoveTag));
                    for (int r = oRemoveTags.Count - 1; r >= 0; --r) {
                        Tag oRemoveChild = oRemoveTags[r];

                        if (oRemoveTag && oRemoveTag.parent != null) {
                            int iChildIndex = oRemoveChild.parent.children.IndexOf(oRemoveChild);
                            if (iChildIndex > -1) {
                                SerializedObject soRemoveParent = new SerializedObject(oRemoveChild.parent);
                                SerializedProperty spChildList = soRemoveParent.FindProperty("_children");
                                spChildList.GetArrayElementAtIndex(iChildIndex).objectReferenceValue = null;
                                spChildList.DeleteArrayElementAtIndex(iChildIndex);
                                soRemoveParent.ApplyModifiedProperties();
                            }

                            SerializedObject soRemoveChild = new SerializedObject(oRemoveChild);
                            SerializedProperty spRemoveChildParent = soRemoveChild.FindProperty("_parent");
                            spRemoveChildParent.objectReferenceValue = null;
                            soRemoveChild.ApplyModifiedProperties();
                        }

                        oTags.Remove(oRemoveTag);
                        Editor.DestroyImmediate(oRemoveChild, true);
                    }
                }
            }

            oTags.RemoveAll(o => o == null);
            treeView.SetElementsFromList(oTags);
        }
    }
}