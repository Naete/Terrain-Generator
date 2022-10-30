namespace NKStudios.ProceduralAlgorithm {
    public static class GaussianBlur {

        private static int _mapLength;
        private static int _mapHeigth;
        
        private static readonly int[,] _gaussMultipliers = {
                { 1, 2, 1},
                { 2, 4, 2},
                { 1, 2, 1}
        };

        public static float[,] ApplyBlur(float[,] map, int repeatAmount = 1) 
        {
            _mapLength = map.GetLength(0);
            _mapHeigth = map.GetLength(1);
            
            for (int i = 0; i < repeatAmount; i++) {
                for (int z = 0; z < _mapLength; z++) {
                    for (int x = 0; x < _mapHeigth; x++) 
                    {
                        map[z, x] = ComputeBlur(z, x, map);
                    }
                }
            }

            return map;
        }

        private static float ComputeBlur(int z, int x, float[,] map) 
        {
            float sum = 0;
            int count = 0;

            // TODO: Remove "3" and use dynamic variable
            for (int mapZ = z - 1, gaussZ = 0; mapZ <= z + 1 && gaussZ < 3; mapZ++, gaussZ++) {
                for (int mapX = x - 1, gaussX = 0; mapX <= x + 1 && gaussX < 3; mapX++, gaussX++) 
                {
                    // Checks if neighbour is not outside maps boundaries
                    if (!(mapZ < 0 || mapZ >= _mapLength || mapX < 0 || mapX >= _mapHeigth)) {
                        sum += map[mapZ, mapX] * _gaussMultipliers[gaussZ, gaussX];
                        count += _gaussMultipliers[gaussX, gaussZ];
                    }
                }
            }

            return sum / count;
        }
    }
}