using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Activates a particle burst whenever any designated tags are selected.
/// </summary>
[System.Serializable]
public class ExampleReactionaryBurst : MonoBehaviour {
    public      ExampleObject   exampleObject;
    public new  ParticleSystem  particleSystem;

    public TagContainer reactToTags = new TagContainer();


    void Burst(object sender, TagEventArgs args) {
        if (particleSystem && reactToTags.ContainsParentOf (args.tag)) {
            particleSystem.Play();
        }
    }

    void Start () {
        if (exampleObject != null) {
            exampleObject.tags.TagAdded += Burst;
        }
    }
}
