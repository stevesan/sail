using UnityEngine;
using System.Collections;

public class UnsmoothGrid : MonoBehaviour
{
    public int res = 100;
    public float displacementMag;

    // for LOD border fitting
    float leadershipValue;
    Vector3[] verts;
    Vector3[,] uniqueVerts;
    bool inited = false;

    void UpdateMeshVertices()
    {
        if( verts == null )
            verts = new Vector3[ GetNumTriVerts() ];

        // update border verts
        for( int i = 0; i < res; i++ )
        for( int j = 0; j < res; j++ )
        {
            // TODO only do borders
            verts[ GetTriVertId(i, j, 0, 0) ] = uniqueVerts[ i, j ];
            verts[ GetTriVertId(i, j, 0, 2) ] = uniqueVerts[ i, j+1 ];
            verts[ GetTriVertId(i, j, 0, 1) ] = uniqueVerts[ i+1, j+1 ];

            verts[ GetTriVertId(i, j, 1, 0) ] = uniqueVerts[ i, j ];
            verts[ GetTriVertId(i, j, 1, 2) ] = uniqueVerts[ i+1, j+1 ];
            verts[ GetTriVertId(i, j, 1, 1) ] = uniqueVerts[ i+1, j ];
        }
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.vertices = verts;
    }

    public int GetUniqueVertId( int i, int j )
    {
        return (res+1) * i + j;
    }

    public int GetTriVertId( int i, int j, int half, int vert )
    {
        return 3*(2*(res*i + j) + half) + vert;
    }

    public int GetNumTriVerts()
    {
        return 3*2*res*res;
    }

	// Use this for initialization
	void Awake()
    {
        leadershipValue = Random.value;

        uniqueVerts = new Vector3[ res+1, res+1 ];

        for( int i = 0; i < res+1; i++ )
        for( int j = 0; j < res+1; j++ )
        {
            float x = j*1f/res-0.5f;
            float z = i*1f/res-0.5f;
            uniqueVerts[ i, j ] = new Vector3( x,
                    Mathf.Lerp(-1, 1, Random.value)*displacementMag, z );
        }

        // Build mesh

        // clear mesh first
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.triangles = null;

        UpdateMeshVertices();

        int[] tris = new int[ GetNumTriVerts() ];
        Vector2[] uvs = new Vector2[ GetNumTriVerts() ];

        for( int i = 0; i < GetNumTriVerts(); i++ )
        {
            tris[i] = i;
            uvs[i] = new Vector2( verts[i].x+0.5f, verts[i].z+0.5f );
        }

        mesh.uv = uvs;
        mesh.triangles = tris;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        inited = true;
	}

    // returns the vertex on the border, mapping k to the north-east-going direction
    Vector3 GetBorderVertex( int dir, int k )
    {
        Utils.Assert( dir < 4 );
        Utils.Assert( k < res+1 );
        Utils.Assert( k >= 0 );

        if( dir == 0 )
            return uniqueVerts[ res, k ];
        else if( dir == 1 )
            return uniqueVerts[ k, res ];
        else if( dir == 2 )
            return uniqueVerts[ 0, k ];
        else
            return uniqueVerts[ k, 0 ];
    }

    void SetBorderVertex( int dir, int k, Vector3 v )
    {
        Utils.Assert( dir < 4 );
        Utils.Assert( k < res+1 );

        if( dir == 0 )
            uniqueVerts[ res, k ] = v;
        else if( dir == 1 )
            uniqueVerts[ k, res ] = v;
        else if( dir == 2 )
            uniqueVerts[ 0, k ] = v;
        else
            uniqueVerts[ k, 0 ] = v;
    }

    void OnLODStateChanged( LODGrid.OnLODStateChangedParms p )
    {
        if( !inited )
            return;

        // Match all border verts to leading neighbors
        for( int dir = 0; dir < 4; dir++ )
        {
            int ni = p.i + Utils.DirToRowOffset[dir];
            int nj = p.j + Utils.DirToColOffset[dir];
            UnsmoothGrid nbor = p.grid.GetCellAs<UnsmoothGrid>(ni, nj);

            int oppDir = (dir + 2) % 4;

            if( nbor != null && nbor.leadershipValue > this.leadershipValue )
            {
                for( int k = 0; k < res+1; k++ )
                {
                    Vector3 nborVert = nbor.GetBorderVertex( oppDir, k );
                    Vector3 myVert = GetBorderVertex( dir, k );
                    myVert.y = nborVert.y;
                    SetBorderVertex( dir, k, myVert );
                }
            }
        }

        UpdateMeshVertices();
    }
}
