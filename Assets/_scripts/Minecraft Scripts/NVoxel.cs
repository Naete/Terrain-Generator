using UnityEngine;

public struct NVoxel {
    public Vector3Int Position;
    private int x => Position.x;
    private int y => Position.y;
    private int z => Position.z;

    public NBlockType Type;

    public Vector3Int[] vertices => new Vector3Int[8] {
        new Vector3Int(x, y, z + 1),
        new Vector3Int(x, y + 1, z + 1),
        new Vector3Int(x + 1, y + 1, z + 1),
        new Vector3Int(x + 1, y, z + 1),
        new Vector3Int(x, y, z),
        new Vector3Int(x, y + 1, z),
        new Vector3Int(x + 1, y + 1, z),
        new Vector3Int(x + 1, y, z),
    };

    public int[,] faces => new int[,] {
        {3, 2, 1, 0},
        {7, 6, 2, 3},
        {4, 5, 6, 7},
        {0, 1, 5, 4},
        {5, 1, 2, 6},
        {0, 4, 7, 3},
    };
    
    public int[,] triangles => new int[,] {
        {3, 2, 1, 3, 1, 0},
        {7, 6, 2, 7, 2, 3},
        {4, 5, 6, 4, 6, 7},
        {0, 1, 5, 0, 5, 4},
        {5, 1, 2, 5, 2, 6},
        {0, 4, 7, 0, 7, 3},
    };
    
    public NVoxel(Vector3Int position, NBlockType type) {
        Position = position;
        Type = type;
    }
}

public enum NBlockType
{
    Empty, Dirt, Grass, Stone, Snow, Water
}