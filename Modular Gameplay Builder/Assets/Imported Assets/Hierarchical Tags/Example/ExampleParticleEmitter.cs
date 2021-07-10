using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This object can be activated and deactivated, adding or removing a tag to the main example object respectively.
/// If the example object has any of the tags that block this emitter, or does not have the tags required by this emitter, it will not activate.
/// </summary>
[System.Serializable]
public class ExampleParticleEmitter : MonoBehaviour {

    private     bool            active;
    public      ExampleObject   exampleObject;
    public new  ParticleSystem  particleSystem;

    public      TagContainer    appliedTags     = new TagContainer();
    public      TagContainer    requiredTags    = new TagContainer();
    public      TagContainer    blockedTags     = new TagContainer();


    //Activate and apply tags.
    public void Activate() {
        if (!active && exampleObject) {
            if (exampleObject.tags.AllTagsMatch (requiredTags) && !exampleObject.tags.AnyTagsMatch (blockedTags)) {

                exampleObject.tags.AddTags(appliedTags);

                if (particleSystem) {
                    particleSystem.Play();
                }

                active = true;
            }
        }
    }

    //Deactivate and remove tags.
    public void Deactivate() {
        if (active) {
            if (exampleObject) {
                exampleObject.tags.RemoveTags(appliedTags);
            }
            if (particleSystem) {
                particleSystem.Stop();
            }

            active = false;
        }
    }
}
