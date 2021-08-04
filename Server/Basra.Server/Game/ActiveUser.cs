using System;
using System.Collections.Generic;
using System.Linq;

namespace Basra.Server
{
    public class ActiveUser
    {
        public ActiveUser(string id, string connectionId, Type initialDomain)
        {
            Id = id;
            ConnectionId = connectionId;
            Domain = initialDomain;
        }

        public string Id { get; }
        public string ConnectionId { get; }
        public Type Domain { get; set; }
        public bool Disconnected { get; set; }
    }

    public abstract class UserDomain
    {
        public class App : UserDomain
        {
            public class Startup : App
            {
            }

            public class Lobby : App
            {
                public class Idle : Lobby
                {
                }

                public class Pending : Lobby
                {
                }

                public class GettingReady : Lobby
                {
                }

                public class WaitingForOthers : Lobby
                {
                }
            }

            public class Room : App
            {
                // public class FinalizingRoom : Room
                // {
                // }
                public class FinishedRoom : Room
                {
                }

                internal class Active : Room
                {
                }
            }
        }
    }
}