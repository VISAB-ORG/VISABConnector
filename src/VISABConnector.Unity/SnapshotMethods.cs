using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace VISABConnector.Unity
{
    public class SnapshotMethods : MonoBehaviour
    {
        public Camera minimapCam = new Camera();
        private static int resWidth = 1024;
        private static int resHeight = 1024;
        public Vector3 spawnPoint = new Vector3();

        public void SnapshotObject(GameObject obj, float offset, Vector3 rotation, int width, int height)
        {
            minimapCam.FocusOn(obj, offset, rotation);

            if (minimapCam.targetTexture == null)
            {
                minimapCam.targetTexture = new RenderTexture(width, height, 24);
            }
            else
            {
                resWidth = minimapCam.targetTexture.width;
                resHeight = minimapCam.targetTexture.height;
            }

            if (minimapCam.gameObject.activeInHierarchy)
            {
                Texture2D snapshot = new Texture2D(width, height, TextureFormat.ARGB32, false);
                minimapCam.Render();
                RenderTexture.active = minimapCam.targetTexture;
                snapshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                byte[] bytes = snapshot.EncodeToPNG();
                //string fileName = SnapshotName(resWidth, resHeight);
                System.IO.File.WriteAllBytes(Path.GetRandomFileName(), bytes);
                Debug.Log(obj.name + "Snapshot taken");

            }
        }

        public IEnumerator SnapInstantiatedObj(SnapshotConfiguration obj)
        {
            // Takes snapshot of object that needs to be instantiated
            yield return new WaitForSeconds(2);
            int oldMask = minimapCam.cullingMask;
            spawnPoint = GameObject.Find("SnapSpawn").transform.position;

            minimapCam = GameObject.Find("Minimap Camera").GetComponent<Camera>();
            minimapCam.gameObject.SetActive(true);

            var loadedObj = Resources.Load(obj.InstantiationSettings.PrefabPath) as GameObject;
            loadedObj = Instantiate(loadedObj, spawnPoint, Quaternion.identity);

            loadedObj.SetActive(true);

            loadedObj.layer = LayerMask.NameToLayer("Shootable");

            minimapCam.orthographic = false;
            minimapCam.cullingMask = 1 << LayerMask.NameToLayer("Shootable");

            SnapshotObject(loadedObj, obj.CameraOffset, obj.CameraRotation, obj.ImageWidth, obj.ImageHeight);
            yield return new WaitForSeconds(1);
            loadedObj.SetActive(false);

        }

        public IEnumerator SnapExistingObj(SnapshotConfiguration obj)
        {
            // Takes snapshot of object that already is spawned ingame
            yield return new WaitForSeconds(2);
            int oldMask = minimapCam.cullingMask;

            var spawnedObj = GameObject.Find(obj.GameObjectId);

            minimapCam = GameObject.Find("Minimap Camera").GetComponent<Camera>();
            minimapCam.gameObject.SetActive(true);
            minimapCam.orthographic = true;

            if (spawnedObj.name.Contains("Player"))
            {
                minimapCam.orthographic = false;
                minimapCam.cullingMask = 1 << LayerMask.NameToLayer("Shootable");
            }

            SnapshotObject(spawnedObj, obj.CameraOffset, obj.CameraRotation, obj.ImageWidth, obj.ImageHeight);

            minimapCam.cullingMask = oldMask;
        }
    }
}

