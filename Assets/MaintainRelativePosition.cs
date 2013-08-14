using UnityEngine;
using System.Collections;

public class MaintainRelativePosition : MonoBehaviour {

    public Transform target;
    private Vector3 delta;

	// Use this for initialization
	void Start ()
    {
        delta = transform.position - target.transform.position;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void LateUpdate()
    {
        transform.position = target.transform.position + delta;
    }
}
