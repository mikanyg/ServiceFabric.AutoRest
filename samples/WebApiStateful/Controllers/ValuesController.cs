using System;
using System.Collections.Generic;
using System.Fabric;
using System.Web.Http;
using Microsoft.Owin;

namespace WebApiStateful.Controllers
{
    [ServiceRequestActionFilter]
    public class ValuesController : ApiController
    {
        // GET api/values 
        public IEnumerable<string> GetAll()
        {
            return new string[] { "statefullValue1", "statefullValue2" };
        }

        // GET api/values/5 
        public string Get(int id)
        {
            return $"statefullValue: time is {DateTime.Now} and internal path is: {Request.RequestUri.PathAndQuery}";
        }

        // POST api/values 
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5 
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5 
        public void Delete(int id)
        {
        }
    }
}
