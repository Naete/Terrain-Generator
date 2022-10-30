using UnityEngine;
using Random = UnityEngine.Random;

namespace NKStudios.ProceduralAlgorithm {
    public static class DiamondSquare {

        private static float[,] _heightMap;
        
        private static float GetHeigthAt(int x, int z) => _heightMap[x, z];
        private static void ApplyHeight(float height, int x, int z) => _heightMap[x, z] = height;
        private static float GetRandomHeigth(float randomFactor) => Random.Range(-1f, 1f) * randomFactor;

        public static float[,] GenerateHeightMap(int exponentialSize, float noiseIntensity, int seed = 0)
        {
            Random.InitState(seed);
            
            int mapWidth = (int) Mathf.Pow(2, exponentialSize) + 1;
            _heightMap = new float[mapWidth, mapWidth];
            int offset = --mapWidth; // Subtract by 1 to avoid OutOfBoundsException

            InitializeCorners(mapWidth, noiseIntensity);

            while (offset > 1) {
                SquareStep(mapWidth, offset, noiseIntensity);
                DiamondStep(mapWidth, offset, noiseIntensity);
                offset /= 2;
                noiseIntensity /= 2;
            }

            return _heightMap;
        }

        private static void InitializeCorners(int mapWidth, float noiseIntensity) 
        {
            ApplyHeight(GetRandomHeigth(noiseIntensity), 0, 0);
            ApplyHeight(GetRandomHeigth(noiseIntensity), mapWidth, 0);
            ApplyHeight(GetRandomHeigth(noiseIntensity), 0, mapWidth);
            ApplyHeight(GetRandomHeigth(noiseIntensity), mapWidth, mapWidth);
        }

        private static void SquareStep(int mapWidth, int offset, float noiseIntensity) {
            for (int z = 0; z < mapWidth; z += offset) {
                for (int x = 0; x < mapWidth; x += offset) {
                    float height = ComputeSquareHeight(offset, x, z, noiseIntensity);
                    ApplyHeight(height, x + (offset / 2), z + (offset / 2));
                }
            }
        }

        private static void DiamondStep(int mapWidth, int offset, float noiseIntensity) 
        {
            int xStart = 0;

            for (int z = 0, count = 0; z <= mapWidth; z += offset / 2) {
                for (int x = xStart; x < mapWidth; x += offset) {
                    float height = ComputeDiamondHeigth(mapWidth, offset, x, z, noiseIntensity);
                    ApplyHeight(height, x + (offset / 2), z);
                }
                
                count++;
                // If count is odd, then shift x to the left (subtract) by half size of offset instead of 0.
                xStart = (count % 2 == 1) ? -(offset / 2) : 0;
            }
        }

        private static float ComputeSquareHeight(int offset, int x, int z, float noiseIntensity) 
        {
            float result = GetHeigthAt(x, z);
            result += GetHeigthAt(x + offset, z);
            result += GetHeigthAt(x, z + offset);
            result += GetHeigthAt(x + offset, z + offset);

            return (result / 4) + GetRandomHeigth(noiseIntensity);
        }

        private static float ComputeDiamondHeigth(int mapWidth, int offset, int x, int z, float noiseIntensity) 
        {
            int count = 0;
            float result = 0;
            int halfOffset = offset / 2;
            
            if (x >= 0) {
                result += GetHeigthAt(x, z); // Left
                count++;
            }

            if (x + offset <= mapWidth) {
                result += GetHeigthAt(x + offset, z); // Right
                count++;
            }

            if (z - halfOffset >= 0) {
                result += GetHeigthAt(x + halfOffset, z - halfOffset); // Bottom
                count++;
            }

            if (z + halfOffset <= mapWidth) {
                result += GetHeigthAt(x + halfOffset, z + halfOffset); // Top
                count++;
            }

            return (result / count) + GetRandomHeigth(noiseIntensity);
        }
    }
}