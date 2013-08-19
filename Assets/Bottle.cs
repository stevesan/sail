
using UnityEngine;
using System.Collections;
using System.IO;

public class Bottle : MonoBehaviour
{
    static int NumExistingBottles = 0;
    static int NumBottlesGot = 0;

    static float GrabDist = 3f;

    public ParticleSystem getFxPrefab;
    public TextMesh distText;
    public string message;

    private int bottleId = -1;

    void Awake()
    {
        bottleId = NumExistingBottles++;
    }

    void Update()
    {
        if( Application.isEditor && Input.GetKeyDown("p") )
        {
            StartCoroutine(OnGetAnimation());
        }

        // when camera is close enough
        float dist = Vector3.Distance( transform.position, Camera.main.transform.position );
       
        if( dist < GrabDist )
            // nab!
            StartCoroutine(OnGetAnimation());
        else if( dist < 20f )
            distText.text = (dist-GrabDist).ToString("0.00") + "m";
        else
            distText.text = "";
    }

    void LateUpdate()
    {
        // keep distance mesh above and unrotated in global position
        distText.transform.position = transform.position + new Vector3(0, 0.5f, 0);
        distText.transform.eulerAngles = new Vector3(0,0,0);
        distText.transform.forward = -1 * (Camera.main.transform.position - transform.position).normalized;

    }

    private float flyYVelocity;
    private float flyYTarget;

    IEnumerator OnGetAnimation()
    {
        Destroy( GetComponent<Rigidbody>() );
        float baseY = transform.position.y;
        float flyStart = Time.time;
        float flyTime = 0.6f;

        while( true )
        {
            float t = Utils.Unlerp( flyStart, flyStart+flyTime, Time.time );

            if( t >= 0.5f )
            {
                if(getFxPrefab != null )
                    Instantiate( getFxPrefab, transform.position, Quaternion.Euler(-90, 0, 0) );
                WriteMessage();
                Destroy(gameObject);
                break;
            }

            Vector3 p = transform.position;
            p.y = baseY + Utils.FreeLerp(0, 2, OutBack(t));
            transform.position = p;
            transform.Rotate( Vector3.up, 360f*4*Time.deltaTime, Space.World );

            yield return null;
        }
    }

    float OutBack( float t )
    {
        float ts = t*t;
        float tc = ts*t;
        return 4*tc + -9*ts + 6*t;
    }

    void WriteMessage()
    {
        StreamWriter writer = new StreamWriter("bottle-message-"+bottleId+".txt");
        writer.WriteLine(message);
        writer.Close();
    }
}
