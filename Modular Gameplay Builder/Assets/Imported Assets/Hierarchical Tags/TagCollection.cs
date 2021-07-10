using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Tag Collection is an asset that houses tag objects.  It should generally be kept in the Assets/Resources folder.
/// </summary>
[System.Serializable]
public sealed class TagCollection : ScriptableObject {
    [SerializeField]    private List<Tag>   _elements   = new List<Tag> ();
    [SerializeField]    private Tag         _root;
    
    public List<Tag> elements {
        get { return _elements; }
    }
    public Tag root {
        get { return _root; }
    }
}
