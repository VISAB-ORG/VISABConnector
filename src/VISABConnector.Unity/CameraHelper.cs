﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VISABConnector.Unity
{
    public static class CameraHelper
    {
        /// <summary>
        /// The name of the camera that is created by us, if no camera is given.
        /// </summary>
        public const string CameraName = "VISABConnector.Unity Camera";

        public static Camera CreateCamera()
        {
            var existing = GameObject.Find(CameraName);
            if (existing && existing.GetComponent<Camera>() != null)
                return existing.GetComponent<Camera>();

            // Create new gameObject and add camera component
            var gameObject = new GameObject(CameraName);
            var camera = gameObject.AddComponent<Camera>();
            ConfigureCamera(camera);

            return camera;
        }

        private static void ConfigureCamera(Camera camera)
        {
            // TODO: Create some class that holds constants for the default creation of the camera
        }
    }
}
