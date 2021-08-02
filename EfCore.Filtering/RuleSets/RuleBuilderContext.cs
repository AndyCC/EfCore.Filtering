using System;
using System.Linq.Expressions;

namespace EfCore.Filtering.RuleSets
{
    /// <summary>
    /// Context used by a rule expression builder
    /// </summary>
    public class RuleBuilderContext
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="parameterExpression">parameter used to access for the source entity</param>
        /// <param name="targetPropertyType">Type of the target property</param>
        internal RuleBuilderContext(ParameterExpression parameterExpression, Type targetPropertyType)
        {
            ParameterExpression = parameterExpression;
            TargetPropertyType = targetPropertyType;
        }

        /// <summary>
        /// parameter used to access for the source entity
        /// </summary>
        public ParameterExpression ParameterExpression { get; private set; }

        /// <summary>
        /// Type of the target property
        /// </summary>
        public Type TargetPropertyType { get; private set; }
    }
}
