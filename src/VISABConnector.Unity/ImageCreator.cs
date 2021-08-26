using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VISABConnector.Unity
{
    /// <summary>
    /// Class that unifies the methods in order to be able to snapshot any game item out of unity
    /// </summary>
    public static class ImageCreator
    {
        /// <summary>
        /// The VISAB layer variable which is assigned to every game object that will be snapshotted
        /// </summary>
        public const string VISABLayerName = "VISAB";

        /// <summary>
        /// The actual method that conducts the snapshotting of game objects. It is calibrated by passing a config object to it
        /// </summary>
        /// <param name="config">The config object that handles the specific requirements for snapping certain game objects</param>
        /// <returns>A byte array which can be converted to a PNG-file or inserted into a JSON-File</returns>
        public static byte[] TakeSnapshot(SnapshotConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (LayerMask.NameToLayer(VISABLayerName) == -1)
                throw new Exception("ImageCreator uses Unity layers to take screenshots of single GameObjects. " +
                                    $"Create a layer named {VISABLayerName} in the Unity Editor.");

            var width = config.ImageWidth > 0
                ? config.ImageWidth
                : throw new ArgumentException("Image width was smaller than 0!");
            var height = config.ImageHeight > 0
                ? config.ImageHeight
                : throw new ArgumentException("Image height was smaller than 0!");

            // create and set up camera
            var cameraConfig = config.CameraConfiguration;
            var camera = CameraSetup(cameraConfig, VISABLayerName, CameraClearFlags.Depth);

            // instantiate or grab game object
            var gameObject = GameObjectSetup(config);

            // preserve old layer before changing gameobjects to visab layer
            var oldLayer = gameObject.layer;
            ChangeLayer(gameObject, config, LayerMask.NameToLayer(VISABLayerName));

            // Focus the camera
            Debug.Log($"Offset: {cameraConfig.CameraOffset} is absolute? {cameraConfig.UseAbsoluteOffset}");

            if (cameraConfig.UseAbsoluteOffset)
                camera.FocusOnAbsolute(gameObject, cameraConfig.CameraOffset, cameraConfig.CameraRotation);
            else
                camera.FocusOn(gameObject, cameraConfig.CameraOffset, cameraConfig.CameraRotation);

            // Take the snapshot
            var texture = MakeTexture(camera, width, height);
            var imageBytes = texture.EncodeToPNG();

            Debug.Log($"Took snapshot of {gameObject.name}");

            // Restore gameobject's layers to previous state
            ChangeLayer(gameObject, config, oldLayer);

            //if (config.ShouldInstantiate)
            //    GameObject.Destroy(gameObject);

            // TODO: Deactivate camera?

            return imageBytes;
        }

        // Creates a texture on which the game object's snapshot will be copied to

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
        /// Conduct several snapshots at once
        /// </summary>
        /// <param name="configs">The config objects you will need for the snapshots</param>
        /// <returns>A dictionary which contains the byte arrays with their respective configuration objects</returns>
        public static IDictionary<SnapshotConfiguration, byte[]> TakeSnapshots(
            IEnumerable<SnapshotConfiguration> configs)
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

            var gameObject =
                Object.Instantiate(resource, config.SpawnLocation,
                    Quaternion.Euler(config.SpawnRotation)) as GameObject;
            if (gameObject == null)
                throw new Exception("Could not instantiate GameObject!");

            gameObject.SetActive(true);

            config.AfterInstantiation?.Invoke(gameObject);

            return gameObject;
        }

        /// <summary>
        /// Assigns Layer to all children of gameobject
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


        // Assigns layer to just child or parent (hence all children)

        private static void ChangeLayer(GameObject obj, SnapshotConfiguration config, int layer)
        {
            if (config.HasChildComponents && config.ChildConfiguration.SnapAllChilds)
                SetLayerRecursively(obj.transform.parent.gameObject, layer);
            else
                SetLayerRecursively(obj, layer);
        }


        // Creates and sets up camera accordingly

        private static Camera CameraSetup(CameraConfiguration camConfig, string cullingLayer,
            CameraClearFlags clearFlags)
        {
            var cam = CameraCreator.CreateCamera();

            cam.orthographic = camConfig.Orthographic;
            cam.cullingMask = 1 << LayerMask.NameToLayer(cullingLayer);
            cam.clearFlags = clearFlags;

            if (camConfig.Orthographic)
                cam.orthographicSize = camConfig.OrthographicSize;

            return cam;
        }


        // Instantiates or grabs Game Object and/or its child object

        private static GameObject GameObjectSetup(SnapshotConfiguration config)
        {
            GameObject gameObject;

            if (config.HasChildComponents)
            {
                gameObject = InstantiateGameObject(config.InstantiationSettings).transform
                    .Find(config.ChildConfiguration.ChildName).gameObject;
            }
            else
            {
                gameObject = config.ShouldInstantiate
                    ? InstantiateGameObject(config.InstantiationSettings)
                    : GameObject.Find(config.GameObjectId);
                if (!config.ShouldInstantiate && gameObject == null)
                    throw new Exception($"There is no GameObject with name {config.GameObjectId}!");
            }

            return gameObject;
        }
    }
}