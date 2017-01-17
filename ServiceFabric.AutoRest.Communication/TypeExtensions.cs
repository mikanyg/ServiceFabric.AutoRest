using System;
using System.Linq;
using Microsoft.Rest;

namespace ServiceFabric.AutoRest.Communication
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Uses reflection to determine whether the type has a Credentials property, 
        /// which is added to the generated ServiceClient when "AutoRest.exe -AddCredentials true"
        /// is used to generate the client code.
        /// </summary>
        internal static bool HasCredentialsSupport(this Type serviceClientType) 
        {
            return serviceClientType.GetProperties()
                .Any(p => p.Name == "Credentials" && typeof(ServiceClientCredentials).IsAssignableFrom(p.PropertyType));
        }
    }
}