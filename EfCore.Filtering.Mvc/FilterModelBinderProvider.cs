using EfCore.Filtering.Client;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace EfCore.Filtering.Mvc
{
    /// <summary>
    /// FilterModelBinderProvider
    /// </summary>
    public class FilterModelBinderProvider : IModelBinderProvider
    {
        /// <summary>
        /// Gets the FitlerModelBinder when the model type is generic and is an implementation of Filter<>
        /// </summary>
        /// <param name="context">ModelBinderProviderContext</param>
        /// <returns>IModelBinder</returns>
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType.IsGenericType && context.Metadata.ModelType.GetGenericTypeDefinition() == typeof(Filter<>))
            {
                var loggerFactory = (ILoggerFactory)context.Services.GetService(typeof(ILoggerFactory));

                var mdProvider = (IModelMetadataProvider)context.Services.GetService(typeof(IModelMetadataProvider));
                var shortFilterQueryStringMetaData = mdProvider.GetMetadataForType(typeof(ShortFilterQueryString));

                var propertyBinders = new Dictionary<ModelMetadata, IModelBinder>();

                for (var i = 0; i < shortFilterQueryStringMetaData.Properties.Count; i++)
                {
                    var property = shortFilterQueryStringMetaData.Properties[i];
                    propertyBinders.Add(property, context.CreateBinder(property));
                }
                
                return new FilterModelBinder(new FilterModelBinderContext(shortFilterQueryStringMetaData, propertyBinders, loggerFactory));
            }

            return null;
        }
    }
}
