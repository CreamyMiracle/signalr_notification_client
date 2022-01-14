using System;
using Microsoft.AspNetCore.SignalR.Client;
using Refit;
using Common;

namespace NotificationPublisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Init().Wait();
            Console.ReadLine();
        }

        internal static async Task Init()
        {
            Console.WriteLine("Give Notification Server base url (f.ex. http://91.154.11.192:4444)");
            string baseUrl = Console.ReadLine();

            // Instantiating API clients
            INotificationsAPI notificationAPI = RestService.For<INotificationsAPI>(baseUrl);
            IHubsAPI hubsAPI = RestService.For<IHubsAPI>(baseUrl);

            // Getting existing Hubs
            Console.WriteLine("Hubs provided by the Notification Server:");
            foreach (string hubname in await hubsAPI.GetHubs())
            {
                Console.WriteLine(hubname);
            }

            Console.WriteLine("Select a Hub to connect to");
            string selectedHub = Console.ReadLine();

            Console.WriteLine("Select space delimited groups to join");
            string groups = Console.ReadLine();

            Console.WriteLine("Give a name to this publisher");
            string pubName = Console.ReadLine();

            while (true)
            {
                Console.WriteLine("Write a notification to be sent");
                var notification = Console.ReadLine();

                foreach (string group in groups.Split(" "))
                {
                    Console.WriteLine(string.Format("Notification sent to Hub \"{0}\" to group \"{1}\"", selectedHub, group));

                    await notificationAPI.PostNotification(group, new Payload() { Content = notification, SenderId = pubName });
                }
            }
        }
    }
}