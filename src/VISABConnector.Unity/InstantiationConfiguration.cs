using UnityEngine;

namespace VISABConnector.Unity
{
    public class InstantiationConfiguration
    {
        public Vector3 SpawnLocation { get; set; }

        public Quaternion SpawnRotation { get; set; }

        public string PrefabPath { get; set; }
    }
}