using System.Collections.Generic;
using UnityEngine;
using NKStudios.VoxelMeshGeneration;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GreedyMeshingTest : MonoBehaviour
{
    private Mesh _mesh;
    private List<Vector3> _verticesList;
    private List<int> _triangleList;
    
    private void Awake() {
        _mesh = new Mesh();
        _verticesList = new List<Vector3>();
        _triangleList = new List<int>();
        GetComponent<MeshFilter>().mesh = _mesh;
    }

    private void Start() {

        #region VoxelList
        // Voxel[,] chunk = new Voxel[4,4];
        //
        // chunk[0,0] = new Voxel(0, 0, 0, BlockType.Grass);
        // chunk[0,1] = new Voxel(1, 0, 0, BlockType.Grass);
        // chunk[0,2] = new Voxel(2, 0, 0, BlockType.Grass);
        // chunk[0,3] = new Voxel(3, 1, 0, BlockType.Grass);
        // chunk[1,0] = new Voxel(0, 0, 1, BlockType.Grass);
        // chunk[1,1] = new Voxel(1, 0, 1, BlockType.Grass);
        // chunk[1,2] = new Voxel(2, 1, 1, BlockType.Grass);
        // chunk[1,3] = new Voxel(3, 0, 1, BlockType.Grass);
        // chunk[2,0] = new Voxel(0, 1, 2, BlockType.Grass);
        // chunk[2,1] = new Voxel(1, 1, 2, BlockType.Grass);
        // chunk[2,2] = new Voxel(2, 0, 2, BlockType.Grass);
        // chunk[2,3] = new Voxel(3, 1, 2, BlockType.Grass);
        // chunk[3,0] = new Voxel(0, 1, 3, BlockType.Grass);
        // chunk[3,1] = new Voxel(1, 1, 3, BlockType.Grass);
        // chunk[3,2] = new Voxel(2, 0, 3, BlockType.Grass);
        // chunk[3,3] = new Voxel(3, 1, 3, BlockType.Grass);
        //
        // foreach (Voxel voxel in chunk) {
        //     if (voxel.Type != BlockType.Air) {
        //         // Add vertices
        //         foreach (Vector3 vertex in voxel.vertices) {
        //             _verticesList.Add(vertex);
        //         }
        //     }
        // }
        //
        // ExecuteGreedyMeshing(chunk, 4); // 4 stands for top face
        #endregion
        
        #region ChunkClass

        // Initialize chunk
        var chunk = new Chunk<Voxel>(4, 256, Vector3.zero);
        
        // Add voxels
        chunk.Add(new Voxel(0,0,0, BlockType.Grass));
        chunk.Add(new Voxel(1,0,0, BlockType.Grass));
        chunk.Add(new Voxel(2,0,0, BlockType.Grass));
        chunk.Add(new Voxel(3,1,0, BlockType.Grass));
        chunk.Add(new Voxel(0,0,1, BlockType.Grass));
        chunk.Add(new Voxel(1,0,1, BlockType.Grass));
        chunk.Add(new Voxel(2,1,1, BlockType.Grass));
        chunk.Add(new Voxel(3,0,1, BlockType.Grass));
        chunk.Add(new Voxel(0,1,2, BlockType.Grass));
        chunk.Add(new Voxel(1,1,2, BlockType.Grass));
        chunk.Add(new Voxel(2,0,2, BlockType.Grass));
        chunk.Add(new Voxel(3,1,2, BlockType.Grass));
        chunk.Add(new Voxel(0,1,3, BlockType.Grass));
        chunk.Add(new Voxel(1,1,3, BlockType.Grass));
        chunk.Add(new Voxel(2,0,3, BlockType.Grass));
        chunk.Add(new Voxel(3,1,3, BlockType.Grass));
        
        // Add vertices
        foreach (Voxel voxel in chunk) {
            if (voxel.Type != BlockType.Air) {
                foreach (Vector3 vertex in voxel.vertices) {
                    _verticesList.Add(vertex);
                }
            }
        }
        
        ExeGM(chunk, 4);
        
        #endregion
        
        UpdateMesh();
    }

    private void ExeGM(Chunk<Voxel> chunk, int direction)
    {
        
    }

    private void ExecuteGreedyMeshing(Voxel[,] chunk, int direction) {
        
        bool[,] isVisited = new bool[chunk.GetLength(0), chunk.GetLength(1)]; // Stores TRUE/FALSE at coordinates of visited voxels
        
        for (int z = 0; z < chunk.GetLength(0); z++) {
            for (int x = 0; x < chunk.GetLength(1); x++) {
                
                // Check if voxel at [z, x] is NOT visited
                if (!isVisited[z, x]) {
                    
                    Voxel start = chunk[z, x];
                    Voxel end = start;
                    
                    // Run along x-axis of the current voxel
                    for (int xPos = x + 1; xPos < chunk.GetLength(1); xPos++) {
                        
                        Voxel nextVoxel = chunk[z, xPos]; // Get the next voxel to check
                        
                        // Check if startVoxel and nextVoxel NOT have same height or type, ...
                        if (start.Position.y != nextVoxel.Position.y || start.Type != nextVoxel.Type) {
                            // ... when TRUE: Stop loop on x-axis 
                            break;
                        } else {
                            // ... when FALSE: Assign the new voxel as visited
                            isVisited[z, xPos] = true; // Voxel at [z, x] is visited
                            
                            end = nextVoxel;
                        }
                    }
                    
                    for (int zPos = z + 1; zPos < chunk.GetLength(0); zPos++) {
                        
                        Voxel nextVoxel = chunk[zPos, start.Position.x];
                        
                        bool isRowUsable = true;
                        
                        for (int xPos = start.Position.x; xPos <= end.Position.x; xPos++) {
                            
                            nextVoxel = chunk[zPos, xPos];
                            
                            if (start.Position.y != nextVoxel.Position.y || start.Type != nextVoxel.Type) {
                                isRowUsable = false;
                                break;
                            }
                        }
                        
                        if (!isRowUsable) {
                            break;
                        }
                        
                        for (int xPos = 0; xPos <= end.Position.x; xPos++) {
                            isVisited[zPos, xPos] = true;
                        }
                        
                        end = nextVoxel;
                    }
                    
                    Debug.Log("Start: " + start.Position + ", End: " + end.Position);
                    
                    int botLeft = (start.Position.x + start.Position.z * chunk.GetLength(0));
                    int topRight = (end.Position.x + end.Position.z * chunk.GetLength(0));
                    int botRight = botLeft + (end.Position.x - start.Position.x);
                    int topLeft = topRight - (end.Position.x - start.Position.x);

                    botLeft *= 24;
                    topRight *= 24;
                    botRight *= 24;
                    topLeft *= 24;
                    
                    Debug.Log("BotLeft: " + botLeft + ", BotRight: " + botRight + ", TopLeft: " + topLeft + ", TopRight: " + topRight);
                    
                    _triangleList.Add(start.faces[direction, 0] + botLeft);
                    _triangleList.Add(start.faces[direction, 1] + topLeft);
                    _triangleList.Add(end.faces[direction, 2] + topRight);
                    
                    _triangleList.Add(start.faces[direction, 0] + botLeft);
                    _triangleList.Add(end.faces[direction, 2] + topRight);
                    _triangleList.Add(end.faces[direction, 5] + botRight);
                }
            }
        }
    }
    
    private void UpdateMesh() {
        _mesh.Clear();
        _mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        _mesh.vertices = _verticesList.ToArray();
        _mesh.triangles = _triangleList.ToArray();
        
        _mesh.Optimize();
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        _mesh.RecalculateTangents();
    }
}