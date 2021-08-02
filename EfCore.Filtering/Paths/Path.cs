using System;
using System.Collections.Generic;
using System.Linq;

namespace EfCore.Filtering.Paths
{
    /// <summary>
    /// Represents a navigation path used within various part in the filter
    /// </summary>
    public class Path
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationPath">navigation mathod</param>
        internal Path(string navigationPath)
        {
            NavigationPath = navigationPath;
            PathParts = new List<PathPart>();
            IsValid = true;
        }

        /// <summary>
        /// Navigation Path
        /// </summary>
        public string NavigationPath { get; internal set; }

        /// <summary>
        /// Parts that make up a path. A part is defined as a path up until an IEnumerable, then a new part will be created up until the next IEnumerable and so forth.
        /// For example A.B.List1.C.D would create 2 parts -> A.B.List1 And C.D
        /// </summary>
        public List<PathPart> PathParts { get; internal set; }

        /// <summary>
        /// true if the path is valid, otherwise false
        /// </summary>
        public bool IsValid { get; internal set; }

        /// <summary>
        /// Type type of the final property in the path
        /// </summary>
        internal Type FinalType
        {
            get
            {
                return PathParts.Last().FinalType;
            }
        }
    }
}
