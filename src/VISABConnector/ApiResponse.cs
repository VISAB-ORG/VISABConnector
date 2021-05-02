using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VISABConnector
{
    /// <summary>
    /// Is returned when requests to the VISAB WebApi are made
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Whether the request was successfull
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// The message returned by VISAB WebApi
        /// </summary>
        public string Message { get; set; }
    }
}
