using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace JCS.HexagonTerrain
{

    public enum HeightOperationMode
    {
        // Shall all be catched by the Terrain Manager Height Generation functions.

        // Noise Operation.
        noise_simplePerlin,
        noise_texture,

        // Adjustment Operation.
        adjustment_saturate,
        adjustment_remap,
    }



    public enum ValueApplyMode
    {
        additive, // a + b.
        multiply, // a * b.
        average, // (a + b) / 2
    }

    public enum TextureWrapMode
    {
        // tiling up.
        tiling,
        // auto scale up or down to to fill the whole area.
        stretch, 
    }


    [Serializable]
    public class SimplePerlinSettings
    {
        // Sample usage: perlin((x * xscaler + xoffset), (y * yscaler + yoffset)) * hscaler + hoffset).
        // Offset moves the sample area, scaler controls sample step size.
        public bool randomOffset = false;
        public float xOffset = 0f;
        public float xScaler = 1f;
        public float yOffset = 0f;
        public float yScaler = 1f;
        public float hOffset = 0;
        public float hScaler = 1f;

    }


    [Serializable]
    public class NoiseTextureSettings
    {
        public Texture2D noiseTexture;
        public NoiseChannel channel = NoiseChannel.red;
        public TextureWrapMode wrapMode = TextureWrapMode.tiling;
        
        [Header("Settings only for tiling.")]
        public int2 offset = int2.zero;
        public bool randomOffset = false;
        public float scale = 1;

        public enum NoiseChannel { red, green, blue, alpha }
    }

    [Serializable]
    public class TerrainObjectVariant
    {
        public TerrainObject objectPrefab;
        public int weight = 1;
    }

}
