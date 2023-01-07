using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCS.HexagonTerrain
{
    public class TerrainFoundationRandomisation : MonoBehaviour
    {

        [Header("Specifications")]
        public bool randomiseTopOffset = true;
        public bool randomiseBottomOffset = true;
        public bool randomiseNoiseOffset = true;
        public bool randomiseBlendPercent = true;

        [Header("Runtime")]
        Material mat;
        public CellData cellData;

        // Start is called before the first frame update
        void Start()
        {
            mat = GetComponent<Renderer>().material;

            if (randomiseTopOffset == true)
            {
                mat.SetVector("_TopOffset", new Vector4(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0, 0));
            }
            if (randomiseBottomOffset == true)
            {
                mat.SetVector("_BottomOffset", new Vector4(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0, 0));
            }
            if (randomiseNoiseOffset == true)
            {
                mat.SetVector("_NoiseOffset", new Vector4(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0, 0));
            }
            if (randomiseBlendPercent == true)
            {
                mat.SetFloat("_BlendPercent", mat.GetFloat("_BlendPercent") * Random.Range(0.5f, 2f));
            }

        }

    }
}