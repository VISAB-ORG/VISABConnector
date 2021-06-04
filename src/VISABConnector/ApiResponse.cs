namespace VISABConnector
{
    /// <summary>
    /// Is returned when requests to the VISAB WebApi are made. <typeparamref name="T">The type of
    /// the requests responses content.</typeparamref>
    /// </summary>
    public class ApiResponse<T>
    {
        /// <summary>
        /// The content of the response.
        /// </summary>
        public T Content { get; set; }

        /// <summary>
        /// Whether the request was successful.
        /// </summary>
        public bool IsSuccess { get; set; }
    }
}