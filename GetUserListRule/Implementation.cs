using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreLib;
using Newtonsoft.Json;

namespace GetUserListRule
{
    public class Implementation : ISimpleRule
    {
        public Dictionary<string, object> Execute(Dictionary<string, object> inputs)
        {
            var count = !inputs.ContainsKey("count") ? 100 : Convert.ToInt32(inputs["count"]);

            var users = new List<User>();
            for (int i = 0; i < count; i++)
                users.Add(new User($"User-{i}", $"FirstName-{i}", $"LastName-{i}"));

            return new Dictionary<string, object>()
            {
                {"time",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") } ,
                {"users", JsonConvert.SerializeObject(users) }
            };
        }
    }
}
