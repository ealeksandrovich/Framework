namespace Framework.Tests
{
    using System.Collections.Generic;
    using System.Web.Http;

    public class TestController : ApiController
    {
        // GET api/values 
        public IEnumerable<string> Get()
        {
            return new [] { "value1", "value2" };
        }

        // GET api/values/5 
        [Authorize]
        public string Get(int id)
        {
            return "value";
        }
    }
}