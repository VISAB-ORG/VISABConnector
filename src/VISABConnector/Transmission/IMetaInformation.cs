using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VISABConnector
{
    /// <summary>
    /// The interface that all classes used for sending meta information have to implement.
    /// This information is required upon opening the session.
    /// </summary>
    public interface IMetaInformation
    {
        /// <summary>
        /// The game of which data will be sent.
        /// </summary>
        string Game { get; }
    }
}
