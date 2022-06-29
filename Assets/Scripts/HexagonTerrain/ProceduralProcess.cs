using UnityEngine;

namespace JCS.ProceduralHexagonTerrain
{
    public static class ProceduralProcess
    {

        #region procedural generation functions
        /// <summary>
        /// Apply random heights. Heights are normalised.
        /// </summary>
        /// <param name="sector"></param>
        /// <param name="p"></param>
        public static void ApplyRandomHeights(TerrainSector sector, RandomAlgorithmParameters p)
        {
            foreach (TerrainCell c in sector.sectorCells)
            {
                c.height = Mathf.Clamp(Random.Range(0, 1f) * p.scaler + p.offset, 0, 1f);
            }

            ApplyHeightSmooth(sector, p.smoothTimes);
        }

        /// <summary>
        /// Apply simple Perlin noise. Heights are normalised.
        /// </summary>
        /// <param name="sector"></param>
        /// <param name="p"></param>
        public static void ApplyPerlinHeights(TerrainSector sector, PerlinAlgorighmParameters p)
        {
            foreach (TerrainCell c in sector.sectorCells)
            {
                float height = Mathf.PerlinNoise(c.gridPosition.x * p.xScaler + p.xOffset, c.gridPosition.y * p.yScaler + p.yOffset) * p.hScaler + p.hOffset;

                c.height = Mathf.Clamp(height, 0, 1f);
            }
        }

        /// <summary>
        /// Apply smooth upon existing heights.
        /// </summary>
        /// <param name="sector"></param>
        /// <param name="times"> Smooth times. </param>
        public static void ApplyHeightSmooth(TerrainSector sector, int times)
        {

            int xIndexMax = sector.sectorCells.GetLength(0) - 1;
            int yIndexMax = sector.sectorCells.GetLength(1) - 1;

            for (int t = 0; t < times; t++)
            {
                for (int x = 0; x < sector.sectorCells.GetLength(0); x++)
                {
                    for (int y = 0; y < sector.sectorCells.GetLength(1); y++)
                    {
                        sector.sectorCells[x, y].height = (sector.sectorCells[x, y].height +
                            sector.sectorCells[Mathf.Clamp(x + 1, 0, xIndexMax), y].height +
                            sector.sectorCells[Mathf.Clamp(x + 1, 0, xIndexMax), Mathf.Clamp(y + 1, 0, yIndexMax)].height +
                            sector.sectorCells[x, Mathf.Clamp(y + 1, 0, yIndexMax)].height +
                            sector.sectorCells[Mathf.Clamp(x - 1, 0, xIndexMax), Mathf.Clamp(y + 1, 0, yIndexMax)].height +
                            sector.sectorCells[Mathf.Clamp(x - 1, 0, xIndexMax), y].height +
                            sector.sectorCells[Mathf.Clamp(x - 1, 0, xIndexMax), Mathf.Clamp(y - 1, 0, yIndexMax)].height +
                            sector.sectorCells[x, Mathf.Clamp(y - 1, 0, yIndexMax)].height +
                            sector.sectorCells[Mathf.Clamp(x + 1, 0, xIndexMax), Mathf.Clamp(y - 1, 0, yIndexMax)].height) / 9;
                    }
                }
            }
        }

        /// <summary>
        /// Apply decoration value to terrain cells, by random algorithm.
        /// </summary>
        /// <param name="sector"></param>
        /// <param name="p"></param>
        public static void ApplyRandomDValue(TerrainSector sector, RandomAlgorithmParameters p)
        {
            foreach (TerrainCell c in sector.sectorCells)
            {
                c.dvalue = Mathf.Clamp(Random.Range(0, 1f) * p.scaler + p.offset, 0, 1f);
            }
        }

        /// <summary>
        /// Apply decoration value to terrain cells, by Perlin algorithm.
        /// </summary>
        /// <param name="sector"></param>
        /// <param name="p"></param>
        public static void ApplyPerlinDValue(TerrainSector sector, PerlinAlgorighmParameters p)
        {
            foreach (TerrainCell c in sector.sectorCells)
            {
                float threshold = Mathf.PerlinNoise(c.gridPosition.x * p.xScaler + p.xOffset, c.gridPosition.y * p.yScaler + p.yOffset) * p.hScaler + p.hOffset;
                c.dvalue = threshold;
            }
        }


        #endregion
    }
    public enum ProceduralAlgorithm
    {
        random,
        perlin,
    }

    [System.Serializable]
    public class RandomAlgorithmParameters
    {
        // Sample usage: y = scaler * x + offset.

        public float offset = 0f;
        public float scaler = 1f;

        public int smoothTimes = 0;

    }

    /// <summary>
    /// Perlin parameters.
    /// </summary>
    [System.Serializable]
    public class PerlinAlgorighmParameters
    {
        // Sample usage: perlin((x * xscaler + xoffset), (y * yscaler + yoffset)) * hscaler + hoffset).
        // Offset moves the sample area, scaler controls sample step size.

        public float xOffset = 0f;
        public float xScaler = 1f;
        public float yOffset = 0f;
        public float yScaler = 1f;
        public float hOffset = 0;
        public float hScaler = 1f;
    }
}