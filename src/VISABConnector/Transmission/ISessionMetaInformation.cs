using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VISABConnector
{
    /// <summary>
    /// The interface that all classes used for sending sesison meta information have to implement.
    /// </summary>
    public interface ISessionMetaInformation
    {
        /// <summary>
        /// The game of which data will be sent.
        /// </summary>
        string Game { get; }
    }
}
