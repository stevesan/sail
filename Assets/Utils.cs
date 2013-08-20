using UnityEngine;

public class Utils
{
    public static void Assert( bool result, string msg = "see callstack" )
    {
        if( !result )
        {
            Debug.LogError("Assert failed: "+msg);
        }
    }

    public static float Unlerp( float from, float to, float value )
    {
        return Mathf.Clamp01( (value-from)/(to-from) );

    }

    // Like Mathf.Lerp, but not clamped.
    public static float FreeLerp( float from, float to, float t )
    {
        return from + t*(to-from);
    }

    public static C FindComponentUpward<C>( GameObject obj ) where C : Component
    {
        C comp = obj.GetComponent<C>();
        if( comp != null )
            return comp;
        if( obj.transform.parent == null )
            return null;
        return FindComponentUpward<C>( obj.transform.parent.gameObject );
    }

    public static void SaveVector3( string prefix, Vector3 v )
    {
        PlayerPrefs.SetFloat( prefix+".x", v.x );
        PlayerPrefs.SetFloat( prefix+".y", v.y );
        PlayerPrefs.SetFloat( prefix+".z", v.z );
    }

    public static void LoadVector3( string prefix, ref Vector3 v )
    {
        v.x = PlayerPrefs.GetFloat( prefix+".x", v.x );
        v.y = PlayerPrefs.GetFloat( prefix+".y", v.y );
        v.z = PlayerPrefs.GetFloat( prefix+".z", v.z );
    }

    public static void SaveQuaternion( string prefix, Quaternion q )
    {
        PlayerPrefs.SetFloat( prefix+".w", q.w );
        PlayerPrefs.SetFloat( prefix+".x", q.x );
        PlayerPrefs.SetFloat( prefix+".y", q.y );
        PlayerPrefs.SetFloat( prefix+".z", q.z );
    }

    public static void LoadQuaternion( string prefix, ref Quaternion q )
    {
        q.w = PlayerPrefs.GetFloat( prefix+".w", q.w );
        q.x = PlayerPrefs.GetFloat( prefix+".x", q.x );
        q.y = PlayerPrefs.GetFloat( prefix+".y", q.y );
        q.z = PlayerPrefs.GetFloat( prefix+".z", q.z );
    }

    public static void SaveTransform( string prefix, Transform t )
    {
        SaveVector3( prefix+".position", t.position );
        SaveQuaternion( prefix+".rotation", t.rotation );
    }

    public static void LoadTransform( string prefix, Transform t )
    {
        Vector3 pos = t.position;
        Quaternion rot = t.rotation;
        LoadVector3( prefix+".position", ref pos );
        LoadQuaternion( prefix+".rotation", ref rot );
        t.position = pos;
        t.rotation = rot;
    }

    public static void SaveRigidbody( string prefix, Rigidbody rb )
    {
        SaveTransform( prefix+".transform", rb.transform );
        SaveVector3( prefix+".velocity", rb.velocity );
        SaveVector3( prefix+".angularVelocity", rb.angularVelocity );
    }

    public static void LoadRigidbody( string prefix, Rigidbody rb )
    {
        LoadTransform( prefix+".transform", rb.transform );

        Vector3 vel = rb.velocity;
        Vector3 angvel = rb.angularVelocity;
        LoadVector3( prefix+".velocity", ref vel );
        LoadVector3( prefix+".angularVelocity", ref angvel );
        rb.velocity = vel;
        rb.angularVelocity = angvel;
    }
}
