using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace JCS.HexagonTerrain
{
    /// <summary>
    /// Region settings for procedural calculations in the HexagonTerrainManager.
    /// </summary>
    [CreateAssetMenu(menuName = "Hexagon Terrain/Terrain Region")]
    public class TerrainRegionAssets : ScriptableObject
    {

        [Header("Terrain Specifications")]
        [Tooltip("(x, y(z)) Better Power of Two")]
        public int2 terrainSize = new int2(64, 32);
        public float hexagonSize = 10f;
        public int2 pivotIndex = new int2(32, 10);
        [Tooltip("Terrain min y value.")]
        public float terrainHeightOffset = -20f;
        [Tooltip("Terrain max y range. Max y value = offset + maxheight")]
        public float terrainMaxHeight = 10f;
        [Tooltip("Percentage sea level in relation to terrain max height")]
        public float seaLevelPercentage = 0.2f;
        [Tooltip("World space sky y value")]
        public float skyLevel = 0f;

        [Header("Operations")]
        [Tooltip("Apply in order")]
        public List<HeightOperationAssets> heightOperations = new List<HeightOperationAssets>();
        [Tooltip("Apply in order")]
        public List<ObjectOperationAssets> objectOperations = new List<ObjectOperationAssets>();

    }
}