﻿using UnityEngine;

namespace VISABConnector.Unity
{
    public class CameraConfiguration
    {
        public float CameraOffset { get; set; }

        public bool UseAbsoluteOffset { get; set; }

        public Vector3 CameraRotation { get; set; } = new Vector3(90, 0, 0);

        public bool Orthographic { get; set; }

        public float OrthographicSize { get; set; } = 1;
    }
}
