using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreLib;

namespace EchoMessageRule
{
    public class Implementation : ISimpleRule
    {
        public Dictionary<string, object> Execute(Dictionary<string, object> inputs)
        {
            var message = !inputs.ContainsKey("message") ? "EMPTY" : (inputs["message"] as string);

            return new Dictionary<string, object>()
            {
                {"time",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") } ,
                {"result",$"Receved: {message}, DONE" }
            };
        }
    }
}
