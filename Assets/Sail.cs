using UnityEngine;
using System.Collections;

public class Sail : MonoBehaviour
{
    static float DebugDrawScale = 5;

    public Rigidbody target;

    public float normalModulus = 1;
    public float toMastFraction = 1;
    public Vector3 planeNormal;
    public Vector3 toMast;

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
        Vector3 wsNormal = transform.TransformDirection( planeNormal );
        Vector3 wsToMast = transform.TransformDirection( toMast );

        //----------------------------------------
        //  Wind
        //----------------------------------------
        if( (Mathf.Abs(normalModulus) > 0 )
                && Wind.main != null )
        {
            Vector3 windForce = normalModulus * Utils.Project( Wind.main.force, wsNormal );
            target.AddForceAtPosition( windForce, transform.position );
            Debug.DrawLine( transform.position, transform.position+windForce*DebugDrawScale, Color.blue );

            // Add some in the toMast direction, to fudge things a bit
            Vector3 toMastForce = toMastFraction * windForce.magnitude * wsToMast.normalized;
            target.AddForceAtPosition( toMastForce, transform.position );
            Debug.DrawLine( transform.position, transform.position+toMastForce*DebugDrawScale, new Color(1,0,1) );
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
