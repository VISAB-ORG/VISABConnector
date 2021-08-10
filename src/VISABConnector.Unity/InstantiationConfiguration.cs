using System;
using UnityEngine;

namespace VISABConnector.Unity
{
    public class InstantiationConfiguration
    {
        public Vector3 SpawnLocation { get; set; }

        public Vector3 SpawnRotation { get; set; }

        public string PrefabPath { get; set; }

        public Action<GameObject> AfterInstantiation { get; set; }
    }
}