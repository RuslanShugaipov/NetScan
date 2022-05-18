using System.Net;

namespace Server
{
    public class ClientInfo
    {
        public ClientInfo(string address, bool status)
        {
            Address = address;
            Status = status;
            Name = "n\\a";
            OS = "n\\a";
        }

        public override string ToString()
        {
            if (Status)
                return Address + " " + Name + " " + OS + " up";
            else
                return Address + " " + Name + " " + OS + " down";
        }

        public string Address { get; set; }
        public string Name { get; set; }
        public string OS { get; set; }
        public bool Status { get; set; }
    }
}
