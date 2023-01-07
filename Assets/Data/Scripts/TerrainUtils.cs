using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering;
using Unity.Mathematics;

namespace JCS.HexagonTerrain
{
    public static class TerrainUtils
    {

        // imperative functions.

        #region Pure Functions
        // Many functions are inspired by shader functions, which are mostly pure. 
        // Optimisation: pure function might cost tremendous lot of memory, when manipulating gigantic arrays, might make in-place changes instead. 

        /// <summary>
        /// Blend the height values of the incoming heightmap with the height values of the current heightmap.
        /// </summary>
        /// <param name="current"> Current heightmap. </param>
        /// <param name="incoming"> Incoming heightmap. </param>
        /// <param name="sIndex"> Starting index on the current heightmap. </param>
        /// <param name="bMode"> Height value application Mode. </param>
        /// <param name="wMode"> Wrap Mode for the incoming heightmap if out of index range. </param>
        /// <returns> The blended heightmap (saturated) with the size of the original (current) heightmap. </returns>
        public static float[,] BlendHeights(float[,] current, float[,] incoming, int2 sIndex, ValueApplyMode bMode, TextureWrapMode wMode)
        {
            float[,] heightmap = current;

            int2 iI = int2.zero; // for caching incoming index.
            int2 wI; // for caching wrapped index.

            int2 tS = new int2(incoming.GetLength(0), incoming.GetLength(1));// for caching incoming heightmap's size.

            for (int x = 0; x < heightmap.GetLength(0); x++)
            {
                for (int y = 0; y < heightmap.GetLength(1); y++)
                {
                    iI.x = x - sIndex.x;
                    iI.y = y - sIndex.y;

                    // out of incoming index range
                    if (iI.x < 0 || iI.x > incoming.GetLength(0) - 1 || iI.y < 0 || iI.y > incoming.GetLength(1) - 1)
                    {
                        switch (wMode)
                        {
                            case TextureWrapMode.tiling:
                                wI = TilingIndexRemap(tS, iI);
                                switch (bMode)
                                {
                                    case ValueApplyMode.additive:
                                        heightmap[x, y] += incoming[wI.x, wI.y];
                                        break;
                                    case ValueApplyMode.multiply:
                                        heightmap[x, y] *= incoming[wI.x, wI.y];
                                        break;
                                    case ValueApplyMode.average:
                                        heightmap[x, y] = (heightmap[x, y] + incoming[wI.x, wI.y]) / 2;
                                        break;
                                }
                                break;
                        }
                    }

                    // within incoming index range.
                    switch (bMode)
                    {
                        case ValueApplyMode.additive:
                            heightmap[x, y] += incoming[iI.x, iI.y];
                            break;
                        case ValueApplyMode.multiply:
                            heightmap[x, y] *= incoming[iI.x, iI.y];
                            break;
                        case ValueApplyMode.average:
                            heightmap[x, y] = (heightmap[iI.x, iI.y] + incoming[iI.x, iI.y]) / 2;
                            break;
                    }
                }
            }

            float[,] sheightmap = SaturateNoiseMap(heightmap);
            return sheightmap;
        }

        



        /// <summary>
        /// Generate a noise map (2D array of floats) using simple Perlin noise.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="size"> Noise map size (x, y). </param>
        /// <param name="rSeed"> Random seed, can be processed before passing in. Add to Offset. </param>
        /// <returns> A noise map created from simple perlin algorithm. </returns>
        public static float[,] GenerateNoiseFromSimplePerlin(SimplePerlinSettings p, int2 size, int rSeed = 0)
        {
            float[,] noiseMap = new float[size.x, size.y];

            int rOffset = p.randomOffset ? rSeed : 0;

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    noiseMap[x, y] = (Mathf.PerlinNoise((x + p.xOffset + rOffset) * p.xScaler, (y + p.yOffset + rOffset) * p.yScaler) + p.hOffset) * p.hScaler;
                }
            }
            return noiseMap;
        }


        /// <summary>
        /// Generate a noise map (2D array of floats) based on a texture.
        /// </summary>
        /// <param name="p"> Noise texture parameters. </param>
        /// <param name="size"> The size of the returned noise map. </param>
        /// <param name="rSeed"> Random seed, add to texture coordinate offset; can processed before passing in. </param>
        /// <returns> A noise map of given size. </returns>
        public static float[,] GenerateNoiseFromTexture(NoiseTextureSettings p, int2 size, int rSeed = 0)
        {
            float[,] noiseMap = new float[size.x, size.y];
            
            if (p.noiseTexture == null) { Debug.LogWarning("noiseTexture in a terrain operation has not been assigned; return 0s noise map."); return noiseMap; }

            int2 texCoord = int2.zero;

            int2 textureSize = new int2(p.noiseTexture.width, p.noiseTexture.height);
            int rOffset = p.randomOffset ? rSeed : 0;
            float xstretch = (float)size.x / p.noiseTexture.width;
            float ystretch = (float)size.y / p.noiseTexture.height;

            for (int x = 0; x < noiseMap.GetLength(0); x++)
            {
                for (int y = 0; y < noiseMap.GetLength(1); y++)
                {                    
                    switch (p.wrapMode)
                    {
                        case TextureWrapMode.tiling:
                            texCoord = new int2((int)((x + p.offset.x + rOffset) / p.scale), (int)((y + p.offset.y + rOffset) / p.scale));
                            texCoord = TilingIndexRemap(textureSize, texCoord);
                            break;
                        case TextureWrapMode.stretch:
                            texCoord = new int2((int)(x / xstretch), (int)(y / ystretch));
                            texCoord = ClampIndexRemap(textureSize, texCoord);
                            break;
                    }

                    switch (p.channel)
                    {
                        case NoiseTextureSettings.NoiseChannel.red:
                            noiseMap[x, y] = p.noiseTexture.GetPixel(texCoord.x, texCoord.y).r;
                            break;
                        case NoiseTextureSettings.NoiseChannel.green:
                            noiseMap[x, y] = p.noiseTexture.GetPixel(texCoord.x, texCoord.y).g;
                            break;
                        case NoiseTextureSettings.NoiseChannel.blue:
                            noiseMap[x, y] = p.noiseTexture.GetPixel(texCoord.x, texCoord.y).b;
                            break;
                        case NoiseTextureSettings.NoiseChannel.alpha:
                            noiseMap[x, y] = p.noiseTexture.GetPixel(texCoord.x, texCoord.y).a;
                            break;
                    }
                }
            }
            return noiseMap;
        }


        /// <summary>
        /// Remap current index to an index within the index range.
        /// </summary>
        /// <param name="size"> x, y (not max index, but max index + 1, use Texture2D.width and height) </param>
        /// <param name="index"> The index that might out of index range (can be negative). </param>
        /// <returns> Remapped index. </returns>
        public static int2 TilingIndexRemap(int2 size, int2 index)
        {
            return new int2(
                index.x >= 0 ?
                index.x - (index.x / size.x) * size.x :
                index.x + ((-(index.x + 1) / size.x) + 1) * size.x,

                index.y >= 0 ?
                index.y - (index.y / size.y) * size.y :
                index.y + ((-(index.y + 1) / size.y) + 1) * size.y
                );
        }

        /// <summary>
        /// Clamp the given index to the size of the 2D structure.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int2 ClampIndexRemap(int2 size, int2 index)
        {
            return new int2(
                index.x < 0 ?
                0 :
                index.x >= size.x ?
                size.x - 1:
                index.x,
                
                index.y < 0 ?
                0 :
                index.y >= size.y ?
                size.y -1:
                index.y
                );
        }



        /// <summary>
        /// Saturate noise map values.
        /// </summary>
        /// <param name="current"> The noise map that is to be saturated </param>
        /// <returns> The saturated noise map. </returns>
        public static float[,] SaturateNoiseMap(float[,] current)
        {
            float[,] noiseMap = current;
            for (int x = 0; x < noiseMap.GetLength(0); x++)
            {
                for (int y = 0; y < noiseMap.GetLength(1); y++)
                {
                    noiseMap[x, y] = noiseMap[x, y] > 1 ? 1 : (noiseMap[x, y] < 0 ? 0 : noiseMap[x, y]);
                }
            }
            return noiseMap;
        }

        /// <summary>
        /// Remap the values of the given noise map to the [min, max] section. It is supposed that max > min.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float[,] RemapNoiseMap(float[,] current, float min, float max)
        {

            float[,] heightmap = new float[current.GetLength(0), current.GetLength(1)];

            float cMin = 1f;
            float cMax = 0f;
            foreach (float h in current)
            {
                if (h < cMin) { cMin = h; }
                else if (h > cMax) { cMax = h; }
            }

            float odeno = cMax - cMin;
            float ndeno = max - min;
            for (int x = 0; x < current.GetLength(0); x++)
            {
                for (int y = 0; y < current.GetLength(1); y++)
                {
                    heightmap[x, y] = ((current[x, y] - cMin) / odeno) * ndeno + min;
                }
            }

            return heightmap;
        }

        /// <summary>
        /// Turn percentage position into index position, for a 2D structure (2D array or list), safe.
        /// </summary>
        /// <param name="percentage"> Percentage position. </param>
        /// <param name="size"> The length of the two dimensions of the 2D structure. </param>
        /// <returns> Index position </returns>
        public static int2 PercentageToIndex(float2 percentage, int2 size)
        {
            int2 index = new int2((int)(size.x * percentage.x), (int)(size.y * percentage.y));
            
            if (index.x < 0) { index.x = 0; }
            if (index.y < 0) { index.y = 0; }

            if (index.x >= size.x) { index.x = size.x - 1; }
            if (index.y >= size.y) { index.y = size.y - 1; }

            return index;
        }






        /// <summary>
        /// Perlin with Brownian Motion.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="oct"></param>
        /// <param name="persistance"></param>
        /// <returns></returns>
        public static float fBM(float x, float y, int oct, float persistance)
        {
            float total = 0;
            float frequency = 1;
            float amplitude = 1;
            float maxValue = 0;
            for (int i = 0; i < oct; i++)
            {
                total += Mathf.PerlinNoise(x * frequency, y * frequency) * amplitude;
                maxValue += amplitude;
                amplitude *= persistance;
                frequency *= 2; // This value can be passed in as a parameter. 
            }

            return total / maxValue;
        }

        #endregion

        #region Not-So-Pure Functions


        /// <summary>
        /// Saturate heights of the cell map. In-place change.
        /// </summary>
        /// <param name="cellMap"></param>
        public static void SaturateCellHeights(CellData[,] cellMap)
        {
            foreach(CellData c in cellMap)
            {
                if (c.height < 0) { c.height = 0; }
                else if(c.height > 1) { c.height = 1; }
            }
        }

        /// <summary>
        /// Remap cell height to a certain section [0, 1] by default.
        /// </summary>
        /// <param name="cellMap"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static void RemapCellHeights(CellData[,] cellMap, float min = 0, float max = 1)
        {
            float minH = 1;
            float maxH = 0;

            foreach (CellData c in cellMap)
            {
                if (c.height < minH) { minH = c.height; }
                else if (c.height > maxH) { maxH = c.height; }
            }

            float odeno = maxH - minH;
            float ndeno = max - min;

            foreach (CellData c in cellMap)
            {
                c.height = ((c.height - minH) / odeno) * ndeno + min;
            }
        }


        /// <summary>
        /// Return a reference to a terrain object prefab.
        /// </summary>
        /// <param name="tbVariants"> Terrain object variants list. </param>
        /// <returns></returns>
        public static TerrainObject ChooseATerrainObject(List<TerrainObjectVariant> tbVariants)
        {
            // Use a weighted randomness algorighm.
            TerrainObject tb = null;
            List<WeightedObject<TerrainObject>> wos = new List<WeightedObject<TerrainObject>>();
            int totalWeight = 0;

            foreach(TerrainObjectVariant c in tbVariants)
            {
                totalWeight += c.weight;
                wos.Add(new WeightedObject<TerrainObject>(totalWeight, c.objectPrefab));
            }

            int randWeightPos = UnityEngine.Random.Range(1, totalWeight);

            foreach(WeightedObject<TerrainObject> r in wos)
            {
                if(r.weightPosition >= randWeightPos)
                {
                    tb = r.wObj;
                    break;
                }
            }
            return tb;
        }



        #endregion

    }

    /// <summary>
    /// Used for weighted randomness calculation. 
    /// </summary>
    public struct WeightedObject<T>
    {
        public int weightPosition;
        public T wObj;

        public WeightedObject(int weightPosition, T wObj)
        {
            this.wObj = wObj;
            this.weightPosition = weightPosition;
        }
    }
}