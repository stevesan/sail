using UnityEngine;
using System.Collections;

public class Weight : MonoBehaviour {

    public Vector3 force;
    public Rigidbody target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate()
    {
        target.AddForceAtPosition( force, transform.position );
	
	}
}
