using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using BarrowHubApp.Models;

namespace BarrowHubApp.Controllers
{
    [Produces("application/json")]
    [Route("api/GMail")]

    public class GMailController : Controller
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/gmail-dotnet-quickstart.json
        static string[] Scopes = { GmailService.Scope.GmailReadonly };
        static string ApplicationName = "Gmail API .NET Quickstart";
        static string regex = @"(\$)[\d]+\.[\d]+";

        public IList<Bill> Get()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                //Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Gmail API service.
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            DateTime now = DateTime.Now;
            var request = service.Users.Messages.List("me");
            int year = now.Year;
            int month = now.Month - 3;
            if(month <= 0)
            {
                month = 12 + month;
                year--;
            }

            // Search parameters across GMail inbox
            request.Q = String.Format("from:(\"xfinity my account\" | consumers| DTE) subject:(bill|ready) after:{0}/{1}/{2}", year, month, 1);


            IList<Message> messages = request.Execute().Messages;
            IList<Bill> bills = new List<Bill>();
            if (messages != null && messages.Count > 0)
            {
                foreach (var message in messages)
                {
                    Bill b = new Bill();
                    Message m = service.Users.Messages.Get("me", message.Id).Execute();
                    MessagePart payload = m.Payload;

                    foreach (var part in payload.Headers)
                    {
                        String name = part.Name;
                        if (name.StartsWith("From"))
                        {
                            String from = "";
                            from = part.Value;
                            from = from.Substring(0, from.IndexOf('<'));
                            from = from.Replace("\"", "");
                            from = from.Replace("[", "");
                            from = from.Replace("]", "");
                            b.from = from;
                        }
                        else if (name.StartsWith("Subject"))
                        {
                            b.subject = part.Value;
                        }
                        else if (name.StartsWith("Date"))
                        {
                            String date = "";
                            date = part.Value;
                            String[] dateParts = date.Split(" ");
                            b.date = String.Format("{0} {1} {2}", dateParts[2], dateParts[1], dateParts[3]);
                        }
                    }
                    b.due = Regex.Match(m.Snippet, regex).Value;
                    b.setHalf();
                    bills.Add(b);
                }
            }
            return bills;
        }

    }
}