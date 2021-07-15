using System;
using UnityEngine;

namespace VISABConnector.Unity
{
    public class SnapshotConfiguration
    {
        public InstantiationConfiguration InstantiationSettings { get; set; }

        /// <summary>
        /// TODO: Really not name? GameObject.Find has a parameter with name that is used by us
        /// </summary>
        public SnapshotConfiguration()
        {
        }

        public string GameObjectId { get; set; }

        public float CameraOffset { get; set; }
        
        public Vector3 CameraRotation { get; set; }

        public bool Orthographic { get; set; }

        public int ImageWidth { get; set; }

        public int ImageHeight { get; set; }

        public bool ShouldInstantiate => InstantiationSettings != null;
    }
}