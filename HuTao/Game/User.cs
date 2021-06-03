using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace HuTao.Game
{
    public class User
    {
        protected SocketUser userSocket;

        public SocketUser UserSocket
        {
            get { return (userSocket); }
        }
    }

    public class OneNightUser : User
    {
        public OneNight.OneNight.Role role;
        public bool vote;

        public OneNightUser(SocketUser socketUser)
        {
            userSocket = socketUser;
            vote = false;
        }
    }
}
