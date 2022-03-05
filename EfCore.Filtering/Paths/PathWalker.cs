using EfCore.Filtering.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EfCore.Filtering.Paths
{
    /// <summary>
    /// Walks all paths ensuring they are valid and caching them for later use
    /// </summary>
    /// <typeparam name="TSource">Type of the object the path is applied to</typeparam>
    public class PathWalker<TSource> : PathWalker
    {
        public PathWalker(Filter filter)
            : base(typeof(TSource), filter)
        {
        }
    }

    /// <summary>
    /// Walks all paths ensuring they are valid and caching them for later use
    /// </summary>
    public class PathWalker
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="sourceType">Type of the object the path is applied to</param>
        /// <param name="filter">Filter object to walk paths on and check they are valid</param>
        public PathWalker(Type sourceType, Filter filter)
        {
            _sourceType = sourceType;
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        private readonly Type _sourceType;
        private readonly Filter _filter;

        /// <summary>
        /// List of unique paths found
        /// SourceType => FullNavigationPath => Path Object
        /// SourceType is the type of the object the navigation path is applied to. e.g. in the path "Price" if "Price" is on a "Product" the "Product" would be the source type
        /// </summary>
        private IDictionary<Type, IDictionary<string, Path>> Paths { get; set; } = new Dictionary<Type, IDictionary<string, Path>>();

        /// <summary>
        /// True if all paths are valid, otherwise false
        /// </summary>
        /// <remarks>Must call WalkPaths() first</remarks>
        public bool IsValid
        {
            get
            {
                return Paths.All(x => x.Value.All(y => y.Value.IsValid));
            }
        }

        /// <summary>
        /// IDictionary<Type, string[]> Lists all invalid paths. Key is the source type of the object the path is applied to and the value is a list of invalid paths on that type
        /// </summary>
        public IDictionary<Type, string[]> InvalidPaths
        {
            get
            {
                return Paths.Where(x => x.Value.Any(x => !x.Value.IsValid))
                    .Select(x => new KeyValuePair<Type, string[]>(x.Key, x.Value.Where(y => !y.Value.IsValid).Select(x => x.Value.NavigationPath).ToArray()))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
        }

        /// <summary>
        /// Walks all the paths in a filter
        /// </summary>
        public void WalkPaths()
        {
            VisitFilter(_sourceType, _filter);
        }

        /// <summary>
        /// Visits a filter 
        /// </summary>
        /// <param name="sourceType">Type the navigation paths are applied to</param>
        /// <param name="filter">Filter to check</param>
        private void VisitFilter(Type sourceType, Filter filter)
        {
            VisitRuleSets(sourceType, filter.WhereClause);
            VisitOrdering(sourceType, filter.Ordering);
            VisitIncludes(sourceType, filter.Includes);
        }

        /// <summary>
        /// Visits the rule sets 
        /// </summary>
        /// <param name="sourceType">Type the navigation paths are applied to</param>
        /// <param name="ruleSet">RuleSet to check</param>
        private void VisitRuleSets(Type sourceType, RuleSet ruleSet)
        {
            if (ruleSet == null)
                return;

            foreach (var rule in ruleSet.Rules)
                VisitRule(sourceType, rule);

            foreach (var innerRuleSet in ruleSet.RuleSets)
                VisitRuleSets(sourceType, innerRuleSet);
        }

        /// <summary>
        /// Visits the rules
        /// </summary>
        /// <param name="sourceType">Type the navigation paths are applied to</param>
        /// <param name="rule">Rule to check</param>
        private void VisitRule(Type sourceType, Rule rule)
        {
            if (Paths.ContainsKey(sourceType) && Paths[sourceType].ContainsKey(rule.Path))
                return;

            if (!Paths.ContainsKey(sourceType))
                Paths.Add(sourceType, new Dictionary<string, Path>());

            BuildAndAddPathSplitByCollections(sourceType, rule.Path);
        }

        /// <summary>
        /// Visits the orderings
        /// </summary>
        /// <param name="sourceType">Type the navigation paths are applied to</param>
        /// <param name="ordering">List of OrderBy to check</param>
        private void VisitOrdering(Type sourceType, List<OrderBy> ordering)
        {
            if (ordering == null)
                return;

            foreach (var order in ordering)
            {
                if (!Paths.ContainsKey(sourceType))
                    Paths.Add(sourceType, new Dictionary<string, Path>());

                if (Paths[sourceType].ContainsKey(order.Path))
                    continue;

                BuildAndAddPathSplitByCollections(sourceType, order.Path);
            }
        }

        /// <summary>
        /// Constructs a path object, with path parts split on IEnumerables. Adds these to the internal collection
        /// </summary>
        /// <param name="sourceType">Type the navigation path is applied to</param>
        /// <param name="navigationPath">full navigation path</param>
        private void BuildAndAddPathSplitByCollections(Type sourceType, string navigationPath)
        {
            var path = new Path(navigationPath);

            var splitPath = path.NavigationPath.Split(PropertyPath.PathSeperator);

            //navigation path for the current path
            string currentPath = null;
            //source type for the current path part being constructed
            var currentSourceType = sourceType;
            //latet type on the path being inspected
            var latestType = sourceType;

            for (var i = 0; i < splitPath.Length; i++)
            {
                var inspectPropertyName = splitPath[i];
                var property = latestType.GetProperty(inspectPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

                if (property == null)
                {
                    path.IsValid = false;
                    Paths[sourceType].Add(path.NavigationPath, path);
                    return;
                }

                if (currentPath == null)
                    currentPath = inspectPropertyName;
                else
                    currentPath = $"{currentPath}.{inspectPropertyName}";

                if (property.PropertyType.IsAssignableTo(typeof(IEnumerable)) || i == splitPath.Length - 1)
                {
                    var pathPart = new PathPart
                    {
                        SourceType = currentSourceType,
                        FinalType = property.PropertyType,
                        Path = currentPath
                    };

                    path.PathParts.Add(pathPart);
                    currentPath = null;
                    currentSourceType = property.PropertyType.GetUnderlyingTypeIfGenericAndEnumerable() ?? property.PropertyType;
                }

                latestType = property.PropertyType.GetUnderlyingTypeIfGenericAndEnumerable() ?? property.PropertyType;
            }

            Paths[sourceType].Add(path.NavigationPath, path);
        }

        /// <summary>
        /// Visits Includes
        /// </summary>
        /// <param name="sourceType">Type the navigation paths are applied to</param>
        /// <param name="includes">List of includes</param>
        private void VisitIncludes(Type sourceType, List<Include> includes)
        {
            if (includes == null)
                return;

            foreach(var include in includes)
            {
                if (!Paths.ContainsKey(sourceType))
                    Paths.Add(sourceType, new Dictionary<string, Path>());

                if (!Paths[sourceType].ContainsKey(include.Path))
                    BuildAndAddPathWhenOnePart(sourceType, include.Path);

                if (include.Filter != null)
                {
                    var innerSourceType = GetFinalTypeOfFinalPropertyInPath(sourceType, include.Path);
                    innerSourceType = innerSourceType.GetUnderlyingTypeIfGenericAndEnumerable() ?? innerSourceType;
                    VisitFilter(innerSourceType, include.Filter);
                }
            }
        }

        /// <summary>
        /// builds a path object when there will only ever be one part in the path
        /// </summary>
        /// <param name="sourceType">Type the navigation paths are applied to</param>
        /// <param name="navigationPath">navigation path</param>
        private void BuildAndAddPathWhenOnePart(Type sourceType, string navigationPath)
        {
            var path = new Path(navigationPath);
            path.PathParts = new List<PathPart>
                {
                    new PathPart
                    {
                        Path = navigationPath,
                        SourceType = sourceType,
                        FinalType = LocateTypeOfFinalPropertyInPath(sourceType, navigationPath)
                    }
                };
            path.IsValid = path.PathParts[0].FinalType != null;
            Paths[sourceType].Add(path.NavigationPath, path);
        }

        /// <summary>
        /// Gets the type of the final property at the end of the path.
        /// A path with an enumerable is not valid
        /// </summary>
        /// <param name="sourceType">source type on which the first property is found</param>
        /// <param name="fullPath">Property path seperated by .</param>
        /// <returns>Type or null if path is invalid</returns>
        /// <remarks>
        /// The type of Property3 will be returned from the path Property1.Property2.Property3
        /// Used when building paths
        /// </remarks>
        private static Type LocateTypeOfFinalPropertyInPath(Type sourceType, string fullPath)
        {
            var parts = fullPath.Split(PropertyPath.PathSeperator);

            foreach (var part in parts)
            {
                var property = sourceType.GetProperty(part, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

                if (property == null || property.PropertyType.IsAssignableTo(typeof(Enumerable)))
                    return null;

                sourceType = property.PropertyType;
            }

            return sourceType;
        }

        /// <summary>
        /// Gets the type at the end of a path
        /// </summary>
        /// <param name="sourceType">Type the navigation paths are applied to</param>
        /// <param name="path">Navigation path supplied</param>
        /// <returns>Type</returns>
        /// <remarks>WalkPaths() must have been called before this method can be used</remarks>
        public Type GetFinalTypeOfFinalPropertyInPath(Type sourceType, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Must be set", nameof(path));

            return GetPath(sourceType, path).FinalType;
        }

        /// <summary>
        /// Returns the parts of a given path
        /// </summary>
        /// <param name="sourceType">Type the navigation paths are applied to</param>
        /// <param name="path">Navigation path supplied</param>
        /// <returns>IEnumerable of PathPart</returns>
        /// <remarks>WalkPaths() must have been called before this method can be used</remarks>
        public IEnumerable<PathPart> PathParts(Type sourceType, string path)
        {
            return GetPath(sourceType, path).PathParts;
        }

        /// <summary>
        /// Gets the Path object from the cached collection
        /// </summary>
        /// <param name="sourceType">Type the navigation paths are applied to</param>
        /// <param name="path">Navigation path supplied</param>
        /// <returns>Path</returns>
        private Path GetPath(Type sourceType, string path)
        {
            if (Paths.ContainsKey(sourceType) && Paths[sourceType].ContainsKey(path))
                return Paths[sourceType][path];

            throw new ArgumentException($"Can not find path '{path}' on type '{sourceType.FullName}'", nameof(path));
        }
    }
}
