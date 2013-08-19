using UnityEngine;
using System.Collections;

public class BoatControls : MonoBehaviour
{
    public Transform rudder;
    public HingeJoint sailJoint;
    public Transform boatTrans;
    public Transform player;
    public MainSheet mainSheet;
    public float lenPerSheetChange;

    private int xNudges = 0;
    private int zNudges = 0;

    private Vector3 bsAnchorToTargetOrig;

    void Start()
    {
        Vector3 wsAnchor = sailJoint.transform.TransformPoint( sailJoint.anchor );
        bsAnchorToTargetOrig = boatTrans.InverseTransformDirection( mainSheet.target.position - wsAnchor );
    }

    float GetMaxSheetLengthForAngle( float degs )
    {
        Vector3 wsMastAxis = sailJoint.transform.TransformDirection( sailJoint.axis );
        Vector3 wsAnchorToTarget = boatTrans.TransformDirection( bsAnchorToTargetOrig );
        Vector3 wsRotatedAnchorToTarget = Quaternion.AngleAxis( degs, wsMastAxis ) * wsAnchorToTarget;

        Vector3 wsAnchor = sailJoint.transform.TransformPoint( sailJoint.anchor );
        Vector3 wsRotatedTarget = wsAnchor + wsRotatedAnchorToTarget;

        Debug.DrawLine( wsAnchor, wsRotatedTarget, Color.yellow );

        return Vector3.Distance( mainSheet.transform.position, wsRotatedTarget );
    }

	// Update is called once per frame
	void Update ()
    {
        float rudderStep = 10;
        if( Input.GetKeyDown("c") && (rudder.localEulerAngles.y < (90-rudderStep/2) || rudder.localEulerAngles.y >= (270-rudderStep/2)) )
            rudder.Rotate( new Vector3(0, rudderStep, 0) );
        else if( Input.GetKeyDown("z") && (rudder.localEulerAngles.y > (270+rudderStep/2) || rudder.localEulerAngles.y <= (90+rudderStep/2)) )
            rudder.Rotate( new Vector3(0, -rudderStep, 0) );

        //----------------------------------------
        //  Sheet/sail control
        //----------------------------------------

        JointLimits limits = sailJoint.limits;

        float sailStep = 10;
        if( Input.GetKeyDown("s") )
        {
            sailJoint.useLimits = true;
            limits.min = Mathf.Min( 0, limits.min+sailStep );
            limits.max = Mathf.Max( 0, limits.max-sailStep );
        }
        if( Input.GetKeyDown("w") )
        {
            sailJoint.useLimits = true;
            limits.min = Mathf.Max( -180, limits.min-sailStep );
            limits.max = Mathf.Min( 180, limits.max+sailStep );
        }


        // release the sheet blocker
        // TODO: play sound
        if( Input.GetKeyDown("space") )
        {
            sailJoint.useLimits = false;
            mainSheet.sheetInLength = 0f;

            // so when we re-engage the block, we don't snap to some crazy value
            limits.min = -180f;
            limits.max = 180f;
            sailJoint.limits = limits;
        }

        if( sailJoint.useLimits )
        {
            sailJoint.limits = limits;
            // tuck in main sheet proper amount
            float maxLen = GetMaxSheetLengthForAngle(limits.min);
            float sheetIn = mainSheet.GetMaxPossibleReach() - maxLen;
            mainSheet.sheetInLength = sheetIn;
        }

        //----------------------------------------
        //  Player position
        //----------------------------------------
        const int MaxNudges = 2;
        const float nudgeDist = 0.7f;
        /*
           Don't really need back and forth movement
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

        //----------------------------------------
        //  Jab the sail left/right
        //----------------------------------------
        float jabMag = 5f;
        if( Input.GetKeyDown("q") )
            sailJoint.rigidbody.AddForceAtPosition( -boatTrans.right * jabMag, sailJoint.transform.position );
        if( Input.GetKeyDown("e") )
            sailJoint.rigidbody.AddForceAtPosition( boatTrans.right * jabMag, sailJoint.transform.position );


        //----------------------------------------
        //  Capsizing..
        //----------------------------------------
        if( Vector3.Dot(boatTrans.up, Vector3.up) < -0.5f )
        {
            if( Input.GetKeyDown("p") )
            {
                // temporarily parent the sail to the boat
                Transform oldSailParent = sailJoint.transform.parent;
                sailJoint.transform.parent = boatTrans;
                boatTrans.transform.up = Vector3.up;
                sailJoint.transform.parent = oldSailParent;
            }
        }
	}
}
