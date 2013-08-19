using UnityEngine;
using System.Collections;

public class Buoy : MonoBehaviour {

    public Rigidbody target;
    public float height = 1;
    public float maxMag = 10;

    void Awake()
    {
        if( target == null )
            target = Utils.FindComponentUpward<Rigidbody>(gameObject);
    }

    void FixedUpdate()
    {
        if( target != null )
        {
            float fraction = Utils.Unlerp( 0, -height, transform.position.y );
            target.AddForceAtPosition( Vector3.up*fraction*maxMag, transform.position );
        }
    }
}
