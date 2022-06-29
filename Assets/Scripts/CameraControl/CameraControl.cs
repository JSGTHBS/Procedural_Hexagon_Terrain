using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCS.CameraControl
{
    /// <summary>
    /// Attach to the main camera; used for checking procedural terrain in the play mode only.
    /// </summary>
    public class CameraControl : MonoBehaviour
    {
        public float cameraSpeed = 10f;

        void Update()
        {
            transform.Translate(Input.GetAxis("Horizontal") * cameraSpeed * Time.deltaTime, 0f, Input.GetAxis("Vertical") * cameraSpeed * Time.deltaTime, Space.World);
        }
    }
}