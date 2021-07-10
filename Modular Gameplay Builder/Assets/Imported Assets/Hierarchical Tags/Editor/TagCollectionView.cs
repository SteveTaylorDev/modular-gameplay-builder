using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace UnityEditor {

    /// <summary>
    /// Allows the contents of the Tag Collection to be viewed as a tree structure, much like that of the Hierarchy window.
    /// </summary>
    [System.Serializable]
    public class TagCollectionView : TreeView {
        [SerializeField]    private TagCollection       _target;
        [SerializeField]    private SerializedObject    _serializedObject;

        public TagCollection target {
            get { return _target; }
            private set { _target = value; }
        }
        public SerializedObject serializedObject {
            get { return _serializedObject; }
            private set { _serializedObject = value; }
        }


        public TagCollectionView(TagCollection oCollection, TreeViewState oTreeViewState) : base(oTreeViewState) {
            showAlternatingRowBackgrounds   = true;
            showBorder                      = true;
            if (oCollection != null) {
                target = oCollection;
            }
            Reload();
        }
        

        #region Functions
        //Get a list of tags from the serialized object.
        public List<Tag> GetElementsAsList() {
            List<Tag> oList = new List<Tag>();
            if (serializedObject != null) {
                SerializedProperty spList = serializedObject.FindProperty("_elements");
                for (int i = 0; i < spList.arraySize; ++i) {
                    oList.Add(spList.GetArrayElementAtIndex(i).objectReferenceValue as Tag);
                }
            }
            return oList;
        }

        //Set the serialized properties from a list of tags.
        public void SetElementsFromList(List<Tag> oList) {
            if (serializedObject == null || oList == null) { return; }
            oList.RemoveAll(o => o == null);

            SerializedProperty spList = serializedObject.FindProperty("_elements");
            spList.arraySize = oList.Count;

            for (int i = 0; i < spList.arraySize; ++i) {
                SerializedProperty spElement = spList.GetArrayElementAtIndex(i);
                spElement.objectReferenceValue = oList[i];
            }

            spList.serializedObject.ApplyModifiedProperties();
            Reload();
        }

        
        public int GetMainSelectionID() {
            int[] iSelections = GetSelection() as int[];
            return iSelections.Length > 0 ? iSelections[0] : -1;
        }

        public int GetIndexFromID (int iID) {
            if (iID > -1) {
                TagCollectionViewItem oItem = FindItem(iID, rootItem) as TagCollectionViewItem;
                if (oItem != null) {
                    SerializedProperty spList = serializedObject.FindProperty("_elements");
                    for (int i = 0; i < spList.arraySize; ++i) {
                        SerializedProperty spProperty = spList.GetArrayElementAtIndex(i);
                        if (oItem.serializedObject != null && spProperty.objectReferenceValue == oItem.serializedObject.targetObject) {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        public void Rename () {
            if (rootItem == null) { return; }
            int iSelection = GetMainSelectionID();

            TagCollectionViewItem oItem = FindItem(iSelection, rootItem) as TagCollectionViewItem;

            if (oItem != null) { BeginRename(oItem); }
        }
        #endregion
        

        #region Overridden functions
        protected override TreeViewItem BuildRoot() {
            if (!target) { return null; }
            serializedObject = new SerializedObject(target);

            TagCollectionViewItem         oRoot   = new TagCollectionViewItem() { id = 0, depth = -1, displayName = "Null", children = new List<TreeViewItem> () };
            List<TagCollectionViewItem>   oItems  = new List<TagCollectionViewItem>() { };

            List<Tag>   oTags = GetElementsAsList();
                        oTags.RemoveAll (o => o == null);
                        oTags.Sort(new TagViewSetupComparer ());

            for (int i = 0; i < oTags.Count; ++i) {
                if (oTags[i]) {
                    SerializedObject soTag = new SerializedObject(oTags[i]);
                    soTag.ApplyModifiedProperties();
                }
            }
            
            if (oTags.Count > 0 && oTags[0] != null) {
                oRoot.serializedObject = new SerializedObject (oTags[0]);
                oRoot.displayName = oTags[0].name;
            }

            for (int i = 1; i < oTags.Count; ++i) {
                Tag                 oTag    = oTags[i];
                SerializedObject    soTag   = new SerializedObject (oTag);

                SerializedProperty spChildren = soTag.FindProperty("_children");
                for (int c = 0; c < spChildren.arraySize; ++c) {
                    SerializedProperty spChild = spChildren.GetArrayElementAtIndex(c);
                    if (spChild.objectReferenceValue == null) {
                        spChildren.DeleteArrayElementAtIndex(c);
                        --c;
                    }
                }

                int iParentIndex = oItems.FindIndex(o => o.serializedObject != null && o.serializedObject.targetObject == oTag.parent);
                TagCollectionViewItem oParentItem = iParentIndex > -1 ? oItems[iParentIndex] : oRoot;
                
                TagCollectionViewItem oNewItem    = new TagCollectionViewItem() {
                    serializedObject    = soTag,
                    displayName         = oTag.name,
                    depth               = oTag.Depth,
                    id                  = oTag.GetInstanceID (),
                    parent              = oParentItem,
                    children            = new List<TreeViewItem> ()
                };

                if (oParentItem != null) { oParentItem.children.Add(oNewItem); }
                int iChildIndex = oParentItem != null ? oParentItem.children.IndexOf (oNewItem) : 0;
                oItems.Insert(iParentIndex + 1 + iChildIndex, oNewItem);
            }
            return oRoot;
        }


        protected override float GetCustomRowHeight(int row, TreeViewItem item) {
            return EditorGUIUtility.singleLineHeight;
        }

        protected override void RowGUI(RowGUIArgs args) {
            GUIStyle    oLabelStyle             = EditorStyles.helpBox;
                        oLabelStyle.alignment   = TextAnchor.MiddleLeft;

            float   fIndent     = GetContentIndent(args.item);
            Rect    rLabelRect  = new Rect(args.rowRect.x + fIndent, args.rowRect.y, args.rowRect.width - fIndent, args.rowRect.height);
            
            TagCollectionViewItem oItem = args.item as TagCollectionViewItem;
            EditorGUI.LabelField(rLabelRect, new GUIContent(oItem.displayName));
            //DEBUG:
            //EditorGUI.LabelField(rLabelRect, new GUIContent(oItem.displayName + " :: Depth = " + oItem.depth.ToString () + " :: Parent = " + (oItem.parent != null ? oItem.parent.displayName : "Null")));
        }


        //Rename
        protected override bool CanRename(TreeViewItem item) {
            return item != rootItem;
        }

        protected override void RenameEnded(RenameEndedArgs args) {
            if (args.acceptedRename) {
                TagCollectionViewItem oItem = FindItem(args.itemID, rootItem) as TagCollectionViewItem;

                if (oItem != null && oItem.serializedObject != null) {
                    oItem.displayName = args.newName;
                    oItem.serializedObject.targetObject.name = args.newName;
                    AssetDatabase.SaveAssets();
                }
            }
        }

        //Drag-and-Drop
        protected override bool CanStartDrag(CanStartDragArgs args) {
            return args.draggedItem != rootItem;
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args) {
            base.SetupDragAndDrop(args);
            int[] iSelection = GetSelection() as int[];
            List<TagCollectionViewItem> oItems = new List<TagCollectionViewItem>();

            for (int i = 0; i < iSelection.Length; ++i) { oItems.Add(FindItem(iSelection[i], rootItem) as TagCollectionViewItem); }

            DragAndDrop.SetGenericData(typeof(List<TagCollectionViewItem>).Name, oItems);
            DragAndDrop.StartDrag("Move Tag");
        }
        
        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args) {
            if (args.performDrop) {
                List<TagCollectionViewItem> oDraggedItems = DragAndDrop.GetGenericData(typeof(List<TagCollectionViewItem>).Name) as List<TagCollectionViewItem>;
                
                if (oDraggedItems.Count > 0) {
                    List<Tag> oTags = GetElementsAsList();

                    if (oTags.Count < 1) {
                        return DragAndDropVisualMode.Rejected;
                    }

                    TagCollectionViewItem     oNewParentItem  = args.parentItem as TagCollectionViewItem;
                    SerializedObject    soNewParent     = oNewParentItem.serializedObject;

                    Tag     oNewParent  = soNewParent != null ? soNewParent.targetObject as Tag : null;
                    int     iNewIndex   = args.insertAtIndex;
                    int     iMainDepth  = oDraggedItems[0].depth;

                    for (int i = 0; i < oDraggedItems.Count; ++i) {
                        TagCollectionViewItem     oMoveItem   = oDraggedItems[i];
                        Tag                 oMoveTag    = oMoveItem.serializedObject.targetObject as Tag;

                        if (oMoveTag.Depth <= iMainDepth) {
                            iMainDepth = oMoveTag.Depth;

                            SerializedObject    soMoveTag   = new SerializedObject(oMoveTag);
                            SerializedProperty  spParent    = soMoveTag.FindProperty("_parent");
                            Tag                 oParent     = spParent.objectReferenceValue as Tag;

                            if (oMoveTag.parent != null && oMoveTag.parent == oNewParent) {
                                iNewIndex = iNewIndex - (oMoveTag.parent != null && (oMoveTag.parent.children.IndexOf(oMoveTag) < iNewIndex) ? 1 : 0);

                                SerializedProperty spChildrenList = soNewParent.FindProperty("_children");
                                int iChildIndex = oNewParent.children.IndexOf(oMoveTag);
                                spChildrenList.MoveArrayElement(iChildIndex, iNewIndex);
                                spChildrenList.serializedObject.ApplyModifiedProperties();
                            }
                            else {
                                if (oMoveTag.parent != null) {
                                    SerializedObject    soParent    = new SerializedObject(oParent);

                                    int iChildIndex = oParent.children.FindIndex(o => o == oMoveTag);
                                    if (iChildIndex > -1) {
                                        SerializedProperty  spChildrenList  = soParent.FindProperty("_children");
                                        SerializedProperty  spChildElement  = spChildrenList.GetArrayElementAtIndex(iChildIndex);
                                        spChildElement.objectReferenceValue = null;
                                        spChildrenList.DeleteArrayElementAtIndex(iChildIndex);
                                        soParent.ApplyModifiedProperties();
                                    }
                                }

                                if (oNewParent != null) {
                                    SerializedProperty spChildrenList = soNewParent.FindProperty("_children");
                                    spChildrenList.arraySize++;
                                    spChildrenList.GetArrayElementAtIndex(spChildrenList.arraySize - 1).objectReferenceValue = oMoveTag;
                                    spChildrenList.MoveArrayElement(spChildrenList.arraySize - 1, iNewIndex);
                                    soNewParent.ApplyModifiedProperties();
                                }

                                spParent.objectReferenceValue = oNewParent;
                            }
                            soMoveTag.ApplyModifiedProperties();
                        }

                        SetSelection(new int[] { oMoveItem.id });
                    }

                    oTags.Sort(new TagViewSetupComparer ());
                    SetElementsFromList(oTags);
                    Reload();
                }
                return DragAndDropVisualMode.Move;
            }
            return DragAndDropVisualMode.Generic;
        }
        #endregion
    }
}
