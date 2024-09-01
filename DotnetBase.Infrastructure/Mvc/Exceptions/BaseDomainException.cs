namespace DotnetBase.Infrastructure.Mvc.Exceptions
{
    using System;

    /// <summary>
    ///    Exception type for app exceptions.
    /// </summary>
    /// <seealso cref="System.Exception"/>
    public class BaseDomainException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDomainException"/> class.
        /// </summary>
        public BaseDomainException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDomainException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public BaseDomainException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDomainException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public BaseDomainException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
