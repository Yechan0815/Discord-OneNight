using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace HuTao.Game
{
    public interface IGame
    {
        void New(SocketCommandContext _context, List<User> users);
        void Call(SocketCommandContext _context, List<User> users);
    }
 
    public class None : IGame
    {
        public void New(SocketCommandContext _context, List<User> users)
        {
            Console.WriteLine("Error");
        }

        public void Call(SocketCommandContext _context, List<User> users)
        {
            Console.WriteLine("Error");
        }
    }


}
