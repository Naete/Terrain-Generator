using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using NKStudios.ProceduralAlgorithm;
using UnityEngine;

public class TerrainController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField, Range(1, 100)] private int renderDistance;
    [SerializeField] private int seed;
    [SerializeField, Range(1, 10)] private int exponentialMapSize;
    [SerializeField] private int noiseIntensity;
    
    private NChunk[,] _loadedChunks;

    private const int CHUNK_WIDTH = 16;
    private const int CHUNK_HEIGHT = 256;

    private void Awake() 
    {
        _loadedChunks = new NChunk[renderDistance, renderDistance];
    }

    private void Start() 
    {
        InitializeChunks();

        foreach (NChunk chunk in _loadedChunks)
        {
            chunk.UpdateMesh();
        }
    }

    private void Update()
    {
        
    }

    private void InitializeChunks()
    {
        float[,] heightMap = DiamondSquare.GenerateHeightMap(exponentialMapSize, noiseIntensity, seed);
        heightMap = GaussianBlur.ApplyBlur(heightMap, 5);

        for (int z = 0; z < renderDistance; z++) {
            for (int x = 0; x < renderDistance; x++) {
                
                // Create new chunk
                var newChunk = new NChunk(CHUNK_WIDTH, CHUNK_HEIGHT, new Vector3Int(x * CHUNK_WIDTH, 0, z * CHUNK_WIDTH), this.transform);
                _loadedChunks[x, z] = newChunk;
                
                for (int vZ = 0; vZ < CHUNK_WIDTH; vZ++) {
                    for (int vX = 0; vX < CHUNK_WIDTH; vX++) {
                        
                        // TODO: Add cancel condition if v_X + (x * CHUNK_WIDTH) and v_Z + (z * CHUNK_WIDTH) are not inside heightmap
                        // Calculate y-value of voxel
                        int y = Mathf.Abs((int)heightMap[vX + (x * CHUNK_WIDTH), vZ + (z * CHUNK_WIDTH)]);
                        
                        // Add voxels to chunk
                        _loadedChunks[x, z].Add(new NVoxel(new Vector3Int(vX, y, vZ), NBlockType.Grass));
                    }
                }
            }
        }
    }

    private void LoadChunk()
    {
        
    }

    private void UnloadChunk()
    {
        
    }
}