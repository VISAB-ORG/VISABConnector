namespace VISABConnector
{
    /// <summary>
    /// The interface that all classes used for sending statistics have to implement.
    /// </summary>
    public interface IVISABStatistics
    {
        /// <summary>
        /// The game of which statistics are sent.
        /// </summary>
        string Game { get; }
    }
}