using Microsoft.AspNetCore.SignalR.Client;
using System;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                          .WithUrl("http://localhost:5000/connect?access_token=1")
                          .Build();

            connection.StartAsync();

            Console.ReadLine();
        }
    }
}
