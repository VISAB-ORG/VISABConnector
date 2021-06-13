using System.Text;

namespace VISABConnector
{
    /// <summary>
    /// Contains default values used for communication with the VISAB WebApi.
    /// </summary>
    public static class Default
    {
        /// <summary>
        /// Default content media type.
        /// </summary>
        public const string ContentMediaType = "application/json";

        /// <summary>
        /// The default host name that VISAB is running on.
        /// </summary>
        public const string HostName = "http://localhost";

        /// <summary>
        /// The default port that VISAB is running on.
        /// </summary>
        public const int Port = 2673;

        /// <summary>
        /// Default timeout for HTTP Requests in seconds.
        /// </summary>
        public const int RequestTimeout = 5;

        /// <summary>
        /// Default Encoding of content sent and received by VISAB.
        /// </summary>
        public static readonly Encoding Encoding = Encoding.UTF8;
    }
}