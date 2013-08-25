using UnityEngine;
using System.Collections;

public class Waves : MonoBehaviour
{
    // The XZ position of the target will be followed
    public Transform followTarget;

    public Transform debugBuoy;

	// Use this for initialization
	void Start()
    {
	
	}

    void Update()
    {
    }

	
	// Update is called once per frame
	void LateUpdate ()
    {
        Vector3 pos = new Vector3( followTarget.position.x, transform.position.y, followTarget.position.z );
        transform.position = pos;

        //----------------------------------------
        //  Update shader properties
        //----------------------------------------
        Vector3 dir = Lake.main.GetWaveDirection();
        float offset = Lake.main.GetWaveOffset();
        renderer.material.SetVector( "Direction", dir );
        renderer.material.SetVector( "Origin", offset*dir-transform.position );
        renderer.material.SetFloat( "Amplitude", Lake.main.wavesAmp );
        renderer.material.SetFloat( "Frequency", Lake.main.wavesFreq );

        if( debugBuoy )
        {
            Vector3 p = debugBuoy.position;
            p.y = Lake.main.GetHeightAt(p);
            debugBuoy.position = p;
        }
	}
}
