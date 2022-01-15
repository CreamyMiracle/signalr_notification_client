using System;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;
using Refit;

namespace NotificationSubscriber
{
    internal class Program
    {
        private static string _hubName = "/notifications";

        static void Main(string[] args)
        {
            Init().Wait();
            Console.ReadKey();
        }

        internal static async Task Init()
        {
            Console.WriteLine("Give Notification Server base url (f.ex. http://91.154.11.192:4444)");
            string baseUrl = Console.ReadLine();
            INotificationsAPI notificationAPI = RestService.For<INotificationsAPI>(baseUrl);


            // Making connection
            await using HubConnection connection = new HubConnectionBuilder().WithUrl(new Uri(baseUrl + _hubName)).WithAutomaticReconnect().Build();
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
                foreach (Payload msg in await notificationAPI.GetNotifications(group))
                {
                    client_side_method_identifier(msg);
                }
            }

            await Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(5);
                }
            });
        }

        public static void client_side_method_identifier(Payload message)
        {
            Console.WriteLine(JsonSerializer.Serialize(message));
        }
    }
}