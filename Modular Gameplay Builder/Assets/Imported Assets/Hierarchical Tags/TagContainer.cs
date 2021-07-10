using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Concrete class for a wrapped list of Tags.  Includes events for adding and removing tags.
/// The provided functions allow containers to be checked for matching tags.
/// </summary>
[System.Serializable]
public class TagContainer : ListWrapper<Tag> {

    public event TagEventHandler    TagAdded;
    public event TagEventHandler    TagRemoved;



    public void TriggerTagAdded (Tag oTag) {
        if (TagAdded != null && oTag != null) { TagAdded (this, new TagEventArgs(oTag)); }
    }
    public void TriggerTagRemoved (Tag oTag) {
        if (TagRemoved != null && oTag != null) { TagRemoved(this, new TagEventArgs(oTag)); }
    }


    #region COMPARISON_FUNCTIONS
    public bool ContainsTag (Tag oTag) {
		return list.Contains (oTag);
	}

    public bool ContainsChildOf (Tag oTag) {
        return list.Exists(o => o != null && o.IsChildOf(oTag));
    }

    public bool ContainsParentOf (Tag oTag) {
        return list.Exists(o => oTag.IsChildOf(o));
    }
    


	public int GetTagQuantity (Tag oTag) {
		return list.FindAll (o => o == oTag).Count;
	}

	public bool AnyTagsMatch (TagContainer oContainer) {
		foreach (Tag oTag in oContainer.list) {
			if (ContainsChildOf (oTag)) {
				return true;
			}
		}
		return false;
	}

	public bool AllTagsMatch (TagContainer oContainer) {
		foreach (Tag oTag in oContainer.list) {
			if (!ContainsChildOf (oTag)) {
				return false;
			}
		}
		return true;
	}

	public bool NoTagsMatch (TagContainer oContainer) {
		foreach (Tag oTag in oContainer.list) {
			if (ContainsChildOf (oTag)) {
				return false;
			}
		}
		return true;
	}
    #endregion


    #region MANAGEMENT_FUNCTIONS
    public void AddTag (Tag oTag) {
        if (oTag != null && !list.Contains(oTag)) {
            list.Add(oTag);
            TriggerTagAdded(oTag);
        }
	}
	public void RemoveTag (Tag oTag) {
        if (oTag != null && list.Contains(oTag)) {
            list.Remove(oTag);
            TriggerTagRemoved(oTag);
        }
    }
    public void RemoveTagAndChildren (Tag oTag) {
        if (oTag != null && list.Contains(oTag)) {
            List<Tag> oTags = list.FindAll(o => o.IsChildOf(oTag));

            for (int i = 0; i < oTags.Count; ++i) {
                list.Remove(oTags[i]);
                TriggerTagRemoved(oTags[i]);
            }
        }
    }
    public void ClearTags() {
        list.Clear();
    }

    public void AddTags (TagContainer oContainer) {
		for (int i = 0; i < oContainer.list.Count; ++i) {
			AddTag (oContainer.list[i]);
		}
	}
	public void RemoveTags (TagContainer oContainer) {
		for (int i = 0; i < oContainer.list.Count; ++i) {
			RemoveTag (oContainer.list[i]);
		}
	}
    #endregion
}