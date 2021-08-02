using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace EfCore.Filtering.Mvc
{
    /// <summary>
    /// Context for the FilterModelBinder
    /// </summary>
    public class FilterModelBinderContext
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="shortFilterQueryStringMetaData">ModelMetadata used to convert a querystring into a ShortFilterQueryString object</param>
        /// <param name="shortFilterQueryStringPropertyBinders">ModelBinders for the properties on a ShortFilterQueryString object</param>
        /// <param name="loggerFactory">LoggerFactory</param>
        public FilterModelBinderContext (ModelMetadata shortFilterQueryStringMetaData, 
                                         IDictionary<ModelMetadata, IModelBinder> shortFilterQueryStringPropertyBinders, 
                                         ILoggerFactory loggerFactory)
        {
            ShortFilterQueryStringMetaData = shortFilterQueryStringMetaData;
            ShortFilterQueryStringPropertyBinders = shortFilterQueryStringPropertyBinders;
            LoggerFactory = loggerFactory;
        }

        /// <summary>
        /// ModelMetadata used to convert a querystring into a ShortFilterQueryString object
        /// </summary>
        public ModelMetadata ShortFilterQueryStringMetaData { get; private set; }
        /// <summary>
        /// ModelBinders for the properties on a ShortFilterQueryString object
        /// </summary>
        public IDictionary<ModelMetadata, IModelBinder> ShortFilterQueryStringPropertyBinders { get; private set; }

        /// <summary>
        /// LoggerFactory
        /// </summary>
        public ILoggerFactory LoggerFactory { get; private set; }
    }
}
