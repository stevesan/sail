using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
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
        {
            BoatControls.main.QuickSave("checkpoint");
            Debug.Log("Checkpoint!");
        }
    }
}
