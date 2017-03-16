using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceFabric.AutoRest.Communication.Client
{
    public interface ICredentialsManager
    {
        Task<ServiceClientCredentials> GetCredentialsAsync();
    }
}
