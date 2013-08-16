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
    public Transform nextToTarget;

    public float sheetInLength = 0f;

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

    void LateUpdate()
    {
        Vector3 toTarget = target.position - transform.position;
        float targetReach = toTarget.magnitude;

        float maxReach = segments.Count * segLength - sheetInLength;
        int numSegsUsed = Mathf.FloorToInt( maxReach/(segments.Count*segLength) * segments.Count );
        float numCyclesFloat = numSegsUsed*1f / segsPerCycle;
        float realisticReach = Mathf.Min( maxReach, targetReach );

        Debug.DrawLine( transform.position + transform.up*0.5f,
                transform.position + toTarget.normalized*maxReach + transform.up*0.5f, Color.red );
        Debug.Log("maxReach = "+maxReach);

        float frequency = 1f;

        if( realisticReach > 1e-4 )
            frequency = numCyclesFloat / realisticReach;
        else
            frequency = 0f;

        float spanFraction = Mathf.Clamp01( realisticReach/maxReach );
        float amplitude = Mathf.Lerp( segsPerCycle*segLength/4f, 0, spanFraction );

        // compute a perpendicular axis
        Vector3 zAxis = toTarget.normalized;
        Vector3 yAxis = Vector3.Cross( zAxis, (nextToTarget.position-target.position).normalized ).normalized;
        Vector3 xAxis = Vector3.Cross( yAxis, zAxis );

        Debug.DrawLine( transform.position, transform.position+xAxis, Color.red );
        Debug.DrawLine( transform.position, transform.position+yAxis, Color.green );
        Debug.DrawLine( transform.position, transform.position+zAxis, Color.blue );

        for( int i = 0; i < segments.Count; i++ )
        {
            if( i > numSegsUsed )
            {
                segments[i].SetActive(false);
                continue;
            }

            float localZ = (i+0.5f) * realisticReach/numSegsUsed;

            float localY = amplitude * Mathf.Sin( 2 * Mathf.PI * frequency * localZ );
            // err.. I'm not sure why this derivative is divided by root 2....
            float deriv = amplitude * Mathf.Cos( 2 * Mathf.PI * frequency * localZ ) * 2f*Mathf.PI*frequency;

            Vector3 segPos = transform.position
                + localZ * zAxis
                + localY * yAxis;

            Transform segTrans = segments[i].transform;
            segTrans.position = segPos;
            segTrans.LookAt(target.position, yAxis);
            segTrans.RotateAround( xAxis, -Mathf.Atan(deriv) );
            //Debug.DrawLine(segTrans.position, segTrans.position+xAxis, Color.green);
            segments[i].SetActive(true);
        }
    }


}
