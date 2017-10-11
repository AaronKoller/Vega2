using System.Collections.Generic;

namespace Vega.Core.Models
{

    public class SMSMessage
    {
        public string Phone {get; set;}
        public string Message {get; set;}
        public string Error {get; set;}
    }

    public class SMSFields : SMSMessage
    {
        public string FirstName {get; set;}
        public string LastName {get; set;}
    }

    public class SMSObject{
        public List<SMSMessage> Messages {get; set;}
        public string Password {get; set;}
        public bool isUseMockData {get; set;}
    }
}