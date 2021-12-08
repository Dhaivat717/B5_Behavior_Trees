using UnityEngine;
using System.Collections;

public class turnLight : MonoBehaviour {

    // Use this for initialization

    public Light lamp;
    public bool switchinRange = false;
    int count = 0;

    void Start () 
    {
    }

    // Update is called once per frame
    void FixedUpdate () {
        switchinRange = GameObject.Find("Cylinder").GetComponent<PlayerinRange>().playerinRange;
        if (Input.GetKeyDown (KeyCode.Space) & switchinRange) {
            lamp.enabled = !lamp.enabled;
            count = count + 1;
        }

    }
    // void OnTriggerEnter (Collider other)
    // {
    //     lamp.enabled = false;
    // }
}
