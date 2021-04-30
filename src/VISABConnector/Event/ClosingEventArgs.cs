namespace VISABConnector
{
    /// <summary>
    /// Event args for when the IVISABSession being closed
    /// </summary>
    public class ClosingEventArgs
    {
        /// <summary>
        /// The RequestHandler currently in use by the IVISABSession object
        /// </summary>
        public IVISABRequestHandler RequestHandler { get; set; }
    }
}