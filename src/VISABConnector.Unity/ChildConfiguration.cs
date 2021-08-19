using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace VISABConnector.Unity
{
    /// <summary>
    /// Class that contains the settings for game objects that contain child objects that need to be snapshotted
    /// </summary>
    public class ChildConfiguration
    {
        /// <summary>
        /// The child gameobject name
        /// </summary>
        public string ChildName { get; set; }

        /// <summary>
        /// If you want to center the cam only on one child but still want to snap all of its connected gameobjects (parents, siblings) set this to true
        /// </summary>
        public bool SnapAllChilds { get; set; }
    }
}
