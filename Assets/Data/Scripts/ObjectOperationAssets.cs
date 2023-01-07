using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace JCS.HexagonTerrain
{
    [CreateAssetMenu(menuName = "Hexagon Terrain/Terrain Object Operations")]
    public class ObjectOperationAssets : ScriptableObject
    {
        [Header("Specifications")]
        public string operationName;

        [Header("Area")]
        public float2 startingPercent = float2.zero;
        public float2 finishingPercent = new float2(1, 1);

        [Header("Height")]
        public bool relevantToHeight = false;
        [Tooltip("Min Apparance Height")]
        [Range(0f, 1f)]
        public float minHeight = 0f;
        [Tooltip("Max Apparance Height")]
        [Range(0f, 1f)]
        public float maxHeight = 1f;

        [Header("Texture Noise")]
        public bool relevantToNoise = false;
        public NoiseTextureSettings nTexSettings;
        [Tooltip("Value above this will appear.")]
        public float threshold = 0.5f;

        [Header("Foundation")]
        public bool manipulateFoundation = false;
        public List<TerrainObjectVariant> foundationVariants;
        public bool replaceFoundation = false;
        public bool lockFoundation = false;

        [Header("Surface")]
        public bool manipulateSurface = false;
        public List<TerrainObjectVariant> surfaceVariants;
        public bool replaceSurface = false;
        public bool lockSurface = false;

        [Header("Air")]
        public bool manipulateAir = false;
        public List<TerrainObjectVariant> airVariants;
        public bool replaceAir = false;
        public bool lockAir = false;

        [Header("Effect")]
        public bool manipulateEffect = false;
        public List<TerrainObjectVariant> effectVariants;
        public bool replaceEffect = false;
        public bool lockEffect = false;

    }
}
