using System;

namespace VISABConnector
{
    /// <summary>
    /// Represents the status of a transmission session at the VISAB WebApi.
    /// </summary>
    public class SessionStatus
    {
        /// <summary>
        /// The Id of the session.
        /// </summary>
        public Guid SessionId { get; internal set; }

        /// <summary>
        /// The game of the session.
        /// </summary>
        public string Game { get; internal set; }

        /// <summary>
        /// Whether the session is active.
        /// </summary>
        public bool IsActive { get; internal set; }

        /// <summary>
        /// The total count of requests that were made by the session.
        /// </summary>
        public int TotalRequests { get; internal set; }

        /// <summary>
        /// The total count of requests for sending statistics that were made by the session.
        /// </summary>
        public int ReceivedStatistics { get; internal set; }

        /// <summary>
        /// The total count of requests for sending images that were made by the session.
        /// </summary>
        public int ReceivedImages { get; internal set; }

        /// <summary>
        /// The host name of the machine that opened the session.
        /// </summary>
        public string HostName { get; internal set; }

        /// <summary>
        /// The ip of the machine that opened the session.
        /// </summary>
        public string Ip { get; internal set; }

        /// <summary>
        /// The time at which the session was opened.
        /// </summary>
        public DateTime SessionOpened { get; internal set; }

        /// <summary>
        /// The time at which the session was closed.
        /// </summary>
        public DateTime SessionClosed { get; internal set; }

        /// <summary>
        /// The time at which the last request was made by the session.
        /// </summary>
        public DateTime LastRequest { get; internal set; }
    }
}