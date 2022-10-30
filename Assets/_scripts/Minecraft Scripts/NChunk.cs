using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NChunk
{
    private GameObject _chunkGameObject;
    private MeshFilter _chunkMeshFilter;
    private MeshRenderer _chunkMeshRenderer;
    private MeshCollider _chunkMeshCollider;
    private readonly Vector3Int[] _directions = {
        new Vector3Int(0, 0, 1), // North
        new Vector3Int(1, 0, 0), // East
        new Vector3Int(0, 0, -1), // South
        new Vector3Int(-1, 0, 0), // West
        new Vector3Int(0, 1, 0), // Up
        //new Vector3Int(0, -1, 0), // Down
    };

    public Vector3Int Position;
    
    private NVoxel[,,] _voxelList;
    public NVoxel this[int x, int y, int z] => _voxelList[x, y, z];
    
    public NChunk(int width, int height, Vector3Int position, Transform parent)
    {
        _chunkGameObject = new GameObject("Chunk[X: " + position.x + ", Z: " + position.z + "]");
        _chunkMeshFilter = _chunkGameObject.AddComponent<MeshFilter>();
        _chunkMeshRenderer = _chunkGameObject.AddComponent<MeshRenderer>();
        _chunkMeshCollider = _chunkGameObject.AddComponent<MeshCollider>();
        _chunkGameObject.transform.SetParent(parent);
        
        // TODO: Remove
        _chunkMeshRenderer.material = AssetDatabase.LoadAssetAtPath("Assets/_materials/StoneTerrainShader.mat", typeof(Material)) as Material;

        _chunkGameObject.transform.position = position;
        Position = position;
        
        _voxelList = new NVoxel[width, height, width];
    }

    public void Add(NVoxel voxel) {
        _voxelList[voxel.Position.x, voxel.Position.y, voxel.Position.z] = voxel;
    }

    public void UpdateMesh()
    {
        var verticesList = new List<Vector3>();
        var trianglesList = new List<int>();

        int scale = 0;
        foreach (NVoxel voxel in _voxelList) {
            if (voxel.Type != NBlockType.Empty)
            {
                for (int direction = 0; direction < _directions.Length; direction++)
                {
                    if (IsNeighbourEmptyOf(voxel, direction))
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            int triangleIndex = voxel.triangles[direction, i];
                            verticesList.Add(voxel.vertices[triangleIndex]);
                            trianglesList.Add(scale);

                            scale++;
                        }
                    }
                }
            }
        }
        
        Mesh mesh = new Mesh();
        _chunkMeshFilter.mesh = mesh;
        _chunkMeshCollider.sharedMesh = mesh;

        mesh.Clear();
        mesh.vertices = verticesList.ToArray();
        mesh.triangles = trianglesList.ToArray();

        mesh.RecalculateNormals();
    }

    private bool IsNeighbourEmptyOf(NVoxel voxel, int direction)
    {
        // Neighbours position
        Vector3Int nbPos = voxel.Position + _directions[direction];

        if (nbPos.x < 0 || nbPos.x >= _voxelList.GetLength(0) ||
            nbPos.y < 0 || nbPos.y >= _voxelList.GetLength(1) ||
            nbPos.z < 0 || nbPos.z >= _voxelList.GetLength(2) ||
            _voxelList[nbPos.x, nbPos.y, nbPos.z].Type == NBlockType.Empty)
        {
            return true;
        }

        return false;
    }
}
