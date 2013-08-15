using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainSheet : MonoBehaviour
{
    public GameObject segmentPrefab;
    public int segsPerCycle = 4;
    public int numCycles = 4;
    public float segLength = 0.1f;
    public Transform target;

    private List<GameObject> segments = new List<GameObject>();

    void Awake()
    {
        Vector3 segPos = transform.position;

        for( int i = 0; i < segsPerCycle*numCycles; i++ )
        {
            segments.Add( (GameObject)GameObject.Instantiate( segmentPrefab, segPos, Quaternion.identity ) );
            segPos += new Vector3( 0, segLength, 0 ); 
        }
    }

    void Update()
    {
        Vector3 delta = target.position - transform.position;
        float targetReach = delta.magnitude;

        int numActiveSegs = Mathf.Min( segments.Count, segsPerCycle * numCycles );

        float maxReach = numActiveSegs * segLength;
        float realisticReach = Mathf.Min( maxReach, targetReach );

        float frequency = 1f;

        if( realisticReach > 1e-4 )
            frequency = numCycles / realisticReach;
        else
            frequency = 0f;

        float spanFraction = Mathf.Clamp01( realisticReach/maxReach );
        float amplitude = Mathf.Lerp( segsPerCycle*segLength/4f, 0, spanFraction );

        // compute a perpendicular axis
        Vector3 yAxis = Vector3.Cross( delta.normalized, transform.forward );
        Vector3 xAxis = delta.normalized;

        for( int i = 0; i < segments.Count; i++ )
        {
            if( i >= numActiveSegs )
            {
                segments[i].SetActive(false);
            }
            else
            {
                float localX = i * realisticReach/numActiveSegs;
                float localY = amplitude * Mathf.Sin( 2 * Mathf.PI * frequency * localX );

                Vector3 segPos = transform.position
                    + localX * xAxis
                    + localY * yAxis;

                segments[i].transform.position = segPos;
                segments[i].SetActive(true);
            }
        }
    }

    void LateUpdate()
    {
    }
}
