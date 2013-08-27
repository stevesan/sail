using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LODGrid : MonoBehaviour
{
    // If not set, Camera.main will be used
    public Transform viewPositionOverride;
    public GameObject nearPrefab;
    public GameObject farPrefab;

    // would recommend keeping local scale to 1, and considering this the world-space size of your cells
    public Vector2 cellSize;    
    public int rows = 10;
    public int cols = 10;
    public int numNearRings;

    public bool debugFreeze;

    GameObject[,] cells;
    bool[,] cellWasNear;
    Queue<GameObject> unusedNearObjs = new Queue<GameObject>();

    private int prevRow = -1;
    private int prevCol = -1;

    void Awake()
    {
        cells = new GameObject[ rows, cols ];
        cellWasNear = new bool[ rows, cols ];

        for( int i = 0; i < rows; i++ )
        for( int j = 0; j < cols; j++ )
        {
            cells[i,j] = null;
            cellWasNear[i,j] = false;
        }
    }

    public bool IsValid( int i, int j )
    {
        return i >= 0 && j >= 0
            && i < rows && j < cols;
    }

    public int GetRow(Vector3 wsPos)
    {
        Vector3 lsPos = transform.InverseTransformPoint(wsPos);
        return Mathf.Clamp(
                Mathf.FloorToInt(lsPos.z / cellSize.y),
                0, rows-1 );
    }

    public int GetCol(Vector3 wsPos)
    {
        Vector3 lsPos = transform.InverseTransformPoint(wsPos);
        return Mathf.Clamp(
                Mathf.FloorToInt(lsPos.x / cellSize.x),
                0, cols-1 );
    }

    public Vector3 GetCellCenter( int i, int j )
    {
        return transform.TransformPoint(
                new Vector3(
                    (j+0.5f) * cellSize.x,
                    0,
                    (i+0.5f) * cellSize.y ));
    }

	// Use this for initialization
	void Start () {
	
	}

    Vector3 GetViewPosition()
    {
        if( viewPositionOverride != null )
            return viewPositionOverride.position;
        else
            return Camera.main.transform.position;
    }

    private void UpdateOldNearCell( int i, int j, int currRow, int currCol )
    {
        if( !IsValid(i,j) )
            return;

        bool stillNear =
            Mathf.Abs(i-currRow) <= numNearRings
            && Mathf.Abs(j-currCol) <= numNearRings;

        if( !stillNear )
        {
            // no longer near. give back its game object
            if( cells[i,j] != null )
            {
                cells[i,j].SetActive(false);
                unusedNearObjs.Enqueue( cells[i,j] );
                cells[i,j] = null;
            }
        }
    }

    void UpdateNewNearCell( int i, int j, int prevRow, int prevCol )
    {
        if( !IsValid(i,j) )
            return;

        bool wasNear = prevRow != -1 && prevCol != -1 
            && Mathf.Abs(i-prevRow) <= numNearRings
            && Mathf.Abs(j-prevCol) <= numNearRings;

        if( !wasNear )
        {
            // is there a free one we can take?
            if( unusedNearObjs.Count > 0 )
            {
                cells[i,j] = unusedNearObjs.Dequeue();
                cells[i,j].SetActive(true);
            }
            else
            {
                cells[i,j] = (GameObject)Instantiate( nearPrefab );
            }

            cells[i,j].transform.position = GetCellCenter(i,j);
            cells[i,j].SendMessage("OnBecameNearLOD", SendMessageOptions.DontRequireReceiver );
        }
    }
	
	// Update is called once per frame
	void Update()
    {
        if( debugFreeze )
            return;

        Vector3 viewPt = GetViewPosition();
        int currRow = GetRow(viewPt);
        int currCol = GetCol(viewPt);
        Debug.Log(currRow+", "+currCol);
        if( prevRow != currRow || prevCol != currCol )
        {
            if( prevRow != -1 && prevCol != -1 )
            {
                for( int di = -numNearRings; di <= numNearRings; di++ )
                for( int dj = -numNearRings; dj <= numNearRings; dj++ )
                    UpdateOldNearCell( prevRow+di, prevCol+dj, currRow, currCol );
            }

            for( int di = -numNearRings; di <= numNearRings; di++ )
            for( int dj = -numNearRings; dj <= numNearRings; dj++ )
                UpdateNewNearCell( currRow+di, currCol+dj, prevRow, prevCol );

            prevRow = currRow;
            prevCol = currCol;
        }
	
	}
}
