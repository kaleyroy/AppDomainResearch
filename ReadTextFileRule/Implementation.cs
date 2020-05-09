using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreLib;

namespace ReadTextFileRule
{
    public class Implementation : ISimpleRule
    {
        public Dictionary<string, object> Execute(Dictionary<string, object> inputs)
        {
            if (!inputs.ContainsKey("path"))
                throw new ArgumentException("Empty path!");

            string content = string.Empty;
            var path = inputs["path"] as string;
            using (var sr = new StreamReader(path))
                content = sr.ReadToEnd();

            return new Dictionary<string, object>()
            {
                {"time",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") } ,
                {"result",content }
            };
        }
    }
}
