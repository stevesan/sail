using UnityEngine;
using System.Collections;

public class BoatControls : MonoBehaviour {

    public Transform rudder;
    public HingeJoint mast;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if( Input.GetButtonDown("RudderRight") && (rudder.localEulerAngles.y < 90 || rudder.localEulerAngles.y >= 265) )
            rudder.Rotate( new Vector3(0, 5, 0) );
        else if( Input.GetButtonDown("RudderLeft") && (rudder.localEulerAngles.y > 270 || rudder.localEulerAngles.y <= 95) )
            rudder.Rotate( new Vector3(0, -5, 0) );

        JointLimits limits = mast.limits;

        if( Input.GetButtonDown("SheetIn") )
        {
            limits.min = Mathf.Min( 0, limits.min+5 );
            limits.max = Mathf.Max( 0, limits.max-5 );
        }
        else if( Input.GetButtonDown("SheetOut") )
        {
            limits.min = Mathf.Max( -180, limits.min-5 );
            limits.max = Mathf.Min( 180, limits.max+5 );
        }
        mast.limits = limits;
	
	}
}
