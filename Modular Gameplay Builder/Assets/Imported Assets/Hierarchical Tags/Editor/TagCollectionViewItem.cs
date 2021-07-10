using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace UnityEditor {

    /// <summary>
    /// A Tree View Item that contains a reference to a serialized Tag object.
    /// </summary>
    [System.Serializable]
    public class TagCollectionViewItem : TreeViewItem {

        [SerializeField]
        private SerializedObject _serializedObject;

        public SerializedObject serializedObject {
            get { return _serializedObject; }
            set { _serializedObject = value; }
        }
    }

}