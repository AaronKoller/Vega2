using System.Collections.Generic;

namespace Vega.Core.Models
{
    public class SMS
    {
        public string Phone {get; set;}
        public string FirstName {get; set;}
        public string LastName {get; set;}

        public string Message {get; set;}
    }

    public class SMSObject{
        public List<SMS> People {get; set;}
        public string Message {get; set;}
        public string Password {get; set;}
        public bool isUseMockData {get; set;}
    }
}