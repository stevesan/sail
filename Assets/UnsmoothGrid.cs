using UnityEngine;
using System.Collections;

public class UnsmoothGrid : MonoBehaviour
{
    public int rows;
    public int cols;
    public float displacementMag;

    public int GetUniqueVertId( int i, int j )
    {
        return (cols+1) * i + j;
    }

    public int GetTriVertId( int i, int j, int half, int vert )
    {
        return 3*(2*(cols*i + j) + half) + vert;
    }

    public int GetNumTriVerts()
    {
        return 3*2*cols*rows;
    }

	// Use this for initialization
	void Start()
    {
        Vector3[] uniqueVerts = new Vector3[ (cols+1)*(rows+1) ];

        for( int i = 0; i < rows+1; i++ )
        for( int j = 0; j < cols+1; j++ )
        {
            float x = i*1f/rows;
            float z = j*1f/cols;
            uniqueVerts[ i*(cols+1) + j ] = new Vector3( x, Random.value*displacementMag, z );
        }

        Vector3[] verts = new Vector3[ GetNumTriVerts() ];

        for( int i = 0; i < rows; i++ )
        for( int j = 0; j < cols; j++ )
        {
            verts[ GetTriVertId(i, j, 0, 0) ] = uniqueVerts[ GetUniqueVertId( i, j ) ];
            verts[ GetTriVertId(i, j, 0, 1) ] = uniqueVerts[ GetUniqueVertId( i, j+1 ) ];
            verts[ GetTriVertId(i, j, 0, 2) ] = uniqueVerts[ GetUniqueVertId( i+1, j+1 ) ];
            
            verts[ GetTriVertId(i, j, 1, 0) ] = uniqueVerts[ GetUniqueVertId( i, j ) ];
            verts[ GetTriVertId(i, j, 1, 1) ] = uniqueVerts[ GetUniqueVertId( i+1, j+1 ) ];
            verts[ GetTriVertId(i, j, 1, 2) ] = uniqueVerts[ GetUniqueVertId( i+1, j ) ];
        }

        int[] tris = new int[ GetNumTriVerts() ];
        Vector2[] uvs = new Vector2[ GetNumTriVerts() ];

        for( int i = 0; i < GetNumTriVerts(); i++ )
        {
            tris[i] = i;
            uvs[i] = new Vector2( verts[i].x, verts[i].z );
        }

        // clear mesh first
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.triangles = null;

        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.triangles = tris;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
	}
	
	// Update is called once per frame
	void Update()
    {
	
	}
}
