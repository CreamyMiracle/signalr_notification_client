using System;
using Common;
using Microsoft.AspNetCore.SignalR.Client;
using Refit;

namespace NotificationSubscriber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Init().Wait();
            Console.ReadKey();
        }

        internal static async Task Init()
        {
            Console.WriteLine("Give Notification Server base url (f.ex. http://91.154.11.192:4444)");
            string baseUrl = Console.ReadLine();

            // Instantiating API clients
            INotificationsAPI notificationAPI = RestService.For<INotificationsAPI>(baseUrl);
            IHubsAPI hubsAPI = RestService.For<IHubsAPI>(baseUrl);


            // Getting existing Hubs
            Console.WriteLine("Hubs provided by the MessageBroker server:");
            foreach (string hubname in await hubsAPI.GetHubs())
            {
                Console.WriteLine(hubname);
            }

            Console.WriteLine("Select a Hub to connect to");
            string selectedHub = Console.ReadLine();

            // Making connection
            await using HubConnection connection = new HubConnectionBuilder().WithUrl(new Uri(baseUrl + selectedHub)).WithAutomaticReconnect().Build();
            await connection.StartAsync();

            // Configuring listener
            connection.On<Payload>("client_side_method_identifier", client_side_method_identifier);



            // Joining groups
            Console.WriteLine("Select space delimited groups to join");
            string groups = Console.ReadLine();
            foreach (string group in groups.Split(" "))
            {
                await connection.SendAsync("JoinGroup", group);
            }

            foreach (string group in groups.Split(" "))
            {
                IEnumerable<Payload> msgs = await notificationAPI.GetNotifications(group);
                if (msgs.Any())
                {
                    Console.WriteLine(string.Format("Message sent to group \"{0}\" while you were gone", group));
                    foreach (Payload msg in msgs)
                    {
                        client_side_method_identifier(msg);
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("No messages sent to group \"{0}\" while you were gone", group));
                }
            }

            await Task.Run(() =>
            {
                while(true)
                {
                    Thread.Sleep(5);
                }
            }); ;
        }

        public static void client_side_method_identifier(Payload message)
        {
            Console.WriteLine(string.Format(">> {0} publisher \"{1}\" says \"{2}\"", message.Timestamp, message.SenderId, message.Content));
        }
    }
}