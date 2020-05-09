using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetUserListRule
{
    public class User
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedTime { get; set; }


        public User() { }
        public User(string userName,string firstName,string lastName)
        {
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            CreatedTime = DateTime.Now;
        }
    }
}
