namespace VISABConnector
{
    /// <summary>
    /// Represents a response that is made by the VISAB WebApi.
    /// <typeparam name="T">The type of the content</typeparam>
    /// </summary>
    public class ApiResponse<T>
    {
        /// <summary>
        /// The content of the response.
        /// </summary>
        public T Content { get; internal set; }

        /// <summary>
        /// The error message that was returned by VISAB WebApi if request wasnt successful.
        /// </summary>
        public string ErrorMessage { get; internal set; }

        /// <summary>
        /// Whether the request was successful.
        /// </summary>
        public bool IsSuccess { get; internal set; }
    }
}