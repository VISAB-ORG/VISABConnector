using UnityEngine;

namespace VISABConnector.Unity
{
    public class CameraConfiguration
    {
        public float CameraOffset { get; set; }

        public Vector3 CameraRotation { get; set; }

        public bool Orthographic { get; set; }

        public float OrthographicSize { get; set; }
    }
}