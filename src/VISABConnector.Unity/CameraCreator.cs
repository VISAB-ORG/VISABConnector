using UnityEngine;

namespace VISABConnector.Unity
{
    internal static class CameraCreator
    {
        /// <summary>
        /// The name of the camera that is created by us, if no camera is given.
        /// </summary>
        internal const string CameraName = "VISABConnector.Unity Camera";

        /// <summary>
        /// Creates the camera that will be used for snapshotting the game objects
        /// </summary>
        /// <returns>Returns the created camera</returns>
        internal static Camera CreateCamera()
        {
            var existing = GameObject.Find(CameraName);
            if (existing && existing.GetComponent<Camera>() != null)
                return existing.GetComponent<Camera>();

            // Create new gameObject and add camera component
            var gameObject = new GameObject(CameraName);
            var camera = gameObject.AddComponent<Camera>();

            return camera;
        }
    }
}