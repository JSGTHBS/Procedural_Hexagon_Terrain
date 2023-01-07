using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Mathematics;

namespace JCS.HexagonTerrain
{
    [CreateAssetMenu(menuName = "Hexagon Terrain/Terrain Height Operation")]
    public class HeightOperationAssets : ScriptableObject
    {

        public HeightOperationMode heightOperationMode = HeightOperationMode.noise_simplePerlin;

        [Header("Settings for noise operation modes.")]
        public ValueApplyMode valueApplyMode = ValueApplyMode.additive;
        [Tooltip("The left-bottom of the operation area in the terrain.")]
        public float2 startingPercent = float2.zero;
        [Tooltip("The right-top of the operation area in the terrain.")]
        public float2 finishingPercent = new float2(1, 1);

        [Tooltip("Settings special for simple Perlin noise.")]
        public SimplePerlinSettings simplePerlinParameters;

        [Tooltip("Settings special for texture noise.")]
        public NoiseTextureSettings textureParameters;
    }
}
