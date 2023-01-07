using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCS.HexagonTerrain
{
    public class TerrainSurfaceRandomisation : MonoBehaviour
    {

        // Randomise Rotation.
        [Header("Specifications")]
        public bool randomiseRotation = false;
        private static float[] possibleRotations = new float[6] { 0f, 60f, 120f, 180f, 240f, 300f };


        // Start is called before the first frame update
        void Start()
        {
            if (randomiseRotation)
            {
                transform.Rotate(new Vector3(0, possibleRotations[Random.Range(0, 5)], 0), Space.Self);
            }
        }

    }
}