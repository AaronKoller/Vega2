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
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace Vega.Controllers
{

    [Route("/api/batchSms")]
    public class BatchSmsController : Controller
    {
        // Place to store the Config object and use in this controller
        private readonly IConfiguration _config;
        public BatchSmsController(IConfiguration config)
        {
            _config = config;
        }

        // [HttpPost]
        // [Route("/api/batchSms1")]
        // public async Task<IActionResult> CreateProductAsync()
        // {
        //     var x = _config["SMS:TwilioAPIKey"];
        //     PlivioSMSPost smsPost = new PlivioSMSPost{
        //         src = "15093809676",
        //         dst = "19712830079",
        //         text = "This is a first test."
        //     };

        //     string auth_id = "MAMTBLMZHHN2MWMZRINJx";
        //     string token_id = "OWI0YWVkM2FiYzM5YjBkMTM4YTI5YjI5YmFhNDdm";

        //     HttpClient client = new HttpClient();
        //     client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",Convert.ToBase64String(Encoding.UTF8.GetBytes($"{auth_id}:{token_id}")));
        // // new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", auth_id, token_id))));
        //     string url = $"https://api.plivo.com/v1/Account/{auth_id}/Message/";
        //     var stringContent = new StringContent(JsonConvert.SerializeObject(smsPost), Encoding.UTF8, "application/json");
        //     var response = await client.PostAsync(url, stringContent);


        //     var response2 = await client.PostAsync("api/products", stringContent);
        //     response.EnsureSuccessStatusCode();

        //     // Return the URI of the created resource.
        //     return Ok();
        // }




        //Sends SMS posts to Twilio.  Uses really bad authentication just to prevent anyone from unauthorized use.
        [HttpPost]
        public IActionResult SendSmsMessages([FromBody] SMSObject smsObject)
        {
            var x = _config["SMS:TwilioAPIKey"];
            Boolean useMockData = smsObject.isUseMockData;
            //https://www.plivo.com/pricing/JP/#!sms   .04 per sms
            //Twilio                                   .075 per sms

            //super bad super simple authentication - REFACTOR to use [Authorize]
            if (smsObject.Password != "abc123")
            {
                return BadRequest("Invalid Password");
            }
            //Ryan correct number: 819066247663,Ryan,Hagglund, banana

            // Your Account SID from twilio.com/console
            //Aaron
            // var accountSidTwilio = "ACf67abf48da0c0e551fd6303b06bd42f7";
            // var authTokenTwilio = "9164d4e0c076685fcd0223f79615d114";
            var accountSidTwilio = _config["SMS:Twilio:AccountSid"];
            var authTokenTwilio = _config["SMS:Twilio:AuthToken"];

            //Ryan
            // accountSidTwilio = "AC5a8e9ca61e822d8c8fe813b505bcee64";
            // authTokenTwilio = "22d62a2f7fbfcc6281db3b7a5b7d5207";

            //var fromNumberTwilio = "+12349013723";
            var fromStringTwilio = _config["SMS:Twilio:FromString"];
            var fromNumberTwilio = _config["SMS:Twilio:FromNumber"]; //ok
            //var fromNumberTwilio = "MyEigo";
            var errors = new List<SMSMessage>();

            // Initialize the Twilio client
            TwilioClient.Init(accountSidTwilio, authTokenTwilio);

            //Iterate over all messages
            if (!useMockData)
            {
                foreach (var message in smsObject.Messages)
                {
                    try //Apha From:
                    {
                        MessageResource.Create(
                            from: new PhoneNumber(fromStringTwilio), // From number, must be an SMS-enabled Twilio number
                            to: new PhoneNumber($"+{message.Phone}"), // To number, if using Sandbox see note above
                            body: $"{message.Message}");
                    }
                    catch (Twilio.Exceptions.ApiException e)
                    {
                        if (e.Code == 21612)
                        {
                            try //numeric only from
                            {
                                MessageResource.Create(
                                    from: new PhoneNumber(fromNumberTwilio), // From number, must be an SMS-enabled Twilio number
                                    to: new PhoneNumber($"+{message.Phone}"), // To number, if using Sandbox see note above
                                    body: $"{message.Message}");
                            }
                            catch (Exception ex)
                            {
                                errors.Add(new SMSMessage { Error = ex.Message, Phone = message.Phone });
                            }
                        }else{
                            errors.Add(new SMSMessage { Error = e.Message, Phone = message.Phone });                          
                        }
                    }
                    catch (Exception e)
                    {
                        errors.Add(new SMSMessage { Error = e.Message, Phone = message.Phone });
                    }
                }
            }

            //if mock data then send sample errors
            else
            {
                errors.Add(new SMSMessage
                { 
                    Phone = "81906624",
                    Error = "This is an error1"
                });
                errors.Add(new SMSMessage
                {
                    Phone = "129038908",
                    Error = "This is an error2"
                });
            }
            return Ok(errors);
        }

        //using reflection, this will get the SMS Fields the front end needs to build its messages.
        //In the future this will come from the database but for now the model is good enough
        [Route("/api/batchSms/getSMSFields")]
        [HttpGet]
        public List<string> getSMSFields()
        {
            //do not allow these fields/tokens to be used.
            var ignoreFields = new List<string>{
                "Message",
                "Error"
            };
            //  Get the properties for the type
            var fields = new SMSFields().GetType().GetProperties();

            var resultFields = fields
                    .Where(p => !ignoreFields
                        .Any(x => x == p.Name))
                    .Select(c => c.Name)
                    .ToList();
            return resultFields;
        }
    }
}