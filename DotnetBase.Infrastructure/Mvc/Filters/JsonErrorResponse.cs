namespace DotnetBase.Infrastructure.Mvc.Filters
{
    using System;

    /// <summary>
    /// The json error response object.
    /// </summary>
    public class JsonErrorResponse
    {
        /// <summary>
        /// Gets or Sets the json error message.
        /// </summary>
        public string[] Messages { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or Sets the json error trace message.
        /// </summary>
        public object DeveloperMessage { get; set; } = new object();
    }
}
