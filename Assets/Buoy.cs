using UnityEngine;
using System.Collections;

public class Buoy : MonoBehaviour {

    public Rigidbody target;
    public float height = 1;
    public float maxMag = 10;

    void FixedUpdate()
    {
        float fraction = Utils.Unlerp( 0, -height, transform.position.y );
        target.AddForceAtPosition( Vector3.up*fraction*maxMag, transform.position );
    }
}
