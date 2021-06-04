namespace VISABConnector
{
    /// <summary>
    /// The interface all classes used for sending images have to implement.
    /// </summary>
    public interface IImage
    {
        /// <summary>
        /// The game of which images are sent.
        /// </summary>
        string Game { get; }
    }
}