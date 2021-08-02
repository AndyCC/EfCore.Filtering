using System;
using System.Collections.Generic;
using System.Linq;

namespace EfCore.Filtering.Paths
{
    /// <summary>
    /// Part of a path
    /// </summary>
    public class PathPart
    {
        /// <summary>
        /// Ctor
        /// </summary>
        internal PathPart()
        {
        }

        /// <summary>
        /// Type of firt property in the path part
        /// </summary>
        public Type SourceType { get; internal set; }

        /// <summary>
        /// Type of the final property in the path part
        /// </summary>
        public Type FinalType { get; internal set; }

        /// <summary>
        /// The navigation path
        /// </summary>
        public string Path { get; internal set; }
    }
}
