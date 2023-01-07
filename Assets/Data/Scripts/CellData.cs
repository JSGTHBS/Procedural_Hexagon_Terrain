using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace JCS.HexagonTerrain
{
    /// <summary>
    /// Used to store procedural generation results and instantiation status.
    /// </summary>
    [System.Serializable]
    public class CellData
    {
        // Initial.
        public int2 index;

        // Procedural Assignment.
        public float height;
        public bool heightLocked = false;

        public List<TerrainObjectVariant> foundationVariants;
        public bool foundationAssigned = false;
        public bool foundationLocked = false;

        public List<TerrainObjectVariant> surfaceVariants;
        public bool surfaceAssigned = false;
        public bool surfaceLocked = false;

        public List<TerrainObjectVariant> airVariants;
        public bool airAssigned = false;
        public bool airLocked = false;

        public List<TerrainObjectVariant> effectVariants;
        public bool effectAssigned = false;
        public bool effectLocked = false;

        // Finialising.
        public Vector2 XZPosition;
        public Vector3 groundPosition;
        public Vector3 skyPosition;
        public TerrainObject foundationPrefab; 
        public TerrainObject surfacePrefab;
        public TerrainObject airPrefab;
        public TerrainObject effectPrefab;

        // The instantiation status.
        public bool instantiated = false;
        // The instantiated cell.
        public TerrainObject foundation;
        public TerrainObject surface;
        public TerrainObject air;
        public TerrainObject effect;
    }
}
