using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NKStudios.VoxelMeshGeneration
{
    public class Chunk<T> : IEnumerable
    {
        public Vector3 position;
        
        public int Width { get; }
        public int Height { get; }

        public int Size => _chunk.Length;

        public T this[int z, int y, int x] => _chunk[z, y, x]; 
        private T[,,] _chunk;

        public Chunk(int width, int height, Vector3 position) {
            Width = width;
            Height = height;
            this.position = position;

            Clear();
        }
        public void Clear() => _chunk = new T[Width, Height, Width];
        public T GetValueAtPos(int x, int y, int z) => _chunk[x, y, z];
        public T GetValueAtPos(Vector3Int position) => GetValueAtPos(position.x, position.y, position.z);
        public void AddAtPos(T valueToAdd, int x, int y, int z) => _chunk[x, y, z] = valueToAdd;

        public void Add(Voxel voxelToAdd) =>
            _chunk[voxelToAdd.Position.x, voxelToAdd.Position.y, voxelToAdd.Position.z] = (T)(object)voxelToAdd;
        

        public void AddAtPos(T valueToAdd, Vector3Int position) =>
            AddAtPos(valueToAdd, position.x, position.y, position.z);

        public IEnumerator GetEnumerator() => new ChunkEnum<T>(_chunk);
        private class ChunkEnum<T> : IEnumerator
        {
            T[,,] _chunk;

            int position = -1;
            int x = -1;
            int y = 0;
            int z = 0;

            public ChunkEnum(T[,,] chunk) {
                _chunk = chunk;
            }

            public bool MoveNext() {
                if (x < _chunk.GetLength(0) - 1)
                    x++;
                else if (y < _chunk.GetLength(1) - 1) {
                    y++;
                    x = 0;
                } else if (z < _chunk.GetLength(2) - 1) {
                    z++;
                    y = 0;
                    x = 0;
                }

                position++;
                return (position < _chunk.Length);
            }

            public void Reset() {
                position = -1;
                x = -1;
                y = 0;
                z = 0;
            }

            object IEnumerator.Current => Current;

            public T Current {
                get {
                    try {
                        return _chunk[x, y, z];
                    } catch (System.Exception e) {
                        throw e;
                    }
                }
            }
        }
    }
}