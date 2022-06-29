using System.Collections.Generic;
using UnityEngine;

namespace JCS.ProceduralHexagonTerrain
{
    /// <summary>
    /// A scriptable object that hosts terrain region settings.
    /// </summary>
    [CreateAssetMenu(menuName = "Hexagon Terrain/Terrain Region")]
    public class TerrainRegionAssets : ScriptableObject
    {

        [Tooltip("Tile size")]
        public float hexagonSize = 10f;
        
        [Tooltip("Grid size")]
        public int sectorSize = 10;

        [Tooltip("Generate how many generations of neighbour sectors. 0 for self only")]
        public int generationGenerations = 1; // 
        
        [Tooltip("Degeneration when distance is larger than how many times of the sector size")]
        public int degenerationGenerations = 3; 

        public float terrainYOffset = -30f;
        public float terrainYMax = 20f;

        public List<TerrainFormAseets> terrainforms = new List<TerrainFormAseets>();
        public int[,] sectorMap;
        public int startingIndexX;
        public int startingIndexY;
        public int mapScale = 1;

        private void OnEnable()
        {
            SampleSectorMap();
        }

        private void SampleSectorMap()
        {
            sectorMap = new int[10, 10]
            {
            { 0,0,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0,0 },
            { 1,1,1,1,1,1,1,1,1,1 },
            { 1,1,1,1,1,1,1,1,1,1 },
            { 1,1,1,1,1,1,1,1,1,1 },
            { 1,1,1,1,1,1,1,1,1,1 },
            { 1,1,1,1,1,1,1,1,1,1 },
            { 1,1,1,1,1,1,1,1,1,1 },
            { 1,1,1,1,1,1,1,1,1,1 },
            };
        }

        /// <summary>
        /// Return the terrain form for the sector in context.
        /// </summary>
        /// <param name="sector"></param>
        /// <returns></returns>
        public TerrainFormAseets GetSectorForm(TerrainSector sector)
        {
            if (sectorMap is null)
            {
                return terrainforms[0];
            }

            int indexX = sector.gridCentre.x / sectorSize / mapScale + startingIndexX;
            int indexY = sector.gridCentre.y / sectorSize / mapScale + startingIndexY;

            if (indexX >= 0 && indexX < sectorMap.GetLength(0) && indexY >= 0 && indexY < sectorMap.GetLength(1))
            {
                return terrainforms[sectorMap[indexX, indexY]];
            }

            return terrainforms[0];
        }

    }

}


