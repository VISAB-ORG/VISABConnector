using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VISABConnector.Unity
{
    public static class ImageCreator
    {
        /// <summary>
        /// TODO: A bunch of dumb arguments checking
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static byte[] TakeSnapshot(SnapshotConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var gameObject = config.ShouldInstantiate ? InstantiateGameObject(config.InstantiationSettings) : GameObject.Find(config.GameObjectId);
            if (gameObject == null)
                return null;

            var camera = config.Camera ?? CreateCamera();



            // Remove camera
            // Remove instantiated gameObject

            return null;

        }

        public static IDictionary<SnapshotConfiguration, byte[]> TakeSnapshots(IEnumerable<SnapshotConfiguration> configs)
        {
            return null;
        }

        /// <summary>
        /// Instantiates a GameObject based on the given configuration and returns the instance.
        /// </summary>
        /// <param name="config">The configuration based on which the GameObject will be instantiated</param>
        /// <returns>The instance for the given configuration</returns>
        public static GameObject InstantiateGameObject(InstantiationConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (string.IsNullOrWhiteSpace(config.PrefabPath))
                throw new ArgumentException("Configuration given has no prefab path!");

            var resource = Resources.Load(config.PrefabPath);
            // TODO: Dont know if this will be null or throw itself

            var @object = GameObject.Instantiate(resource, position: config.SpawnLocation, rotation: config.SpawnRotation);
            // TODO: Dont know if this will be null or throw itself

            return @object as GameObject;
        }

        private static Camera CreateCamera()
        {
            return null;
        }

        public static Bounds GetBoundsWithChildren(GameObject gameObject)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>();

            var bounds = renderers.Length > 0 ? renderers[0].bounds : new Bounds();
            for (int i = 1; i < renderers.Length; i++)
            {
                if (renderers[i].enabled)
                    bounds.Encapsulate(renderers[i].bounds);
            }

            return bounds;
        }
    }
}
