using System;
using System.Collections.Generic;

namespace EfCore.Filtering
{
    /// <summary>
    /// Exception thrown when there are invalid paths within a Filter
    /// </summary>
    public class InvalidPathsException : Exception
    {
        public InvalidPathsException(IDictionary<Type, string[]> invalidPaths) 
            : base("There are paths within the filter that can not be navigated")
        {
            InvalidPaths = invalidPaths;
        }

        /// <summary>
        /// List of invalid paths against the source type for each set of paths
        /// </summary>
        public IDictionary<Type, string[]> InvalidPaths { get; private set; }

        protected InvalidPathsException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
