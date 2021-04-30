﻿namespace VISABConnector
{
    /// <summary>
    /// The interface all statistics objects that want to be sent to the VISAB WebApi have to implement.
    /// </summary>
    public interface IVISABStatistics
    {
        /// <summary>
        /// The game of which statistics are sent
        /// </summary>
        string Game { get; }
    }
}