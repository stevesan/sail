using UnityEngine;
using System.Collections;

public class DragPlane : MonoBehaviour
{
    static float DebugDrawScale = 5;

    public float dragModulus = 0;
    public Rigidbody target;
    public Vector3 localSailNormal;

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
        Vector3 wsNormal = transform.TransformDirection( localSailNormal );

        if( Mathf.Abs(dragModulus) > 0 )
        {
            // We can NOT just use target.velocity here, since we are concerned about our own velocity!
            Vector3 velocity = (transform.position - prevPos) / Time.fixedDeltaTime;
            Vector3 dragForce = -dragModulus * wsNormal * Vector3.Dot(wsNormal, velocity);

            target.AddForceAtPosition( dragForce, transform.position );
            Debug.DrawLine( transform.position, transform.position+dragForce*DebugDrawScale, Color.red );

            prevPos = transform.position;
        }
    }

    void QuickSave(string prefix)
    {
        Utils.SaveVector3( prefix+gameObject.name+".draggerPrevPos", prevPos );
    }

    void QuickLoad(string prefix)
    {
        Utils.LoadVector3( prefix+gameObject.name+".draggerPrevPos", ref prevPos );
    }
}
