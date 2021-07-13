using System;
using System.Collections.Generic;
using UnityEngine;

namespace VISABConnector.Unity
{
    /// <summary>
    /// TODO: What is culling and what are layers?
    /// </summary>
    public static class ImageCreator
    {
        /// <summary>
        /// TODO: A bunch of dumb arguments checking TODO: You may also pass an existing camera.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static byte[] TakeSnapshot(SnapshotConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            int width = config.ImageWidth > 0 ? config.ImageWidth : throw new ArgumentException("Image width was smaller than 0!");
            int height = config.ImageHeight > 0 ? config.ImageHeight : throw new ArgumentException("Image height was smaller than 0!");

            //camera = camera ?? CameraCreator.CreateCamera();
            var camera = CameraCreator.CreateCamera();
            camera.orthographic = config.Orthographic;

            var gameObject = config.ShouldInstantiate ? InstantiateGameObject(config.InstantiationSettings) : GameObject.Find(config.GameObjectId);
            if (gameObject == null)
                throw new NotImplementedException(); // TODO: If this can happend, should throw instead

            camera.FocusOn(gameObject, config.CameraOffset, config.CameraRotation);

            // TODO: Ask marcel what this is
            if (camera.targetTexture == null)
            {
                // TODO: Does this 24 depth not clash with TextureFormat.ARGB32? How does this work
                camera.targetTexture = new RenderTexture(height, width, 24);
            }
            else
            {
                width = camera.targetTexture.width;
                height = camera.targetTexture.height;
            }

            // TODO: Ask marcel about this. Can this occur?
            if (!camera.gameObject.activeInHierarchy)
                throw new NotImplementedException();

            var snapshot = new Texture2D(width, height, TextureFormat.ARGB32, false);
            camera.Render();
            RenderTexture.active = camera.targetTexture;
            snapshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);

            var imageBytes = snapshot.EncodeToPNG();
            Debug.Log($"Took snapshot of{gameObject.name}");

            // Remove instantiated gameObject
            if (config.ShouldInstantiate)
            {
                GameObject.Destroy(gameObject);

            }

            // TODO: Deactivate camera?

            return imageBytes;
        }

        /// <summary>
        /// TODO: You may also pass an existing camera.
        /// </summary>
        /// <param name="configs"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static IDictionary<SnapshotConfiguration, byte[]> TakeSnapshots(IEnumerable<SnapshotConfiguration> configs)
        {
            var snapshots = new Dictionary<SnapshotConfiguration, byte[]>();
            foreach (var config in configs)
                snapshots[config] = TakeSnapshot(config);

            return snapshots;
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
    }
}