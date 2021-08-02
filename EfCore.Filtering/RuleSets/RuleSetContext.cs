using EfCore.Filtering.Client;
using EfCore.Filtering.Paths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EfCore.Filtering.RuleSets
{
    /// <summary>
    /// Context used to build a ruleset
    /// </summary>
    public class RuleSetContext
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="parameterExpression">parameter used to access for the source entity</param>
        /// <param name="pathWalker">Path Walker for use with paths in the rules</param>
        internal RuleSetContext(ParameterExpression parameterExpression, PathWalker pathWalker)
        {
            ParameterExpression = parameterExpression;
            PathWalker = pathWalker;
        }

        /// <summary>
        /// parameter used to access for the source entity
        /// </summary>
        public ParameterExpression ParameterExpression { get; private set; }

        /// <summary>
        /// Path Walker for use with paths in the rules
        /// </summary>
        public PathWalker PathWalker { get; private set; }

        /// <summary>
        /// Gets the path parts for a given path that is accessed from the Type of the parameter in the parameter expression
        /// </summary>
        /// <param name="path">navigation path</param>
        /// <returns>List of PathPart</returns>
        public List<PathPart> GetPathParts(string path)
        {
              return PathWalker.PathParts(ParameterExpression.Type, path).ToList();
        }
    }
}
