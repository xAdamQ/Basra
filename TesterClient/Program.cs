using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace TesterClient
{
    class Program
    {
        static List<Client> Clients = new();

        static async Task Main(string[] args)
        {
            // using StreamWriter file = new("history.txt", append: true);
            // await file.WriteLineAsync("Fourth line");
            ReadLine.HistoryEnabled = true;

            while (true)
            {
                var input = ReadLine.Read();

                foreach (var cmd in input.Split(';'))
                {
                    if (cmd == "nu")
                    {
                        MakeClient();
                        break;
                    }
                    else if (cmd.StartsWith("call "))
                    {
                        var words = cmd.Split(' ');
                        var clientIndex = int.Parse(words[1]);
                        var rpc = words[2];

                        if (words.Length != 3)
                            Clients[clientIndex].RpcCall(rpc, ProcessArgs(words, 3));
                    }
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

        static async void MakeClient()
        {
            var c = new Client(Clients.Count);
            Clients.Add(c);
            Console.WriteLine("a new client is made with index: " + (Clients.Count - 1));
            await c.Connect();
        }



    }
}