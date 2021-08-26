using UnityEngine;

namespace VISABConnector.Unity
{
    /// <summary>
    /// Class that unites the settings for the snapshot camera
    /// </summary>
    public class CameraConfiguration
    {
        /// <summary>
        /// If you want to adjust the distance between the camera and the object to be captured use this property
        /// </summary>
        public float CameraOffset { get; set; }

        /// <summary>
        /// Uses the absolute offset without considering the game objects bounds
        /// </summary>
        public bool UseAbsoluteOffset { get; set; }

        /// <summary>
        /// If you want to adjust the camera's rotation use this property
        /// </summary>
        public Vector3 CameraRotation { get; set; }

        /// <summary>
        /// In case you want to switch between 'perspective' view and 'orthographic' view
        /// </summary>
        public bool Orthographic { get; set; }

        /// <summary>
        /// In case 'orthographic' is true, one may need to adjust the camera's panel size
        /// </summary>
        public float OrthographicSize { get; set; } = 1;
    }
}