using UnityEngine;
using System.Collections;

public class RbSaveTest : MonoBehaviour
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {

        if( Input.GetKeyDown("7") )
            Utils.SaveRigidbody(gameObject.name, rigidbody);
        else if( Input.GetKeyDown("9") )
            Utils.LoadRigidbody(gameObject.name, rigidbody);
	
	}
}
