using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This custom type of Tag provides more flexibility than Unity's default tag system, utilizing a tree structure to allow complex tag comparisons.
/// These hierarchical tags are stored in a Tag Collection asset (see asset menu), and referenced by Tag Containers in scripts.
/// </summary>
[System.Serializable]
public sealed class Tag : ScriptableObject, System.IEquatable<Tag>, System.IComparable<Tag>{
    [SerializeField]    private Tag         _parent;
    [SerializeField]    private List<Tag>   _children       = new List<Tag> ();
    
    public Tag          parent      { get { return _parent; }       set { _parent       = value; } }
    public List<Tag>    children    { get { return _children; }     set { _children     = value; } }



    public bool IsChildOf(Tag oTag) {
        if (!oTag) { return false; }

        List<Tag>   oLineage    = new List<Tag>();
        Tag         oParent     = this;

        while (oParent != null) {
            if (oLineage.Contains(oParent)) { return false; }
            if (oParent == oTag)            { return true; }
            oLineage.Add(oParent);
            oParent = oParent.parent;
        }
        return false;
    }
    
    public string GetFullPath (char cSeparator = '/') {
        List<Tag>   oLineage    = new List<Tag>() { };
        Tag         oParent     = parent;
        string      sPath       = name;
        
        while (oParent != null) {
            if (oParent.parent == null || oLineage.Contains (oParent)) { break; }
            oLineage.Add(oParent);
            sPath   = oParent.name + cSeparator + sPath;
            oParent = oParent.parent;
        }
        return sPath;
    }

    public int Depth {
        get {
            List<Tag>   oLineage    = new List<Tag>() { };
            Tag         oParent     = parent;
        
            while (oParent != null) {
                if (oLineage.Contains (oParent)) { break; }
                oLineage.Add(oParent);
                oParent = oParent.parent;
            }
            return oLineage.Count - 1;
        }
    }


    public bool Equals(Tag other)       { return other == this ? true : false; }
    public int  CompareTo(Tag other)    { return other != null ? GetFullPath ().CompareTo(other.GetFullPath ()) : 1; }

    public static bool operator >   (Tag operand1, Tag operand2) { return operand1.CompareTo(operand2) == 1; }
    public static bool operator <   (Tag operand1, Tag operand2) { return operand1.CompareTo(operand2) == -1; }
    public static bool operator >=  (Tag operand1, Tag operand2) { return operand1.CompareTo(operand2) == 0; }
    public static bool operator <=  (Tag operand1, Tag operand2) { return operand1.CompareTo(operand2) == 0; }
}


/// <summary>
/// Allows scripts to keep track of when Tags are added to or removed from Tag Containers.
/// </summary>
/// <param name="sender"></param>
/// <param name="args"></param>
public delegate void TagEventHandler(object sender, TagEventArgs args);
public sealed class TagEventArgs : System.EventArgs {
    private Tag _tag;
    public Tag tag { get { return _tag; } }

    public TagEventArgs (Tag oTag) {
        _tag = oTag;
    }
}