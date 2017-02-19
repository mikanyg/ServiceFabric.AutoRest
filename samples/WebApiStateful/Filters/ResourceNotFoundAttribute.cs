using System;
using System.Net;
using System.Web.Http.Filters;

namespace WebApiStateful.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    internal sealed class ResourceNotFoundAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext?.Response.StatusCode == HttpStatusCode.NotFound)
            {
                actionExecutedContext.Response.Headers.Add("X-ServiceFabric", "ResourceNotFound");
            }
        }
    }
}
