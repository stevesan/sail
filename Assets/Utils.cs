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
}
