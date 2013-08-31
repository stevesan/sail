using UnityEngine;
using System.Collections;

public class Key : MonoBehaviour
{

	// Use this for initialization
	void Start()
    {
	
	}
	
	// Update is called once per frame
	void Update()
    {
	
	}

    void OnTriggerEnter( Collider col )
    {
        if( Utils.FindComponentUpward<Boat>(col.gameObject) != null )
            Destroy(this.gameObject);
    }
}
