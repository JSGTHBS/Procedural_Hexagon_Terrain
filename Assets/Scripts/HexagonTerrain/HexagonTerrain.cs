using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCS.ProceduralHexagonTerrain
{
    public class HexagonTerrain : MonoBehaviour
    {

        public TerrainRegionAssets region;

        public float generationCheckInterval = 1f;
        public float degenerationCheckInterval = 1f;

        List<TerrainSector> activeSectors = new List<TerrainSector>(); 

        private void Start()
        {
            StartCoroutine(GenerationCoroutine(new WaitForSeconds(generationCheckInterval)));
            StartCoroutine(DegenerationCoroutine(new WaitForSeconds(degenerationCheckInterval)));
        }

        private IEnumerator GenerationCoroutine(WaitForSeconds interval)
        {
            while (true)
            {
                ProceduralGeneration();
                yield return interval;
            }
        }

        /// <summary>
        /// Observe where the current sector is, then generate terrain sectors of n generations, if any of them not yet generated. 
        /// </summary>
        private void ProceduralGeneration()
        {
            Vector2 camPosXZ = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.z);

            GridPosition[] nearbySectors = GridPosition.NearbySectorCentres(camPosXZ, region.hexagonSize, region.sectorSize, region.generationGenerations);

            foreach (GridPosition gp in nearbySectors)
            {
                bool isActive = false;
                foreach (TerrainSector ts in activeSectors)
                {
                    if (ts.gridCentre == gp)
                    {
                        isActive = true;
                        break;
                    }
                }
                if (isActive == false)
                {
                    GenerateTerrainSector(gp);
                }
            }
        }

        private IEnumerator DegenerationCoroutine(WaitForSeconds interval)
        {
            while (true)
            {
                ProceduralDegeneration();
                yield return interval;
            }
        }

        /// <summary>
        /// Degenerate Terrain sectors if too far away from the camera.
        /// </summary>
        private void ProceduralDegeneration()
        {
            Vector2 camPosXZ = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.z);

            GridPosition currentSector = GridPosition.CurrentSectorCentre(camPosXZ, region.hexagonSize, region.sectorSize);

            TerrainSector[] aSectors = activeSectors.ToArray();
            foreach (TerrainSector s in aSectors)
            {
                if (GridPosition.SqrtDistance(s.gridCentre, currentSector) >
                    Mathf.Pow(region.degenerationGenerations * region.sectorSize, 2))
                {
                    DegenerationTerrainSector(s);
                }
            }
        }

        /// <summary>
        /// Generate a sector of terrain.
        /// </summary>
        /// <param name="sectorCentre"></param>
        private void GenerateTerrainSector(GridPosition sectorCentre)
        {
            // Sector instantiation.
            TerrainSector newSector = new TerrainSector(sectorCentre, new TerrainCell[region.sectorSize, region.sectorSize]);
            int xMin = sectorCentre.x - region.sectorSize / 2;
            int xMax = sectorCentre.x + region.sectorSize / 2;
            int yMin = sectorCentre.y - region.sectorSize / 2;
            int yMax = sectorCentre.y + region.sectorSize / 2;

            for (int x = xMin; x < xMax; x++)
            {
                for (int y = yMin; y < yMax; y++)
                {
                    newSector.sectorCells[x - xMin, y - yMin] = new TerrainCell(new GridPosition(x, y), region.hexagonSize);
                }
            }

            activeSectors.Add(newSector);

            // Apply Procedural.
            TerrainFormAseets terrainForm = region.GetSectorForm(newSector);
            terrainForm.ApplyHeights(newSector);
            terrainForm.ApplyDValues(newSector);

            // Instantiate game objects.
            foreach (TerrainCell c in newSector.sectorCells)
            {
                foreach (CylinderPrototype tp in terrainForm.cylinderPrototypes)
                {
                    if (c.height >= tp.minHeight && c.height < tp.maxHeight)
                    {
                        c.cylinder = Instantiate(tp.cylinderPrefabs[Random.Range(0, tp.cylinderPrefabs.Count)], transform);
                        c.cylinder.transform.localPosition = new Vector3(c.hexagonPosition.x, c.height * region.terrainYMax + region.terrainYOffset, c.hexagonPosition.y);
                        c.cylinder.transform.localScale *= region.hexagonSize;
                        c.cylinder.cell = c;
                    }
                }

                foreach (DecorationPrototype dp in terrainForm.decorationPrototypes)
                {
                    if (c.height >= dp.minHeight && c.height < dp.maxHeight && c.dvalue > dp.threshold && c.decoration is null)
                    {
                        c.decoration = Instantiate(dp.decorationPrefabs[Random.Range(0, dp.decorationPrefabs.Count)], transform);
                        c.decoration.transform.localPosition = new Vector3(c.hexagonPosition.x, c.height * region.terrainYMax + region.terrainYOffset, c.hexagonPosition.y);
                        c.decoration.transform.localScale *= region.hexagonSize;
                        c.decoration.cell = c;
                    }
                }
            }
        }

        /// <summary>
        /// Degenerate a sector of terrain.
        /// </summary>
        /// <param name="sector"></param>
        private void DegenerationTerrainSector(TerrainSector sector)
        {
            foreach (TerrainCell c in sector.sectorCells)
            {
                if (!ReferenceEquals(c.cylinder, null))
                {
                    Destroy(c.cylinder.gameObject);
                }
                if (!ReferenceEquals(c.decoration, null))
                {
                    Destroy(c.decoration.gameObject);
                }
            }

            activeSectors.Remove(sector);
        }
    }
}