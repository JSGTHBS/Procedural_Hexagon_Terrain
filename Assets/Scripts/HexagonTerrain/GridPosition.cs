using System;
using System.Collections.Generic;
using UnityEngine;

namespace JCS.ProceduralHexagonTerrain
{
    /// <summary>
    /// Representing the inner abstract 2D coordination system of the hexagon terrain in the game world.
    /// In this grid system, hexagons in the game world are normalised then snapped onto integer points,
    /// by the rule of all y normalised by being divided by the hexagon size and 0.866, then all odd rows moved 0.5 rightward in x.
    /// </summary>
    public struct GridPosition : IEquatable<GridPosition>
    {
        public int x;
        public int y;

        public GridPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj) => obj is GridPosition other && this.Equals(other);
        public bool Equals(GridPosition other) => x == other.x && y == other.y;
        public override int GetHashCode() => (x, y).GetHashCode();
        public static bool operator ==(GridPosition lhs, GridPosition rhs) => lhs.Equals(rhs);
        public static bool operator !=(GridPosition lhs, GridPosition rhs) => !lhs.Equals(rhs);

        /// <summary>
        /// Find the square distance between two grid positions.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float SqrtDistance(GridPosition a, GridPosition b)
        {
            return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y);
        }

        /// <summary>
        /// Find the hexagon's right neighour's grid position.
        /// </summary>
        /// <param name="hexagonGridPos"></param>
        /// <returns></returns>
        public static GridPosition HexagonRight(GridPosition hexagonGridPos)
        {
            return new GridPosition(hexagonGridPos.x + 1, hexagonGridPos.y);
        }

        /// <summary>
        /// Find the hexagon's top right neighbour's grid position.
        /// </summary>
        /// <param name="hexagonGridPos"></param>
        /// <returns></returns>
        public static GridPosition HexagonTopright(GridPosition hexagonGridPos)
        {
            return new GridPosition(hexagonGridPos.x + 1, hexagonGridPos.y + 1);
        }

        /// <summary>
        /// Find the hexagon's top left neighbour's grid position.
        /// </summary>
        /// <param name="hexagonGridPos"></param>
        /// <returns></returns>
        public static GridPosition HexagonTopleft(GridPosition hexagonGridPos)
        {
            return new GridPosition(hexagonGridPos.x - 1, hexagonGridPos.y + 1);
        }

        /// <summary>
        /// Find the hexagon's left neighbour's grid position.
        /// </summary>
        /// <param name="centre"></param>
        /// <returns></returns>
        public static GridPosition HexagonLeft(GridPosition centre)
        {
            return new GridPosition(centre.x - 1, centre.y);
        }

        /// <summary>
        /// Find the hexagon's bottom left neighbour's grid position.
        /// </summary>
        /// <param name="hexagonGridPos"></param>
        /// <returns></returns>
        public static GridPosition HexagonBottomleft(GridPosition hexagonGridPos)
        {
            return new GridPosition(hexagonGridPos.x, hexagonGridPos.y - 1);
        }

        /// <summary>
        /// Find the hexagon's bottom right neighbour's grid position.
        /// </summary>
        /// <param name="hexagonGridPos"></param>
        /// <returns></returns>
        public static GridPosition HexagonBottomright(GridPosition hexagonGridPos)
        {
            return new GridPosition(hexagonGridPos.x + 1, hexagonGridPos.y - 1);
        }

        /// <summary>
        /// Turn the grid position into a game world position.
        /// </summary>
        /// <param name="gridPos"> A grid point. </param>
        /// <param name="hSize"> Game world hexagon size. </param>
        /// <returns> Game world position. </returns>
        public static Vector2 GridToGame(GridPosition gridPos, float hSize)
        {
            if (gridPos.y % 2 == 0)
            {
                return new Vector2(gridPos.x, gridPos.y * 0.866f) * hSize;
            }
            else
            {
                return new Vector2(gridPos.x - 0.5f, gridPos.y * 0.866f) * hSize;
            }
        }

        /// <summary>
        /// Turn the game world position into a grid position. Will find cloest grid point if the worldPos is not any hexagon's centre position.
        /// </summary>
        /// <param name="gamePos"> Game world position (in a 2D plane). </param>
        /// <param name="hSize"> Game world hexagon size. </param>
        /// <returns> (Nearest) grid position. </returns>
        public static GridPosition GameToGrid(Vector2 gamePos, float hSize)
        {
            int yPos = Mathf.RoundToInt(gamePos.y / (hSize * 0.866f));

            if (yPos % 2 == 0)
            {
                return new GridPosition(Mathf.RoundToInt(gamePos.x / hSize), yPos);
            }
            else
            {
                return new GridPosition(Mathf.RoundToInt(gamePos.x / hSize + 0.5f), yPos);
            }
        }

        /// <summary>
        /// Find the centre grid position of the sector where the given game world position belongs to.
        /// Might not be precise when in sector edges. 
        /// </summary>
        /// <param name="gamePos"> Game world position (in a 2D plane). </param>
        /// <param name="hSize"> Game world hexagon size. </param>
        /// <param name="sSize"> Grid sector size. </param>
        /// <returns> the centre of the current sector. </returns>
        public static GridPosition CurrentSectorCentre(Vector2 gamePos, float hSize, int sSize)
        {
            GridPosition nearestGridPoint = GameToGrid(gamePos, hSize);

            return new GridPosition(Mathf.RoundToInt((float)nearestGridPoint.x / sSize) * sSize, Mathf.RoundToInt((float)nearestGridPoint.y / sSize) * sSize);
        }

        /// <summary>
        /// Find the eight neighbourhood sector grid centres of the given sector.
        /// </summary>
        /// <param name="cSector"> The given sector's centre. </param>
        /// <param name="hSize"> Game world hexagon side. </param>
        /// <param name="sSize"> Grid sector size. </param>
        /// <returns> Eight neighbourhood sector centres. </returns>
        public static GridPosition[] NeighbourSectorCentres(GridPosition cSector, int sSize)
        {
            return new GridPosition[8]
            {
            new GridPosition(cSector.x + sSize, cSector.y),
            new GridPosition(cSector.x + sSize, cSector.y + sSize),
            new GridPosition(cSector.x, cSector.y + sSize),
            new GridPosition(cSector.x - sSize, cSector.y + sSize),
            new GridPosition(cSector.x - sSize, cSector.y),
            new GridPosition(cSector.x - sSize, cSector.y - sSize),
            new GridPosition(cSector.x, cSector.y - sSize),
            new GridPosition(cSector.x + sSize, cSector.y - sSize),
            };
        }

        /// <summary>
        /// Find nearby sector's grid centres of certain generations.
        /// </summary>
        /// <param name="gamePos"> Game world position (in a 2D plane). </param>
        /// <param name="hSize"> Game world hexagon size. </param>
        /// <param name="sSize"> Grid sector size. </param>
        /// <param name="generations"> 0 for current sector, 1 for self plus immediate neighbours, and so on. </param>
        /// <returns> Grid centres of nearby sectors. </returns>
        public static GridPosition[] NearbySectorCentres(Vector2 gamePos, float hSize, int sSize, int generations)
        {
            GridPosition cSector = CurrentSectorCentre(gamePos, hSize, sSize);

            List<GridPosition> toProcess = new List<GridPosition>() { cSector };
            List<GridPosition> sectors = new List<GridPosition>() { cSector };

            for (int i = 0; i < generations; i++)
            {
                GridPosition[] currProcess = toProcess.ToArray();
                foreach (GridPosition p in currProcess)
                {
                    GridPosition[] neighbours = NeighbourSectorCentres(p, sSize);

                    foreach (GridPosition n in neighbours)
                    {
                        if (!sectors.Contains(n))
                        {
                            sectors.Add(n);
                            toProcess.Add(n);
                        }
                    }
                    toProcess.Remove(p);
                }
            }

            return sectors.ToArray();
        }
    }
}