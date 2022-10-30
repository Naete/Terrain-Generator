using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NKStudios.VoxelMeshGeneration
{
    public struct Voxel
    {
        public Vector3Int Position;
        private int x => Position.x;
        private int y => Position.y;
        private int z => Position.z;
        public BlockType Type { get; set; }
        
        public Vector3[] vertices => new Vector3[24] {
            // North Vertexes
            new Vector3(x + 1, y, z + 1),        // 0 - BottomRight
            new Vector3(x + 1, y + 1, z + 1), // 1 - TopRight
            new Vector3(x, y + 1, z + 1),       // 2 - TopLeft
            new Vector3(x, y, z + 1),             // 3 - BottomLeft
            
            // East Vertexes
            new Vector3(x + 1, y, z),               // 4 - BottomRight
            new Vector3(x + 1, y + 1, z),        // 5 - TopRight
            new Vector3(x + 1, y + 1, z + 1), // 6 - TopLeft
            new Vector3(x + 1, y, z + 1),       // 7 - BottomLeft
            
            // South Vertexes
            new Vector3(x, y, z),                      // 8 - BottomLeft
            new Vector3(x, y + 1, z),               // 9 - TopLeft
            new Vector3(x + 1, y + 1, z),        // 10 - TopRight
            new Vector3(x + 1, y, z),              // 11 - BottomRight
            
            // West Vertexes
            new Vector3(x, y, z + 1),              // 12 - BottomLeft
            new Vector3(x, y + 1, z + 1),       // 13 - TopLeft
            new Vector3(x, y + 1, z),             // 14 - TopRight
            new Vector3(x, y, z),                   // 15 - BottomRight
            
            // Top Vertexes
            new Vector3(x, y + 1, z),               // 16 - BottomLeft
            new Vector3(x, y + 1, z + 1),        // 17 - TopLeft
            new Vector3(x + 1, y + 1, z + 1), // 18 - TopRight
            new Vector3(x + 1, y + 1, z),       // 19 - BottomRight
            
            // Bottom Vertexes
            new Vector3(x, y, z + 1),              // 20 - BottomLeft
            new Vector3(x, y, z),                    // 21 - TopLeft
            new Vector3(x + 1, y, z),             // 22 - TopRight
            new Vector3(x + 1, y, z + 1),      // 23 - BottomRight
        };
        
        public int[,] faces => new int[,] {
            {0, 1, 2, 0, 2, 3},          // North
            {4, 5, 6, 4, 6, 7},          // East
            {8, 9, 10, 8, 10, 11},       // South
            {12, 13, 14, 12, 14, 15},    // West
            {16, 17, 18, 16, 18, 19},    // Top
            {20, 21, 22, 20, 22, 23}     // Bottom
        };
        
        public Voxel(int x, int y, int z, BlockType type) {
            Position = new Vector3Int(x, y, z);
            Type = type;
        }
    }
    
    public enum BlockType
    {
        Air, Dirt, Grass, Snow, Stone, Water
    }
}