using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib
{
    public interface ISimpleRule
    {
        Dictionary<string, object> Execute(Dictionary<string, object> inputs);
    }
}
