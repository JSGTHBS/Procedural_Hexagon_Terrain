namespace JCS.ProceduralHexagonTerrain
{
    /// <summary>
    /// Representing terrain sectors in the inner grid.
    /// Sector centres will also be (n * region.sectorSize.x, m * region.sectorSize.y) where n, m are integer scalers.
    /// Also hosting a reference to sector cells in 2D array of TerrainCell form.
    /// </summary>
    public class TerrainSector
    {
        public GridPosition gridCentre;
        public TerrainCell[,] sectorCells; // a 2D array representing the matrix of sector cells, with the bottom-left as the index [0, 0], the top-right as the index [sectorSize - 1, sectorSize -1].

        public TerrainSector(GridPosition centreGridPosition, TerrainCell[,] sectorCells)
        {
            this.gridCentre = centreGridPosition;
            this.sectorCells = sectorCells;
        }
    }
}

