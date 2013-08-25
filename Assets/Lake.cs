using UnityEngine;
using System.Collections;

public class Lake : MonoBehaviour
{
    public static Lake main;
    public float wavesFreq;
    public float wavesAmp;
    public float waveSpeedScale;

    void Awake()
    {
        Utils.Assert( main == null );
        main = this;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public Vector3 GetWaveDirection()
    {
        return Wind.main.force.normalized;
    }

    public float GetWaveOffset()
    {
        float windMag = Wind.main.force.magnitude;
        float waveSpeed = waveSpeedScale * windMag;
        return Time.time * waveSpeed;
    }

    public float GetHeightAt( Vector3 wsPos )
    {
        // waves go in direction of wind, with speed proportional to wind force
        Vector3 origin = GetWaveOffset() * GetWaveDirection();
        float t = Vector3.Dot( wsPos-origin, GetWaveDirection() );
        return transform.position.y
            + wavesAmp * Mathf.Sin( 2*Mathf.PI*wavesFreq * t );
    }
}
