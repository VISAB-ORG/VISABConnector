using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VISABConnector.Unity
{
    public static class CameraExtensionMethods
    {
        public static string SnapshotName(int width, int height)
        {
            return string.Format("{0}/Snapshots/minimap_{1}x{2}_{3}.png", Application.dataPath, width, height, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        }

    }
}
