using UnityEngine;
using System.Collections;

public class LODGrid : MonoBehaviour
{
    public GameObject nearPrefab;
    public GameObject farPrefab;

    // would recommend keeping local scale to 1, and considering this the world-space size of your cells
    public Vector2 cellSize;    

    public int rows = 10;
    public int cols = 10;
    public int numRingsNear;

    private GameObject[] cells;

    private int prevRow = -1;
    private int prevCol = -1;

    void Awake()
    {
        cells = new GameObject[ rows * cols ];
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

    public GameObject GetCell( int i, int j )
    {
        return cells[ cols*i + j ];
    }

	// Use this for initialization
	void Start () {
	
	}

    Vector3 GetViewPoint()
    {
        return Camera.main.transform.position;
    }

    private void UpdateCell( int i, int j, int ci, int cj )
    {
        bool isNear = Mathf.Abs(i-ci) <= numRingsNear
            && Mathf.Abs(j-cj) <= numRingsNear;

        bool wasNear = GetCell(i,j) != null && GetCell(i,j).activeInHierarchy;

        if( isNear )
        {
        }
    }
	
	// Update is called once per frame
	void Update()
    {
        Vector3 viewPt = GetViewPoint();
        int currRow = GetRow(viewPt);
        int currCol = GetCol(viewPt);
        if( prevRow != currRow || prevCol != currCol )
        {
            if( prevRow != -1 && prevCol != -1 )
            {
                for( int di = -numRingsNear; di <= numRingsNear; di++ )
                for( int dj = -numRingsNear; dj <= numRingsNear; dj++ )
                    UpdateCell( prevRow+di, prevCol+dj, currRow, currCol );
            }

            for( int di = -numRingsNear; di <= numRingsNear; di++ )
            for( int dj = -numRingsNear; dj <= numRingsNear; dj++ )
                UpdateCell( currRow+di, currCol+dj, currRow, currCol );
        }
	
	}
}
