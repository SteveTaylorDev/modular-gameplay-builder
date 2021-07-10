using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExampleObject : MonoBehaviour {

    public TagContainer tags = new TagContainer();

    public List<ExampleParticleEmitter> emitters = new List<ExampleParticleEmitter>();

    
    void Update () {
        if (emitters.Count > 0 && emitters[0] != null) {
            if (Input.GetButtonDown ("Jump"))   { emitters[0].Activate(); }
            if (Input.GetButtonUp   ("Jump"))   { emitters[0].Deactivate(); }
        }
        if (emitters.Count > 1 && emitters[1] != null) {
            if (Input.GetButtonDown ("Fire1"))  { emitters[1].Activate(); }
            if (Input.GetButtonUp   ("Fire1"))  { emitters[1].Deactivate(); }
        }
        if (emitters.Count > 2 && emitters[2] != null) {
            if (Input.GetButtonDown ("Fire2"))  { emitters[2].Activate(); }
            if (Input.GetButtonUp   ("Fire2"))  { emitters[2].Deactivate(); }
        }
        if (emitters.Count > 3 && emitters[3] != null) {
            if (Input.GetButtonDown ("Fire3"))  { emitters[3].Activate(); }
            if (Input.GetButtonUp   ("Fire3"))  { emitters[3].Deactivate(); }
        }
    }
}
