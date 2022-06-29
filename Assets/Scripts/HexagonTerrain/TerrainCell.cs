using UnityEngine;

namespace JCS.ProceduralHexagonTerrain
{
    /// <summary>
    /// The class is a rendezvous point between three constructs of the hexagon terrain:
    /// the 3D game world individual terrain game objects;
    /// the 2D-version of these terrain game objects, or say the xz-plane hexagons of them, used for fast computation;
    /// the inner normalised square gird of these hexagons that facilitates even faster computation.
    /// Check GridPosition for the rule of projection between game world hexagon and grid point.
    /// </summary>
    public class TerrainCell
    {
        public GridPosition gridPosition;

        public float hexagonSize;
        public Vector2 hexagonPosition; // The local position of the cell in relation to the Hexagon terrain.
        public float height; // normalised height, for terrain height generation.
        public float dvalue; // normalised decoration value, will instantiate decoration game object if this value is larger than set threshold.

        // references.
        public TerrainCylinder cylinder;
        public TerrainDecoration decoration;

        public TerrainCell(GridPosition gridPos, float hSize)
        {
            gridPosition = gridPos;
            hexagonSize = hSize;
            hexagonPosition = GridPosition.GridToGame(gridPos, hSize);
        }
    }
}
