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
}
