using System;
using UnityEngine;

namespace VISABConnector.Unity
{
    /// <summary>
    /// Configuration class that contains the relevant properties for snapshotting a gameobject
    /// </summary>
    public class SnapshotConfiguration
    {
        /// <summary>
        /// Contains several settings that can affect objects that will be instantiated
        /// </summary>
        public InstantiationConfiguration InstantiationSettings { get; set; }

        /// <summary>
        /// Contains several settings that adjust the camera that is used for snapshotting
        /// </summary>
        public CameraConfiguration CameraConfiguration { get; set; }

        /// <summary>
        /// Contains several settings that apply for game objects that contain multiple child objects (and you only want to snapshot a certain child)
        /// </summary>
        public ChildConfiguration ChildConfiguration { get; set; }


        /// <summary>
        ///
        /// </summary>
        public SnapshotConfiguration()
        {
        }

        /// <summary>
        /// If gameobject does not need to be instantiated, this property contains its name tag
        /// </summary>
        public string GameObjectId { get; set; }

        /// <summary>
        /// Output image width
        /// </summary>
        public int ImageWidth { get; set; }

        /// <summary>
        /// Output image height
        /// </summary>
        public int ImageHeight { get; set; }

        /// <summary>
        /// Boolean property that sets its value depending on if ChildConfiguration is available or not
        /// </summary>
        public bool HasChildComponents => ChildConfiguration != null;

        /// <summary>
        /// Boolean property that sets its value depending on if InstantiationSettings is available or not
        /// </summary>
        public bool ShouldInstantiate => InstantiationSettings != null;
    }
}