using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vega.Controllers.Resources;
using Vega.Core.Models;
using Vega.Core;
using Vega.Persistence;
using System.Collections.Generic;

using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using System.Reflection;
using System.Linq;

namespace Vega.Controllers
{
public class StoryAddRequest
{
    public string Action { get; set; }
}


    [Route("/api/batchSms")]
    public class BatchSmsController : Controller
    {
        // [HttpPut]
        // public async Task<IActionResult> PutSmsMessages([FromBody] StoryAddRequest smsMessages){

        //     await SendSmsMessages(smsMessages);
        //     return Ok();
        // }

        [HttpPost]
        // public async Task<IActionResult> SendSmsMessages([FromBody] IEnumerable<SmsMessage> smsMessages)
        // public async Task<IActionResult> SendSmsMessages([FromBody] IEnumerable<SmsMessage> smsMessages)
        public async Task<IActionResult> SendSmsMessages([FromBody] SMSObject smsObject)
        {
            Boolean useMockData = smsObject.isUseMockData;
            //https://www.plivo.com/pricing/JP/#!sms   .04 per sms
            //Twilio                                   .075 per sms

            if(smsObject.Password != "abc123")
            {
                return BadRequest("Invalid Password");
            }
            // Your Account SID from twilio.com/console
            var accountSid = "ACf67abf48da0c0e551fd6303b06bd42f7";
            // Your Auth Token from twilio.com/console
            var authToken = "9164d4e0c076685fcd0223f79615d114";   

            var twillioFromNumber = "+12349013723";

            // Initialize the Twilio client
            TwilioClient.Init(accountSid, authToken);

            var smsProps = getbatchSMSPeopleProperties();

            var smsObjectsProps = new SMS();
            var properties = smsObjectsProps.GetType().GetProperties();


            var errors = new List<SMS>();
            //Iterate over all our friends
            if(!useMockData){
                foreach (var person in smsObject.People)
                {
                try{
                    string newMessage = smsObject.Message;
                    foreach (var prop in properties) {
                        string v = (string)prop.GetValue(person, null);
                        newMessage = newMessage.Replace($"[{prop.Name}]", v);
                    }

                    // Send a new outgoing SMS by POSTing to the Messages resource
                    MessageResource.Create(
                        from: new PhoneNumber(twillioFromNumber), // From number, must be an SMS-enabled Twilio number
                        to: new PhoneNumber($"+{person.Phone}"), // To number, if using Sandbox see note above
                        // Message content
                        // body: $"Hey {person.FirstName} {person.LastName} Monkey Party at 6PM. Bring Bananas!  Please respond in Facebook Messenger if you get this.  Let the SPAMMING begin!");
                        body: $"{newMessage}");
                    
                    }catch(Exception e){
                        person.Message = e.Message;
                        errors.Add(person);
                    }
                };
            }else{
                errors.Add(new SMS{
                    Phone = "1239898",
                    FirstName = "Candace",
                    LastName = "Koller",
                    Message = "This is an error message."
                });
                errors.Add(new SMS{
                    Phone = "129038908",
                    FirstName = "Gabe",
                    LastName = "喬橋",
                    Message = "This is an error message."
                });
            }
            return Ok(errors);
        }
    [Route("/api/batchSms/peopleProperties")]
    [HttpGet]
    public List<string> getbatchSMSPeopleProperties(){
        PropertyInfo[] props = null;
 
        //  Get the properties for the type
        var smsObject = new SMS();

        var properties = smsObject.GetType().GetProperties();

        var SMSProperties = new List<string>();

        foreach (var p in properties)
        {
            SMSProperties.Add(p.Name);
        }
        return SMSProperties;
    }
    }
}