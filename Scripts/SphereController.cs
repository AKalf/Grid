using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereController : MonoBehaviour {
    public Transform trans = null;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
    public Transform GetTransform() {
        if (trans == null)
            trans = transform;
        return trans;

    }
}
