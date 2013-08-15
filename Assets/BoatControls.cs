using UnityEngine;
using System.Collections;

public class BoatControls : MonoBehaviour {

    public Transform rudder;
    public HingeJoint mast;
    public Transform player;

    private int xNudges = 0;
    private int zNudges = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        float rudderDegs = 10;
        if( Input.GetButtonDown("RudderRight") && (rudder.localEulerAngles.y < (90-rudderDegs/2) || rudder.localEulerAngles.y >= (270-rudderDegs/2)) )
            rudder.Rotate( new Vector3(0, rudderDegs, 0) );
        else if( Input.GetButtonDown("RudderLeft") && (rudder.localEulerAngles.y > (270+rudderDegs/2) || rudder.localEulerAngles.y <= (90+rudderDegs/2)) )
            rudder.Rotate( new Vector3(0, -rudderDegs, 0) );

        //----------------------------------------
        //  Sheet/sail control
        //----------------------------------------

        JointLimits limits = mast.limits;

        if( Input.GetButtonDown("SheetIn") )
        {
            mast.useLimits = true;
            limits.min = Mathf.Min( 0, limits.min+5 );
            limits.max = Mathf.Max( 0, limits.max-5 );
        }
        else if( Input.GetButtonDown("SheetOut") )
        {
            mast.useLimits = true;
            limits.min = Mathf.Max( -180, limits.min-5 );
            limits.max = Mathf.Min( 180, limits.max+5 );
        }

        // release the sheet blocker
        // TODO: play sound
        if( Input.GetKeyDown("space") )
        {
            mast.useLimits = false;
            limits.min = -180;
            limits.max = 180;
        }

        mast.limits = limits;

        //----------------------------------------
        //  Player position
        //----------------------------------------
        const int MaxNudges = 2;
        const float nudgeDist = 0.7f;
        /*
        if( Input.GetKeyDown("w") && zNudges < MaxNudges )
        {
            zNudges++;
            player.transform.localPosition += new Vector3(0, 0, nudgeDist);
        }
        if( Input.GetKeyDown("s") && zNudges > -MaxNudges )
        {
            zNudges--;
            player.transform.localPosition -= new Vector3(0, 0, nudgeDist);
        }
        */
        if( Input.GetKeyDown("d") && xNudges < MaxNudges )
        {
            xNudges++;
            player.transform.localPosition += new Vector3(nudgeDist, 0, 0);
        }
        if( Input.GetKeyDown("a") && xNudges > -MaxNudges )
        {
            xNudges--;
            player.transform.localPosition -= new Vector3(nudgeDist, 0, 0);
        }
	
	}
}
