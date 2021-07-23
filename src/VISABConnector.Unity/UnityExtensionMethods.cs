using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VISABConnector.Unity
{
    public static class UnityExtensionMethods
    {
        public static Bounds GetBoundsWithChildren(this GameObject gameObject)
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

        public static void FocusOnAbsolute(this Camera cam, GameObject obj, float absoluteYOffset, Vector3 rotationAngle)
        {
            Bounds bounds = obj.GetBoundsWithChildren();
            float maxExtent = bounds.extents.magnitude;
            cam.transform.position = bounds.center + Vector3.up * absoluteYOffset;
            cam.transform.Rotate(rotationAngle.x, rotationAngle.y, rotationAngle.z, Space.Self);


            Debug.Log("GameObj: " + obj + ", coordinates: " + obj.transform.position);
            Debug.Log("Camera: " + cam + ", coordinates: " + cam.transform.position);

            Debug.Log(bounds);
        }

        public static void FocusOn(this Camera cam, GameObject focusedObject, float marginPercentage, Vector3 rotationAngle)
        {
            Bounds bounds = focusedObject.GetBoundsWithChildren();
            float maxExtent = bounds.extents.magnitude;
            float minDistance = (maxExtent * marginPercentage) / Mathf.Sin(Mathf.Deg2Rad * cam.fieldOfView / 0.5f);
            cam.transform.position = bounds.center + Vector3.up * minDistance;
            cam.transform.rotation = Quaternion.identity;
            cam.transform.Rotate(CameraCreator.DefaultRotation);
            cam.transform.Rotate(rotationAngle.x, rotationAngle.y, rotationAngle.z, Space.Self);

            Debug.Log("GameObj: " + focusedObject + ", coordinates: " + focusedObject.transform.position);
            Debug.Log("Camera: " + cam + ", coordinates: " + cam.transform.position);
            cam.nearClipPlane = minDistance - maxExtent;
            //cam.transform.LookAt(focusedObject.transform.position);
            Debug.Log(bounds);
        }

        public static void CenterOn(this Camera cam, GameObject obj)
        {
            Bounds b = GetBoundsWithChildren(obj);
            Vector3 v = obj.transform.position;

            cam.transform.position = v + (Vector3.up * 10);
            Debug.Log(cam.transform.position);
            cam.transform.LookAt(obj.transform.position);
        }
    }
}
