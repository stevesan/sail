﻿using UnityEngine;
using System.Collections;

public class BoatControls : MonoBehaviour
{
    public static BoatControls main = null;

    const int MaxShifts = 2;
    const float ShiftDist = 0.7f;

    public Transform rudder;
    public Transform till;
    public HingeJoint sailJoint;
    public BoatBody boatBody;
    public Transform player;
    public MainSheet mainSheet;
    public float lenPerSheetChange;
    public float paddlePushForceMagnitude = 1f;
    public float paddlePushLength = 10f;

    [System.Serializable]
    public class SimpleControlSettings
    {
        public float turnTorque = 100f;
    }
    public bool simpleControls = false;
    public SimpleControlSettings simpleSettings;

    public GameObject paddlePushFx;
    public GUITexture pushCrosshair;

    private int xShifts = 0;
    public Utils.SmoothDampedFloat playerX = new Utils.SmoothDampedFloat();

    private int rudderNotch = 0;
    public Utils.SmoothDampedFloat rudderYaw = new Utils.SmoothDampedFloat();

    Vector3 bsAnchorToTargetOrig;
    bool saveQueued = false;
    bool loadQueued = false;
    bool checkpointLoadQueued = false;

    void Awake()
    {
        Utils.Assert( main == null );
        main = this;
    }

    void Start()
    {
        Vector3 wsAnchor = sailJoint.transform.TransformPoint( sailJoint.anchor );
        bsAnchorToTargetOrig = boatBody.transform.InverseTransformDirection( mainSheet.target.position - wsAnchor );
    }

    float GetMaxSheetLengthForAngle( float degs )
    {
        Vector3 wsMastAxis = sailJoint.transform.TransformDirection( sailJoint.axis );
        Vector3 wsAnchorToTarget = boatBody.transform.TransformDirection( bsAnchorToTargetOrig );
        Vector3 wsRotatedAnchorToTarget = Quaternion.AngleAxis( degs, wsMastAxis ) * wsAnchorToTarget;

        Vector3 wsAnchor = sailJoint.transform.TransformPoint( sailJoint.anchor );
        Vector3 wsRotatedTarget = wsAnchor + wsRotatedAnchorToTarget;

        Debug.DrawLine( wsAnchor, wsRotatedTarget, Color.yellow );

        return Vector3.Distance( mainSheet.transform.position, wsRotatedTarget );
    }

    void ReleaseSheetBlock()
    {
        sailJoint.useLimits = false;
        mainSheet.sheetInLength = 0f;

        // so when we re-engage the block, we don't snap to some crazy value
        JointLimits limits = sailJoint.limits;
        limits.min = -180f;
        limits.max = 180f;
        sailJoint.limits = limits;
    }

    void UpdateSimControls()
    {
        if( Input.GetKeyDown("c") && rudderNotch < 8 )
            rudderNotch++;

        if( Input.GetKeyDown("z") && rudderNotch > -8 )
            rudderNotch--;

        rudderYaw.target = rudderNotch * 10;
        rudderYaw.isAngle = true;
        rudderYaw.Update();
        rudder.localEulerAngles = new Vector3( 0, rudderYaw.current, 0 );
        till.localEulerAngles = new Vector3( 0, rudderYaw.current/5f, 0 );

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
        if( Input.GetKeyDown("space") )
            ReleaseSheetBlock();

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

        if( Input.GetKeyDown("d") && xShifts < MaxShifts )
            xShifts++;
        if( Input.GetKeyDown("a") && xShifts > -MaxShifts )
            xShifts--;

        playerX.target = xShifts * ShiftDist;
        playerX.Update();
        player.transform.localPosition = new Vector3(playerX.current, 0, 0);

        //----------------------------------------
        //  Jab the sail left/right
        //----------------------------------------
        float jabMag = 5f;
        if( Input.GetKeyDown("q") )
            sailJoint.rigidbody.AddForceAtPosition( -boatBody.transform.right * jabMag, sailJoint.transform.position );
        if( Input.GetKeyDown("e") )
            sailJoint.rigidbody.AddForceAtPosition( boatBody.transform.right * jabMag, sailJoint.transform.position );
        // We should counter that force on the boat, to avoid a net translational force.
    }

    float limitsMinVelocity;
    float limitsMaxVelocity;

    void UpdateSimpleControls()
    {
        JointLimits limits = sailJoint.limits;

        limits.min = Mathf.SmoothDampAngle( limits.min, -30, ref limitsMinVelocity, 0.1f );
        limits.max = Mathf.SmoothDampAngle( limits.max, 30, ref limitsMaxVelocity, 0.1f );
        sailJoint.useLimits = true;
        sailJoint.limits = limits;

        float maxLen = GetMaxSheetLengthForAngle(limits.min);
        float sheetIn = mainSheet.GetMaxPossibleReach() - maxLen;
        mainSheet.sheetInLength = sheetIn;
    }

	// Update is called once per frame
	void Update ()
    {
        if( simpleControls )
        {
            UpdateSimpleControls();
        }
        else
            UpdateSimControls();

        //----------------------------------------
        //  Capsizing..
        //----------------------------------------
        if( Vector3.Dot(boatBody.transform.up, Vector3.up) < 0.0f )
        {
            if( Input.GetKeyDown("p") )
            {
                // TODO this is fucked 
                // temporarily parent the sail to the boat
                Transform oldSailParent = sailJoint.transform.parent;
                sailJoint.transform.parent = boatBody.transform;
                boatBody.transform.transform.up = Vector3.up;
                sailJoint.transform.parent = oldSailParent;
                ReleaseSheetBlock();

                BroadcastMessage("OnCapsizeRecover", sailJoint, SendMessageOptions.DontRequireReceiver);
                BroadcastMessage("OnCapsizeRecover", boatBody, SendMessageOptions.DontRequireReceiver);

                boatBody.rigidbody.velocity = Vector3.zero;
                sailJoint.rigidbody.velocity = Vector3.zero;

                rudderNotch = 0;
                rudderYaw.SetInstant(0);
                rudder.localEulerAngles = new Vector3( 0, rudderYaw.current, 0 );
            }
        }

        //----------------------------------------
        //  Save/load
        //----------------------------------------
        if( Input.GetKeyDown("7") )
            saveQueued = true;
        else if( Input.GetKeyDown("9") )
            loadQueued = true;
        else if( Input.GetKeyDown("0") )
            checkpointLoadQueued = true;
	}

    public Vector3 GetBoatPosition()
    {
        return boatBody.transform.position;
    }

    public void FixedUpdate()
    {
        if( simpleControls )
        {
            Vector3 boatTorque = Vector3.up;

            int torqueSign = Input.GetKey("a") ? -1 :
                Input.GetKey("d") ? 1 : 0;

            boatBody.rigidbody.AddTorque( Vector3.up * simpleSettings.turnTorque * torqueSign );
        }

        //----------------------------------------
        //  Save/load during fixed update, to play nicely with Physics
        //----------------------------------------
        string userSavePrefix = "user.";
        if( saveQueued )
        {
            QuickSave(userSavePrefix);
        }
        else if( loadQueued )
        {
            QuickLoad(userSavePrefix);
        }
        else if( checkpointLoadQueued )
        {
            QuickLoad("checkpoint");
        }

        saveQueued = false;
        loadQueued = false;
        checkpointLoadQueued = false;

        //----------------------------------------
        //  Poking the shore
        //----------------------------------------
        pushCrosshair.color = new Color(0,0,0,0);
        Ray ray = Camera.main.ViewportPointToRay( new Vector3(0.5f, 0.5f, 0) );
        foreach( RaycastHit hit in Physics.RaycastAll(ray, paddlePushLength) )
        {
            GameObject gob = hit.collider.gameObject;

            if( Utils.FindComponentUpward<Boat>(gob) == null )
            {
                // we CAN push off
                pushCrosshair.color = Color.green;

                if( Input.GetButtonDown("PaddlePush") )
                {
                    // Do push
                    Vector3 force = -ray.direction * paddlePushForceMagnitude;
                    boatBody.rigidbody.AddForceAtPosition( force, ray.origin );

                    GameObject fx = (GameObject)Instantiate(paddlePushFx, hit.point, Quaternion.identity);
                    fx.transform.forward = hit.normal;
                }
                break;
            }
        }
    }

    public void QuickSave(string prefix)
    {
        Debug.Log("quick saving");

        boatBody.gameObject.BroadcastMessage("QuickSave", prefix, SendMessageOptions.DontRequireReceiver );
        sailJoint.gameObject.BroadcastMessage("QuickSave", prefix, SendMessageOptions.DontRequireReceiver );
        Lake.main.QuickSave(prefix);

        PlayerPrefs.SetInt(prefix+"sailUseLimits", sailJoint.useLimits ? 1:0);
        PlayerPrefs.SetFloat(prefix+"sailLimitsMin", sailJoint.limits.min);
        PlayerPrefs.SetFloat(prefix+"sailLimitsMax", sailJoint.limits.max);

        Utils.SaveRigidbody( prefix+"boat", boatBody.rigidbody );
        Utils.SaveTransform( prefix+"rudder", rudder );
        Utils.SaveTransform( prefix+"till", till );
        Utils.SaveRigidbody( prefix+"sail", sailJoint.rigidbody );

        PlayerPrefs.SetFloat(prefix+"sheetInLength", mainSheet.sheetInLength);
        PlayerPrefs.SetInt(prefix+"xShifts", xShifts);
        Utils.SaveTransform( prefix+"player", player.transform );

        PlayerPrefs.SetInt(prefix+"rudderNotch", rudderNotch);

        playerX.ProfileSave(prefix+"playerXDamper");
        rudderYaw.ProfileSave(prefix+"rudderYaw");
    }

    public void QuickLoad(string prefix)
    {
        boatBody.gameObject.BroadcastMessage("QuickLoad", prefix, SendMessageOptions.DontRequireReceiver );
        sailJoint.gameObject.BroadcastMessage("QuickLoad", prefix, SendMessageOptions.DontRequireReceiver );
        Lake.main.QuickLoad(prefix);

        sailJoint.useLimits = PlayerPrefs.GetInt(prefix+"sailUseLimits", sailJoint.useLimits ? 1:0) == 1;
        JointLimits limits = sailJoint.limits;
        limits.min = PlayerPrefs.GetFloat(prefix+"sailLimitsMin", limits.min);
        limits.max = PlayerPrefs.GetFloat(prefix+"sailLimitsMax", limits.max);
        sailJoint.limits = limits;

        Utils.LoadRigidbody( prefix+"boat", boatBody.rigidbody );
        Utils.LoadTransform( prefix+"rudder", rudder );
        Utils.LoadTransform( prefix+"till", till );
        Utils.LoadRigidbody( prefix+"sail", sailJoint.rigidbody );

        mainSheet.sheetInLength = PlayerPrefs.GetFloat(prefix+"sheetInLength", mainSheet.sheetInLength);
        xShifts = PlayerPrefs.GetInt(prefix+"xShifts", xShifts);
        Utils.LoadTransform( prefix+"player", player.transform );

        rudderNotch = PlayerPrefs.GetInt(prefix+"rudderNotch", rudderNotch);

        playerX.ProfileLoad(prefix+"playerXDamper");
        rudderYaw.ProfileLoad(prefix+"rudderYaw");
    }
}
