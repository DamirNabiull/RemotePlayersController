using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PlayersController
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IP { get; set; }
        public string Port { get; set; }
        public string MAC { get; set; }
    }

    public class Checker
    {
        public Checker()
        {

        }

        public bool check_Ip(string ip)
        {
            Regex rgx = new Regex(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
            return rgx.IsMatch(ip);
        }

        public bool check_Port(string port)
        {
            Regex rgx = new Regex(@"^([1-9][0-9][0-9][0-9]|[1-3][0-9][0-9][0-9][0-9])$");
            return rgx.IsMatch(port);
        }

        public bool check_Name(string name)
        {
            string new_name = Regex.Replace(name, @"\s+", "");
            return new_name.Length > 0;
        }
    }
}
