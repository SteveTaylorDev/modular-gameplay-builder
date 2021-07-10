using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ListWrapper { }

/// <summary>
/// Wrapper class for any generic list, such that it can be drawn as a Reorderable List.
/// Any given list must be in the form of a concrete class, so it should primarily be used for lists that appear in the inspector.
/// </summary>
[System.Serializable]
public class ListWrapper<T> : ListWrapper {
    
    [SerializeField]    private List<T> _list   = new List<T> ();

    public List<T> list {
        get { return _list; }
        protected set { _list = value; }
    }

    public static implicit operator List<T>(ListWrapper<T> oList) { return oList.list; }
    public static implicit operator ListWrapper<T>(List<T> oList) { return new ListWrapper<T>() { list = oList }; }
}
