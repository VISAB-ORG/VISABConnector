using System;
using UnityEngine;

namespace VISABConnector.Unity
{
    public class SnapshotConfiguration
    {
        public InstantiationConfiguration InstantiationSettings { get; set; }

        public CameraConfiguration CameraConfiguration { get; set; }

        /// <summary>
        /// TODO: Really not name? GameObject.Find has a parameter with name that is used by us
        /// </summary>
        public SnapshotConfiguration()
        {
        }

        public string GameObjectId { get; set; }

        public int ImageWidth { get; set; }

        public int ImageHeight { get; set; }

        public bool ShouldInstantiate => InstantiationSettings != null;
    }
}