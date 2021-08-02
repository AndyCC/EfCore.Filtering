using EfCore.Filtering.Client;
using EfCore.Filtering.Paths;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EfCore.Filtering.Mvc
{
    /// <summary>
    /// ModelBinder for Filter<TEntity>
    /// Converts:
    /// query string filter={shortFilter as Json or filter as json}
    /// query string = shortFilterQueryString as a traditional query string
    /// body = {shortFilter as Json or filter as json}
    /// </summary>
    public class FilterModelBinder : IModelBinder
    {
        private readonly FilterModelBinderContext _context;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">context contining info the binder requires</param>
        public FilterModelBinder(FilterModelBinderContext context)
        {
            _context = context;
        }

        private const string queryStringKey = "filter";

        /// <summary>
        /// Binds the model
        /// </summary>
        /// <param name="bindingContext">ModelBindingContext</param>
        /// <returns>Tasl</returns>
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            Filter filter = null;

            if (bindingContext.BindingSource == BindingSource.Query && bindingContext.ActionContext.HttpContext.Request.Query.ContainsKey(queryStringKey))
                filter = BindFromQueryStringJson(bindingContext);
            else if (bindingContext.BindingSource == BindingSource.Body)
                filter = await BindFromBody(bindingContext);
            else if (bindingContext.BindingSource == BindingSource.Query)            
                filter = await BindFilterFromQueryStringPure(bindingContext);
            
            if (filter == null)
            {
                bindingContext.ModelState.AddModelError(null, "Can not bind filter");
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            var entityType = bindingContext.ModelType.GenericTypeArguments[0];
            var pathWalker = new PathWalker(entityType, filter);
            pathWalker.WalkPaths();

            if (!pathWalker.IsValid)
            {
                var invalidPaths = pathWalker.InvalidPaths.Values.SelectMany(x => x);
                bindingContext.ModelState.AddModelError(string.Empty, $"Invalid paths in request: {string.Join(",", invalidPaths)}");
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            filter.EnsureRuleValuesAreCorrectType(entityType, pathWalker);
            bindingContext.Result = ModelBindingResult.Success(filter);
        }

        /// <summary>
        /// Binds a Filter from a query string of json
        /// </summary>
        /// <param name="bindingContext">ModelBindingContext</param>
        /// <returns>Filter</returns>
        private static Filter BindFromQueryStringJson(ModelBindingContext bindingContext)
        {
            var json = bindingContext.ActionContext.HttpContext.Request.Query[queryStringKey];
            return ConvertJsonToFilter(json, bindingContext.ModelType);
        }

        /// <summary>
        /// Binds a Filter from a body of json
        /// </summary>
        /// <param name="bindingContext">ModelBindingContext</param>
        /// <returns>Task<Filter></Filter></returns>
        private static async Task<Filter> BindFromBody(ModelBindingContext bindingContext)
        {
            var json = string.Empty;

            using(var streamReader = new StreamReader(bindingContext.ActionContext.HttpContext.Request.Body))
            {
                json = await streamReader.ReadToEndAsync();
            }

            return ConvertJsonToFilter(json, bindingContext.ModelType);
        }

        /// <summary>
        /// Converts json to the given filter type
        /// </summary>
        /// <param name="json">json to convert</param>
        /// <param name="filterType">Type that derives from Filter</param>
        /// <returns></returns>
        private static Filter ConvertJsonToFilter(string json, Type filterType)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            };

            var shortFilter = JsonSerializer.Deserialize<ShortFilter>(json, jsonOptions);
            var filter = shortFilter.IsValid() ? shortFilter.ToFilter(filterType) : null;

            if (filter == null)
                filter = (Filter)JsonSerializer.Deserialize(json, filterType, jsonOptions);

            return filter;
        }

        /// <summary>
        /// Binds a Filter from a pure query string
        /// </summary>
        /// <param name="bindingContext">ModelBindingContext</param>
        /// <returns>Task<Filter></Filter></returns>
        private async Task<Filter> BindFilterFromQueryStringPure(ModelBindingContext bindingContext)
        {
            var originalModelMetaData = bindingContext.ModelMetadata;
            var filterType = bindingContext.ModelType;
            var complexTypeModelBinder = new ComplexTypeModelBinder(_context.ShortFilterQueryStringPropertyBinders, _context.LoggerFactory);
            bindingContext.ModelMetadata = _context.ShortFilterQueryStringMetaData;
            await complexTypeModelBinder.BindModelAsync(bindingContext);
            var shortFilter = bindingContext.Model as ShortFilterQueryString;
            var filter = shortFilter.ToFilter(filterType);
            bindingContext.ModelMetadata = originalModelMetaData;
            return filter;
        }
    }
}
