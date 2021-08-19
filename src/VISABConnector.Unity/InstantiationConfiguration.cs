using System;
using UnityEngine;

namespace VISABConnector.Unity
{
    /// <summary>
    /// Contains the configuration settings for gameobjects that need to be instantiated before being snapshotted 
    /// </summary>
    public class InstantiationConfiguration
    {
        /// <summary>
        /// Location where gameobject will be instantiated
        /// </summary>
        public Vector3 SpawnLocation { get; set; }

        /// <summary>
        /// In case you want to rotate the gameobject
        /// </summary>
        public Vector3 SpawnRotation { get; set; }

        /// <summary>
        /// Contains the prefab path in case you want to instantiate a prefab
        /// </summary>
        public string PrefabPath { get; set; }

        public Action<GameObject> AfterInstantiation { get; set; }
    }
}