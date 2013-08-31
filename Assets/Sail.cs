using UnityEngine;
using System.Collections;

public class Sail : MonoBehaviour
{
    static float DebugDrawScale = 5;

    public float modulus;
    public Rigidbody target;
    public Vector3 localSailNormal;

    public bool catchesWind = false;

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

    void OnCapsizeRecover()
    {
        prevPos = transform.position;
    }

    void FixedUpdate()
    {
        // We can NOT just use target.velocity here, since we are concerned about our own velocity!
        Vector3 velocity = (transform.position - prevPos) / Time.fixedDeltaTime;
        Vector3 wsNormal = transform.TransformDirection( localSailNormal );
        Vector3 dragForce = -modulus * wsNormal * Vector3.Dot(wsNormal, velocity);

        target.AddForceAtPosition( dragForce, transform.position );
        Debug.DrawLine( transform.position, transform.position+dragForce*DebugDrawScale, Color.red );

        prevPos = transform.position;

        //----------------------------------------
        //  Wind
        //----------------------------------------
        if( catchesWind && Wind.main != null )
        {
            Vector3 windForce = wsNormal * Vector3.Dot( Wind.main.force, wsNormal );
            target.AddForceAtPosition( windForce, transform.position );

            Debug.DrawLine( transform.position, transform.position+windForce*DebugDrawScale, Color.blue );
        }
    }

    void QuickSave(string prefix)
    {
        Utils.SaveVector3( prefix+gameObject.name+".sailPrevPos", prevPos );
    }

    void QuickLoad(string prefix)
    {
        Utils.LoadVector3( prefix+gameObject.name+".sailPrevPos", ref prevPos );
    }
}
