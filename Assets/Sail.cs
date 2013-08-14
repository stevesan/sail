using UnityEngine;
using System.Collections;

public class Sail : MonoBehaviour
{
    static float DebugDrawScale = 5;

    public float modulus;
    public Rigidbody target;
    public Vector3 localSailNormal;

    public Vector3 debugWind;

    Vector3 prevPos;

    void Awake()
    {
        prevPos = transform.position;
    }

    void Start()
    {
        if( target == null )
            target = gameObject.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 velocity = (transform.position - prevPos) / Time.fixedDeltaTime;
        Vector3 wsNormal = transform.TransformDirection( localSailNormal );
        Vector3 dragForce = -modulus * wsNormal * Vector3.Dot(wsNormal, velocity);

        target.AddForceAtPosition( dragForce, transform.position );
        Debug.DrawLine( transform.position, transform.position+dragForce*DebugDrawScale, Color.red );

        prevPos = transform.position;

        //----------------------------------------
        //  Wind
        //----------------------------------------
        Vector3 windForce = wsNormal * Vector3.Dot( debugWind.normalized, wsNormal );
        target.AddForceAtPosition( windForce, transform.position );

        Debug.DrawLine( transform.position, transform.position+windForce*DebugDrawScale, Color.blue );
    }
}
