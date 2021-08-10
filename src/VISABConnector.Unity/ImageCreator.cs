using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VISABConnector.Unity
{
    /// <summary>
    /// TODO: Smart Instantiate
    /// </summary>
    public static class ImageCreator
    {
        public const string VISABLayerName = "VISAB";

        /// <summary>
        /// TODO: A bunch of dumb arguments checking TODO: You may also pass an existing camera.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static byte[] TakeSnapshot(SnapshotConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (LayerMask.NameToLayer(VISABLayerName) == -1)
            {
                throw new Exception($"ImageCreator uses Unity layers to take screenshots of single GameObjects. " +
                                    $"Create a layer named {VISABLayerName} in the Unity Editor.");
            }

            int width = config.ImageWidth > 0 ? config.ImageWidth : throw new ArgumentException("Image width was smaller than 0!");
            int height = config.ImageHeight > 0 ? config.ImageHeight : throw new ArgumentException("Image height was smaller than 0!");

            var camera = CameraCreator.CreateCamera();
            var cameraConfig = config.CameraConfiguration;
            camera.orthographic = cameraConfig.Orthographic;
            if (cameraConfig.Orthographic)
                camera.orthographicSize = cameraConfig.OrthographicSize;

            GameObject gameObject;

            if (config.HasChildComponents)
            {
                
                gameObject = InstantiateGameObject(config.InstantiationSettings).transform.Find(config.ChildConfiguration.ChildName).gameObject;
            }
            else
            {
                gameObject = config.ShouldInstantiate ? InstantiateGameObject(config.InstantiationSettings) : GameObject.Find(config.GameObjectId);
                if (!config.ShouldInstantiate && gameObject == null)
                    throw new Exception($"There is no GameObject with name {config.GameObjectId}!");
            }

            //gameObject = config.ShouldInstantiate ? InstantiateGameObject(config.InstantiationSettings) : GameObject.Find(config.GameObjectId);

            //if (!config.ShouldInstantiate && gameObject == null)
            //    throw new Exception($"There is no GameObject with name {config.GameObjectId}!");


            int oldLayer = gameObject.layer;

            if (config.HasChildComponents)
            {
                SetLayerRecursively(gameObject.transform.parent.gameObject, LayerMask.NameToLayer(VISABLayerName));
            }
            else
            {
                SetLayerRecursively(gameObject, LayerMask.NameToLayer(VISABLayerName));
            }


            camera.cullingMask = 1 << LayerMask.NameToLayer(VISABLayerName);
            camera.clearFlags = CameraClearFlags.Depth;

            // Focus the camera
            Debug.Log($"Offset: {cameraConfig.CameraOffset} is absolute? {cameraConfig.UseAbsoluteOffset}");
            if (cameraConfig.UseAbsoluteOffset)
                camera.FocusOn(gameObject, cameraConfig.CameraOffset, cameraConfig.CameraRotation);
                //camera.FocusOnAbsolute(gameObject, cameraConfig.CameraOffset, cameraConfig.CameraRotation);
            else
                camera.FocusOn(gameObject, cameraConfig.CameraOffset, cameraConfig.CameraRotation);

            // Take the snapshot
            var texture = MakeTexture(camera, width, height);
            var imageBytes = texture.EncodeToPNG();

            Debug.Log($"Took snapshot of {gameObject.name}");

            // Restore gameobject to previous state
            if(config.HasChildComponents)
            {
                SetLayerRecursively(gameObject.transform.parent.gameObject, oldLayer);
            }
            else
            {
                SetLayerRecursively(gameObject, oldLayer);
            }

            //if (config.ShouldInstantiate)
            //    GameObject.Destroy(gameObject);

            // TODO: Deactivate camera?

            return imageBytes;
        }

        private static Texture2D MakeTexture(Camera camera, int width, int height)
        {
            camera.targetTexture = RenderTexture.GetTemporary(width, height, 32);
            camera.Render();

            RenderTexture.active = camera.targetTexture;

            var texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();

            RenderTexture.active = null;
            camera.targetTexture = null;

            RenderTexture.ReleaseTemporary(camera.targetTexture);

            return texture;
        }

        /// <summary>
        /// TODO: You may also pass an existing camera.
        /// </summary>
        /// <param name="configs"></param>
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
            if (resource == null)
                throw new Exception($"Could not load prefab from path {config.PrefabPath}");

            var gameObject = GameObject.Instantiate(resource, position: config.SpawnLocation, rotation: config.SpawnRotation) as GameObject;
            if (gameObject == null)
                throw new Exception($"Could not instantiate GameObject!");

            gameObject.SetActive(true);

            config.AfterInstantiation?.Invoke(gameObject);

            return gameObject;
        }

        /// <summary>
        /// Assigns Layer to all children of gameobjects
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="layer"></param>
        public static void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;
            foreach (Transform child in obj.transform)
            {
                obj.layer = layer;

                SetLayerRecursively(child.gameObject, layer);
            }
        }
    }
}