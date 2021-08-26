using UnityEngine;

namespace VISABConnector.Unity
{
    /// <summary>
    /// Extension methods that do the actual snapshotting work
    /// </summary>
    public static class UnityExtensionMethods
    {
        /// <summary>
        /// Returns the bounds of the gameobject including all of its children
        /// </summary>
        /// <param name="gameObject">The gameobject that is supposed to be snapped</param>
        /// <returns>The object's bounds</returns>
        public static Bounds GetBoundsWithChildren(this GameObject gameObject)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>();

            var bounds = renderers.Length > 0 ? renderers[0].bounds : new Bounds();
            for (var i = 1; i < renderers.Length; i++)
                if (renderers[i].enabled)
                    bounds.Encapsulate(renderers[i].bounds);

            return bounds;
        }

        /// <summary>
        /// Positions the camera exactly centered onto the according game object. But does not consider the object's bounds to
        /// determine the offset, rather the given parameter absoluteYOffset.
        /// </summary>
        /// <param name="cam">The snapshot cam</param>
        /// <param name="obj">The game object to be snapped</param>
        /// <param name="absoluteYOffset">The distance between object and camera</param>
        /// <param name="rotationAngle">The camera's rotation</param>
        public static void FocusOnAbsolute(this Camera cam, GameObject obj, float absoluteYOffset,
            Vector3 rotationAngle)
        {
            var bounds = obj.GetBoundsWithChildren();
            var maxExtent = bounds.extents.magnitude;
            cam.transform.position = bounds.center + Vector3.up * absoluteYOffset;

            cam.transform.rotation = Quaternion.Euler(rotationAngle);

            Debug.Log("GameObj: " + obj + ", coordinates: " + obj.transform.position);
            Debug.Log("Camera: " + cam + ", coordinates: " + cam.transform.position);

            Debug.Log(bounds);
        }

        /// <summary>
        /// Positions the camera over the gamobject
        /// </summary>
        /// <param name="cam">The snapshot cam</param>
        /// <param name="focusedObject">The game object to be snapped</param>
        /// <param name="marginPercentage">Adjusts the distance between object and camera</param>
        /// <param name="rotationAngle">The camera's rotation</param>
        public static void FocusOn(this Camera cam, GameObject focusedObject, float marginPercentage,
            Vector3 rotationAngle)
        {
            var bounds = focusedObject.GetBoundsWithChildren();
            var maxExtent = bounds.extents.magnitude;
            var minDistance = maxExtent * marginPercentage / Mathf.Sin(Mathf.Deg2Rad * cam.fieldOfView / 0.5f);
            cam.transform.position = bounds.center + Vector3.up * minDistance;

            cam.transform.rotation = Quaternion.Euler(rotationAngle);

            Debug.Log("GameObj: " + focusedObject + ", coordinates: " + focusedObject.transform.position);
            Debug.Log("Camera: " + cam + ", coordinates: " + cam.transform.position);
            cam.nearClipPlane = minDistance - maxExtent;
            //cam.transform.LookAt(focusedObject.transform.position);
            Debug.Log(bounds);
        }

        /// <summary>
        /// Test method that centers the camera over the gameobject and makes it look at it. For testing purposes only.
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="obj"></param>
        public static void CenterOn(this Camera cam, GameObject obj)
        {
            var b = GetBoundsWithChildren(obj);
            var v = obj.transform.position;

            cam.transform.position = v + Vector3.up * 10;
            Debug.Log(cam.transform.position);
            cam.transform.LookAt(obj.transform.position);
        }
    }
}