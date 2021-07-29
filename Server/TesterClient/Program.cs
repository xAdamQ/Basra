using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Basra.Server.Exceptions;
using Microsoft.AspNetCore.SignalR.Client;

namespace TesterClient
{
    class Program
    {
        static List<Client> Clients = new();

        static async Task Main(string[] args)
        {
            ReadLine.HistoryEnabled = true;

            while (true)
            {
                var commands = string.Empty;
                for (int i = 0; i < Client.Rpcs.Length; i++) commands += $"{i} {Client.Rpcs[i]} ";
                Console.WriteLine(commands);

                var input = ReadLine.Read();

                try
                {
                    foreach (var cmd in input.Split(';'))
                    {
                        if (cmd == "nu")
                        {
                            await MakeClient();
                            break;
                        }
                        else
                        {
                            var words = cmd.Split(' ');
                            var res0 = int.TryParse(words[0], out int clientIndex);
                            var res1 = int.TryParse(words[1], out int commandIndex);
                            if (!res0 || !res1)
                            {
                                Console.WriteLine("invilde input");
                                continue;
                            }

                            Clients[int.Parse(words[0])].RpcCall(Client.Rpcs[int.Parse(words[1])], ProcessArgs(words, 2));
                        }
                    }
                }
                catch (BadUserInputException e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        static object[] ProcessArgs(string[] cmdWords, int startIndex)
        {
            var rpcArgs = new List<object>();
            for (var i = startIndex; i < cmdWords.Length; i++)
            {
                if (cmdWords[i] == "int")
                {
                    rpcArgs.Add(int.Parse(cmdWords[i + 1]));
                    i++;
                }
                else if (cmdWords[i] == "string")
                {
                    rpcArgs.Add(cmdWords[i + 1]);
                    i++;
                }
                else if (int.TryParse(cmdWords[i], out int intArg))
                {
                    rpcArgs.Add(intArg);
                }
                else
                {
                    rpcArgs.Add(cmdWords[i]);
                }
            }

            return rpcArgs.ToArray();
        }

        private static readonly ConsoleLogger ConsoleLogger = new();

        static async Task MakeClient()
        {
            var c = new Client(Clients.Count, ConsoleLogger);

            var hubConnection = new HubConnectionBuilder()
                // .ConfigureLogging(lb => lb
                //     .AddConsole()
                //     .AddDebug()
                //     .AddFilter(l => l =+= LogLevel.Information)
                // )
                .WithUrl("http://localhost:5000/connect?access_token=" + Clients.Count + 4)
                //.WithUrl("https://tstappname.azurewebsites.net/connect?access_token=" + Id + 4)
                .Build();

            Clients.Add(c);

            Console.WriteLine("a new client is made with index: " + (Clients.Count - 1));

            await c.Connect(hubConnection);
        }
    }
}