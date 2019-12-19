using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogDemo.Api.Helpers
{
    [AttributeUsage(AttributeTargets.All , Inherited =true,AllowMultiple =true)]
    public class RequestHeaderMatchingMediaTypeAttribute : Attribute , IActionConstraint
    {
        private readonly string _requestHeaderToMatch;
        private readonly string[] _mediaTypes;

        public RequestHeaderMatchingMediaTypeAttribute(string requestHeaderToMathch , string[] mediaTypes)
        {
            _requestHeaderToMatch = requestHeaderToMathch;
            _mediaTypes = mediaTypes;
        }

        public bool Accept(ActionConstraintContext context)
        {
            var requestHeaders = context.RouteContext.HttpContext.Request.Headers;
            if (!requestHeaders.ContainsKey(_requestHeaderToMatch))
                return false;
            foreach(var mediaType in _mediaTypes)
            {
                var mediaTypeMatchs = string.Equals(requestHeaders[_requestHeaderToMatch].ToString(), mediaType, StringComparison.OrdinalIgnoreCase);
                if (mediaTypeMatchs)
                    return true;

            }
            return false;
        }

        public int Order { get; } = 0;
    }
}
