using UnityEngine;
using System.Collections;

public class OneOffParticle : MonoBehaviour
{
	
	// Update is called once per frame
	void Update()
    {
        if( !particleSystem.isPlaying )
            Destroy(gameObject);
	}
}
