using Discord.Commands;
using Discord.WebSocket;
using HuTao.Game.OneNight;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace HuTao.Game
{
    public enum GameName
    {
        OneNight,
        None
    }

    public enum Status
    {
        Join,
        Werewolves,
        Minion,
        Mason,
        Seer,
        Robber,
        Troublemaker,
        Drunk,
        Insomniac,
        Ingame,
        Hunter,
        None
    }

    public class Manager
    {
        private static Manager _instance = null;

        public static string[] gameNameStr = {
        "한밤의 늑대인간"
        };

        private GameName gameName;
        private List<User> users;
        private int count;

        public SocketCommandContext channel;
        public IGame game;
        public SocketCommandContext _context;
        public Status session;

        public Manager()
        {
            users = new List<User>();
            count = 0;

            session = Status.None;
        }

        public static Manager Instance()
        {
            if (_instance == null)
                _instance = new Manager();
            return (_instance);
        }

        public void clear()
        {
            gameName = GameName.None;
            users.Clear();
            count = 0;

            session = Status.None;
        }

        public List<User> Users
        {
            get { return (users); }
            set { this.users = value; }
        }

        public GameName GameName
        {
            get { return (gameName); }
            set { this.gameName = value; }
        }

        public int Count
        {
            get { return (count); }
            set { this.count = value; }
        }

        public void setChannel(SocketCommandContext channel)
        {
            this.channel = channel;
        }

        public int Join(SocketUser socketUser)
        {
            foreach (User u in users)
            {
                if (u.UserSocket.Id == socketUser.Id)
                    return (0);
            }
            lock (users)
            {
                if (users.Count == count)
                    return (1);
                if (gameName == GameName.OneNight)
                    users.Add(new OneNightUser(socketUser));
            }
            return (2);
        }

        public void Start(SocketCommandContext socketCommandContext)
        {
            _context = socketCommandContext;
            if (gameName == GameName.OneNight)
                game = new OneNight.OneNight();
            else
                game = new None();
            game.New(_context, users);
        }
    }
}
