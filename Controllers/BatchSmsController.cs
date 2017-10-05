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
            Boolean useMockData = true;
            //https://www.plivo.com/pricing/JP/#!sms   .04 per sms
            //Twilio                                   .075 per sms

            // Your Account SID from twilio.com/console
            var accountSid = "ACf67abf48da0c0e551fd6303b06bd42f7";
            // Your Auth Token from twilio.com/console
            var authToken = "9164d4e0c076685fcd0223f79615d114";   

            var twillioFromNumber = "+12349013723";

            // Initialize the Twilio client
            TwilioClient.Init(accountSid, authToken);

            var errors = new List<SMS>();
            //Iterate over all our friends
            if(!useMockData){
                foreach (var person in smsObject.People)
                {
                try{
                    // Send a new outgoing SMS by POSTing to the Messages resource
                    MessageResource.Create(
                        from: new PhoneNumber(twillioFromNumber), // From number, must be an SMS-enabled Twilio number
                        to: new PhoneNumber(person.Phone), // To number, if using Sandbox see note above
                        // Message content
                        body: $"Hey {person.FirstName} {person.LastName} Monkey Party at 6PM. Bring Bananas!  Please respond in Facebook Messenger if you get this.  Let the SPAMMING begin!");
                    
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
    }
}