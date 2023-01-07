using System;
using System.Collections.Generic;
using UnityEngine;

namespace JCS.ProceduralHexagonTerrain.v1
{
    /// <summary>
    /// A scriptable object that hosts landform settings.
    /// </summary>
    [CreateAssetMenu(menuName = "Hexagon Terrain/Terrain Form")]
    public class TerrainFormAseets : ScriptableObject
    {
        // for cylinder generation.
        public List<CylinderPrototype> cylinderPrototypes = new List<CylinderPrototype>();
        public ProceduralAlgorithm cylinderAlgorithm;
        public Action<TerrainSector> ApplyHeights;
        public RandomAlgorithmParameters cylinderRandomParameters;
        public PerlinAlgorighmParameters cylinderPerlinParameters;

        // for decoration generation.
        public List<DecorationPrototype> decorationPrototypes = new List<DecorationPrototype>();
        public ProceduralAlgorithm decorationAlgorithm;
        public Action<TerrainSector> ApplyDValues;
        public RandomAlgorithmParameters decorationRandomParameters;
        public PerlinAlgorighmParameters decorationPerlinParameters;

        private void OnEnable()
        {
            switch (cylinderAlgorithm)
            {
                case ProceduralAlgorithm.random:
                    ApplyHeights = (TerrainSector sector) => ProceduralProcess.ApplyRandomHeights(sector, cylinderRandomParameters);
                    break;
                case ProceduralAlgorithm.perlin:
                    ApplyHeights = (TerrainSector sector) => ProceduralProcess.ApplyPerlinHeights(sector, cylinderPerlinParameters);
                    break;
            }

            switch (decorationAlgorithm)
            {
                case ProceduralAlgorithm.random:
                    ApplyDValues = (TerrainSector sector) => ProceduralProcess.ApplyRandomDValue(sector, decorationRandomParameters);
                    break;
                case ProceduralAlgorithm.perlin:
                    ApplyDValues = (TerrainSector sector) => ProceduralProcess.ApplyPerlinDValue(sector, decorationPerlinParameters);
                    break;
            }
        }
    }

    [System.Serializable]
    public class CylinderPrototype
    {
        [Tooltip("Cylinder models")]
        public List<TerrainCylinder> cylinderPrefabs = new List<TerrainCylinder>();

        [Tooltip("Minimun inclusive normalised height that the cylinder will appear.")]
        public float minHeight;
        [Tooltip("Max exclusive normalised height that the cylinder will appear.")]
        public float maxHeight;
    }

    [System.Serializable]
    public class DecorationPrototype
    {
        [Tooltip("Decoration models")]
        public List<TerrainDecoration> decorationPrefabs = new List<TerrainDecoration>();

        [Tooltip("Minimun inclusive normalised height that the decoration will appear.")]
        public float minHeight;
        [Tooltip("Max exclusive normalised height that the decoration will appear.")]
        public float maxHeight;
        [Tooltip("min normalised value that the decoration will appear. The lesser the more likely the decoration appear.")]
        public float threshold;

    }
}