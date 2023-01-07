using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace JCS.HexagonTerrain
{
    /// <summary>
    /// Representing normalised point-top hexagon 2D index, where hexagons are effectively snapped to a 2D normalised integer grid: 
    /// hexagons' x length is 1, all y values are scaled down by 0.866, and all odd rows move 0.5 rightwards.
    /// </summary>
    public static class HexagonIndexUtils
    {
        /// <summary>
        /// Find the square distance between two index.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float SqrtDistance(int2 a, int2 b)
        {
            return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y);
        }

        /// <summary>
        /// Find the hexagon's right neighour's index.
        /// </summary>
        /// <param name="index"> The hexagon's index. </param>
        /// <returns></returns>
        public static int2 HexagonRight(int2 index)
        {
            return new int2(index.x + 1, index.y);
        }

        /// <summary>
        /// Find the hexagon's top right neighbour's index.
        /// </summary>
        /// <param name="index"> The hexagon's index. </param>
        /// <returns></returns>
        public static int2 HexagonTopright(int2 index)
        {
            return new int2(index.x + 1, index.y + 1);
        }

        /// <summary>
        /// Find the hexagon's top left neighbour's index.
        /// </summary>
        /// <param name="index"> The hexagon's index. </param>
        /// <returns></returns>
        public static int2 HexagonTopleft(int2 index)
        {
            return new int2(index.x - 1, index.y + 1);
        }

        /// <summary>
        /// Find the hexagon's left neighbour's index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int2 HexagonLeft(int2 index)
        {
            return new int2(index.x - 1, index.y);
        }

        /// <summary>
        /// Find the hexagon's bottom left neighbour's index.
        /// </summary>
        /// <param name="index"> The hexagon's index. </param>
        /// <returns></returns>
        public static int2 HexagonBottomleft(int2 index)
        {
            return new int2(index.x, index.y - 1);
        }

        /// <summary>
        /// Find the hexagon's bottom right neighbour's index.
        /// </summary>
        /// <param name="index"> The hexagon's index. </param>
        /// <returns></returns>
        public static int2 HexagonBottomright(int2 index)
        {
            return new int2(index.x + 1, index.y - 1);
        }

        /// <summary>
        /// Find the local position of the cell from its index.
        /// </summary>
        /// <param name="currentIndex"> The cell's index. </param>
        /// <param name="pivotIndex"> the central cell's index. </param>
        /// <param name="hSize"> Game world hexagon size. </param>
        /// <returns> A Vector2 represents a 2D-plane position in local space, taken the central cell as in the (0, 0). </returns>
        public static Vector2 IndexToPosition(int2 currentIndex, int2 pivotIndex, float hSize)
        {
            int2 rIndex = currentIndex - pivotIndex;

            if (rIndex.y % 2 == 0)
            {
                return new Vector2(rIndex.x, rIndex.y * 0.866f) * hSize;
            }
            else
            {
                return new Vector2(rIndex.x - 0.5f, rIndex.y * 0.866f) * hSize;
            }
        }

        /// <summary>
        /// Find the nearest index given a local position.
        /// </summary>
        /// <param name="gamePos"> local position (in a 2D plane). </param>
        /// <param name="hSize"> Game world hexagon size. </param>
        /// <returns> (Nearest) index. </returns>
        public static int2 PositionToIndex(Vector2 gamePos, int2 pivotIndex, float hSize)
        {
            int yPos = Mathf.RoundToInt(gamePos.y / (hSize * 0.866f));

            if (yPos % 2 == 0)
            {
                return new int2(Mathf.RoundToInt(gamePos.x / hSize), yPos) + pivotIndex;
            }
            else
            {
                return new int2(Mathf.RoundToInt(gamePos.x / hSize + 0.5f), yPos) + pivotIndex;
            }
        }

    }
}