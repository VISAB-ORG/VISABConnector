using UnityEngine;

namespace VISABConnector.Unity
{
    public class SnapshotConfiguration
    {
        public InstantiationConfiguration InstantiationSettings { get; set; }

        /// <summary>
        /// TODO: Really not name? GameObject.Find has a parameter with name that is used by us
        /// </summary>
        public string GameObjectId { get; set; }

        public float CameraOffset { get; set; }

        public Vector3 Rotation { get; set; }

        public int ImageWidth { get; set; }

        public int ImageHeight { get; set; }

        public Camera Camera { get; set; }

        public bool ShouldInstantiate => InstantiationSettings != null;
    }
}