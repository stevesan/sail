using UnityEngine;
using System.Collections;

public class WindSounds : MonoBehaviour
{
    public static Wind main;
    public Transform listener;
    public float perlinPerturbAmp;

    void Update()
    {
        // compute volume dependent on magnitude
        float windMag = Wind.main.force.magnitude;

        // perturb with perlin noise
        float p = Mathf.PerlinNoise( 0, Time.time ) * perlinPerturbAmp;
        audio.volume = Utils.Unlerp( 0, 10, windMag ) + p;
    }

    void LateUpdate()
    {
        // place self in the correct position relative to the player.
        // ie. if wind is coming from North, place ourselves North of player.
        Vector3 offset = -Wind.main.force.normalized * 2;
        transform.position = offset + listener.position;
    }
}
