using UnityEngine;
using System.Collections;

public class Buoy : MonoBehaviour
{
    public Rigidbody target;
    public float length = 1;    // how much this buoy sticks into the water, downward
    public float maxMag = 10;
    public AudioClip splashSound;

    //----------------------------------------
    //  Private state
    //----------------------------------------
    private bool wasInAir = false;

    void Awake()
    {
        if( target == null )
            target = Utils.FindComponentUpward<Rigidbody>(gameObject);
    }

    void FixedUpdate()
    {
        if( target != null )
        {
            float surfaceY = Lake.main.GetHeightAt(transform.position);
            float submergedFraction = Mathf.Clamp01( Utils.Unlerp( surfaceY, surfaceY-length, transform.position.y ) );
            target.AddForceAtPosition( Vector3.up*submergedFraction*maxMag, transform.position );

            if( submergedFraction <= 0.01 && !wasInAir )
                // left air
                wasInAir = true;
            else if( submergedFraction >= 0.05 && wasInAir )
            {
                // splash down!
                wasInAir = false;
                if( splashSound != null )
                    AudioSource.PlayClipAtPoint( splashSound, transform.position );
            }

        }
    }
}
