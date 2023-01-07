using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System.Runtime.CompilerServices;
using System.Net;

namespace JCS.HexagonTerrain
{
    public class HexagonTerrainManager : MonoBehaviour
    {

        [Header("Specifications")]
        public TerrainRegionAssets region;
        public int seed = 0;

        [Tooltip("If set to false, instantiate all cells at start; if set to true, instantiate the cells around the central object.")]
        public bool proceduralInstantiation = true;
        public int2 proceduralHalfSize = new int2(32, 16);

        [Tooltip("Set runtime or in inspector")]
        public GameObject centralObject;
        public float proceduralCheckInterval = 1f;

        // caches
        private CellData[,] cellMap;
        private List<CellData> activeCells = new List<CellData>();
        [SerializeField] private int2 maxIndex;
        [SerializeField] private int2 cMinIndex;
        [SerializeField] private int2 cMaxIndex;


        void Start()
        {
            PotentialTerrainGeneration();

            if (proceduralInstantiation)
            {
                maxIndex = new int2(region.terrainSize.x - 1, region.terrainSize.y - 1);
                StartCoroutine(TerrainProceduralInstantiationCoroutine());
            }
            else
            {
                TerrainInstantiationAll();
            }
        }

        IEnumerator TerrainProceduralInstantiationCoroutine()
        {
            while (true)
            {
                TerrainInstantiationProcedural();
                yield return new WaitForSeconds(proceduralCheckInterval);
            }
        }

        private void TerrainInstantiationProcedural()
        {

            int2 centralIndex = HexagonIndexUtils.PositionToIndex(new Vector2(centralObject.transform.position.x, centralObject.transform.position.z), region.pivotIndex, region.hexagonSize);

            if (centralIndex.x > maxIndex.x) { Debug.Log("Run out of terrain, x axis"); return; }
            if (centralIndex.y > maxIndex.y) { Debug.Log("Run out of terrain, y axis"); return; }

            cMinIndex = new int2(centralIndex.x - proceduralHalfSize.x, centralIndex.y - proceduralHalfSize.y);
            cMaxIndex = new int2(centralIndex.x + proceduralHalfSize.x, centralIndex.y + proceduralHalfSize.y);

            if (cMinIndex.x < 0) { cMinIndex.x = 0; }
            if (cMinIndex.y < 0) { cMinIndex.y = 0; }
            if (cMaxIndex.x > maxIndex.x) { cMaxIndex.x = maxIndex.x; }
            if (cMaxIndex.y > maxIndex.y) { cMaxIndex.y = maxIndex.y; }

            List<CellData> toInstantiate = new List<CellData>();
            List<CellData> toDestory = new List<CellData>();

            foreach (CellData cd in activeCells)
            {
                if (cd.index.x < cMinIndex.x || cd.index.x > cMaxIndex.x || cd.index.y < cMinIndex.y || cd.index.y > cMaxIndex.y)
                {
                    toDestory.Add(cd);
                }
            }

            for(int x = cMinIndex.x; x <= cMaxIndex.x; x++)
            {
                for (int y = cMinIndex.y; y <= cMaxIndex.y; y++)
                {
                    if(cellMap[x, y].instantiated == false)
                    {
                        toInstantiate.Add(cellMap[x, y]);
                    }
                }
            }

            foreach(CellData cd in toInstantiate)
            {
                CellInstantiation(cd);
            }

            foreach(CellData cd in toDestory)
            {
                CellDestruction(cd);
            }
        }



        private void TerrainInstantiationAll()
        {
            foreach(CellData cd in cellMap)
            {
                CellInstantiation(cd);
            }
        }


        private void CellInstantiation(CellData cd)
        {
            if (cd.foundationAssigned)
            {
                cd.foundation = Instantiate(cd.foundationPrefab, this.transform);
                cd.foundation.transform.position = cd.groundPosition;
                cd.foundation.transform.localScale *= region.hexagonSize;
                cd.foundation.cellData = cd;
            }

            if (cd.surfaceAssigned)
            {
                cd.surface = Instantiate(cd.surfacePrefab, this.transform);
                cd.surface.transform.position = cd.groundPosition;
                cd.surface.transform.localScale *= region.hexagonSize;
                cd.surface.cellData = cd;
            }

            if (cd.airAssigned)
            {
                cd.air = Instantiate(cd.airPrefab, this.transform);
                cd.air.transform.position = cd.skyPosition;
                cd.air.transform.localScale *= region.hexagonSize;
                cd.air.cellData = cd;
            }

            if (cd.effectAssigned)
            {
                cd.effect = Instantiate(cd.effectPrefab, this.transform);
                cd.effect.transform.position = cd.skyPosition;
                cd.effect.transform.localScale *= region.hexagonSize;
                cd.effect.cellData = cd;
            }

            cd.instantiated = true;
            activeCells.Add(cd);
        }


        private void CellDestruction(CellData cd)
        {
            if (cd.foundationAssigned) { Destroy(cd.foundation.gameObject); }
            if (cd.airAssigned) { Destroy(cd.air.gameObject); }
            if (cd.surfaceAssigned) { Destroy(cd.surface.gameObject); }
            if (cd.effectAssigned) { Destroy(cd.effect.gameObject); }
            cd.instantiated = false;
            activeCells.Remove(cd);
        }

        private void PotentialTerrainGeneration()
        {
            CellMapInstantiation();

            // can delete if operation asset is rigid enough.
            OperationAssetsCheck(); 

            HeightAssignment();

            ObjectAssignment();

            FinalisingCellMap();
        }

        private void CellMapInstantiation()
        {
            cellMap = new CellData[region.terrainSize.x, region.terrainSize.y];

            for (int x = 0; x < cellMap.GetLength(0); x++)
            {
                for (int y = 0; y < cellMap.GetLength(1); y++)
                {
                    cellMap[x, y] = new CellData() { index = new int2(x, y) };
                }
            }
        }

        private void OperationAssetsCheck()
        {
            foreach(ObjectOperationAssets o in region.objectOperations)
            {
                if (o.manipulateFoundation)
                {
                    if (o.foundationVariants == null || o.foundationVariants.Count == 0) 
                    { 
                        o.manipulateFoundation = false; 
                        Debug.LogWarning($"{o.name} foundationVariants is null or empty, set manipulateFoundation to false."); 
                    }
                }

                if (o.manipulateSurface)
                {
                    if (o.surfaceVariants == null || o.surfaceVariants.Count == 0)
                    {
                        o.manipulateSurface = false;
                        Debug.LogWarning($"{o.name} surfaceVariants is null or empty, set manipulateSurface to false.");
                    }
                }

                if (o.manipulateAir)
                {
                    if (o.airVariants == null || o.airVariants.Count == 0)
                    {
                        o.manipulateAir = false;
                        Debug.LogWarning($"{o.name} airVariants is null or empty, set manipulateAir to false.");
                    }
                }

                if (o.manipulateEffect)
                {
                    if (o.effectVariants == null || o.effectVariants.Count == 0)
                    {
                        o.manipulateEffect = false;
                        Debug.LogWarning($"{o.name} effectVariants is null or empty, set manipulateEffect to false.");
                    }
                }
            }
        }

        private void HeightAssignment()
        {
            foreach (HeightOperationAssets h in region.heightOperations)
            {
                // Adjustment Operations.
                switch (h.heightOperationMode)
                {
                    case HeightOperationMode.adjustment_saturate:
                        TerrainUtils.SaturateCellHeights(cellMap);
                        continue;
                    case HeightOperationMode.adjustment_remap:
                        TerrainUtils.RemapCellHeights(cellMap);
                        continue;
                }

                // Noise Operations.
                int2 startingIndex = TerrainUtils.PercentageToIndex(h.startingPercent, region.terrainSize);
                int2 finishingIndex = TerrainUtils.PercentageToIndex(h.finishingPercent, region.terrainSize);
                int2 operationSize = new int2(finishingIndex.x - startingIndex.x + 1, finishingIndex.y - startingIndex.y + 1);

                float[,] noiseMap = new float[1, 1];
                switch (h.heightOperationMode)
                {
                    case HeightOperationMode.noise_simplePerlin:
                        noiseMap = TerrainUtils.GenerateNoiseFromSimplePerlin(h.simplePerlinParameters, operationSize, seed);
                        break;
                    case HeightOperationMode.noise_texture:
                        noiseMap = TerrainUtils.GenerateNoiseFromTexture(h.textureParameters, operationSize, seed);
                        break;
                }

                for (int x = startingIndex.x; x <= finishingIndex.x; x++)
                {
                    for (int y = startingIndex.y; y <= finishingIndex.y; y++)
                    {
                        float noiseValue = noiseMap[x - startingIndex.x, y - startingIndex.y];
                        switch (h.valueApplyMode)
                        {
                            case ValueApplyMode.additive:
                                cellMap[x, y].height += noiseValue;
                                break;
                            case ValueApplyMode.multiply:
                                cellMap[x, y].height *= noiseValue;
                                break;
                            case ValueApplyMode.average:
                                cellMap[x, y].height = (cellMap[x, y].height + noiseValue) / 2;
                                break;
                        }
                    }
                }
            }

            // By default, saturate the heights.
            TerrainUtils.SaturateCellHeights(cellMap);
        }

        private void ObjectAssignment()
        {
            foreach (ObjectOperationAssets o in region.objectOperations)
            {
                int2 startingIndex = TerrainUtils.PercentageToIndex(o.startingPercent, region.terrainSize);
                int2 finishingIndex = TerrainUtils.PercentageToIndex(o.finishingPercent, region.terrainSize);
                int2 operationSize = new int2(finishingIndex.x - startingIndex.x + 1, finishingIndex.y - startingIndex.y + 1);

                float[,] noiseMap = new float[1, 1];
                if (o.relevantToNoise)
                {
                    noiseMap = TerrainUtils.GenerateNoiseFromTexture(o.nTexSettings, operationSize, seed);
                }

                for (int x = startingIndex.x; x <= finishingIndex.x; x++)
                {
                    for (int y = startingIndex.y; y <= finishingIndex.y; y++)
                    {
                        CellData cd = cellMap[x, y];

                        bool heightCheck = false;
                        bool noiseCheck = false;
                        if (o.relevantToHeight && cd.height >= o.minHeight && cd.height <= o.maxHeight) { heightCheck = true; }
                        if (o.relevantToNoise && noiseMap[x - startingIndex.x, y - startingIndex.y] >= o.threshold) { noiseCheck = true; }

                        bool assignCheck = false;
                        if (o.relevantToHeight && o.relevantToNoise && heightCheck && noiseCheck) { assignCheck = true; }
                        else if (o.relevantToHeight && !o.relevantToNoise && heightCheck) { assignCheck = true; }
                        else if (!o.relevantToHeight && o.relevantToNoise && noiseCheck) { assignCheck = true; }

                        if (assignCheck == false) { continue; }

                        if (o.manipulateFoundation &&  !cd.foundationLocked && (!cd.foundationAssigned || o.replaceFoundation))
                        {
                            cd.foundationVariants = o.foundationVariants;
                            cd.foundationAssigned = true;
                            if (o.lockFoundation) { cd.foundationLocked = true; }
                        }

                        if (o.manipulateSurface && !cd.surfaceLocked && (!cd.surfaceAssigned || o.replaceSurface))
                        {
                            cd.surfaceVariants = o.surfaceVariants;
                            cd.surfaceAssigned = true;
                            if (o.lockSurface) { cd.surfaceLocked = true; }
                        }

                        if (o.manipulateAir && !cd.airLocked && (!cd.airAssigned || o.replaceAir))
                        {
                            cd.airVariants = o.airVariants;
                            cd.airAssigned = true;
                            if (o.lockAir) { cd.airLocked = true; }
                        }

                        if (o.manipulateEffect && !cd.effectLocked && (!cd.effectAssigned || o.replaceEffect))
                        {
                            cd.effectVariants = o.effectVariants;
                            cd.effectAssigned = true;
                            if (o.lockEffect) { cd.effectLocked = true; }
                        }
                    }
                }
            }
        }

        private void FinalisingCellMap()
        {
            for (int x = 0; x < cellMap.GetLength(0); x++)
            {
                for (int y = 0; y < cellMap.GetLength(1); y++)
                {
                    CellData cd = cellMap[x, y];

                    cd.XZPosition = HexagonIndexUtils.IndexToPosition(cd.index, region.pivotIndex, region.hexagonSize);

                    cd.groundPosition = new Vector3(cd.XZPosition.x, cd.height * region.terrainMaxHeight + region.terrainHeightOffset, cd.XZPosition.y);

                    cd.skyPosition = new Vector3(cd.XZPosition.x, region.skyLevel, cd.XZPosition.y);

                    if (cd.foundationAssigned)
                    {
                        cd.foundationPrefab = TerrainUtils.ChooseATerrainObject(cd.foundationVariants);
                    }

                    if (cd.surfaceAssigned)
                    {
                        cd.surfacePrefab = TerrainUtils.ChooseATerrainObject(cd.surfaceVariants);
                    }

                    if (cd.airAssigned)
                    {
                        cd.airPrefab = TerrainUtils.ChooseATerrainObject(cd.airVariants);
                    }

                    if (cd.effectAssigned)
                    {
                        cd.effectPrefab = TerrainUtils.ChooseATerrainObject(cd.effectVariants);
                    }
                }
            }
        }

    }
}