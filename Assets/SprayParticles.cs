using UnityEngine;
using System.Collections;

public class SprayParticles : MonoBehaviour
{
    public Rigidbody boat;
    public float emissionRatePerSpeed = 10f;

    void Start()
    {
        if( boat == null )
            boat = Utils.FindComponentUpward<Rigidbody>(gameObject);
    }

	// Update is called once per frame
	void Update()
    {
        Vector3 vel = boat.velocity;
        vel.y = 0f; // ignore vertical component
        float speed = vel.magnitude;
        particleSystem.emissionRate = speed * emissionRatePerSpeed;
	
	}
}
