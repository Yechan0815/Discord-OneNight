using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.DependencyInjection;

namespace HuTao
{
    class Program
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = Discord.LogSeverity.Info
            });

            _commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                LogLevel = Discord.LogSeverity.Debug
            });

            _services = new ServiceCollection()
                .BuildServiceProvider();

            _client.MessageReceived += _client_MessageReceived;
            _client.Log += Client_Log;

            string Token = "fill the TOKEN";

            _client.MessageUpdated += MessageUpdated;

            await _client.LoginAsync(Discord.TokenType.Bot, Token);
            await _client.StartAsync();

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            await Task.Delay(-1);
        }

        private async Task Client_Log(Discord.LogMessage Message)
        {
            Console.WriteLine($"{DateTime.Now} {Message.Source}] {Message.Message}");
            //SocketGuild Guild = _client.Guilds.Where(x => x.Id == 600266040101830656).FirstOrDefault();
            //SocketTextChannel Channel = Guild.Channels.Where(x => x.Id == 600266040101830658).FirstOrDefault() as SocketTextChannel;
            //await Channel.SendMessageAsync($"{DateTime.Now} at {Message.Source}] {Message.Message}");
        }

        private async Task _client_MessageReceived(SocketMessage MessageParam)
        {
            var Message = MessageParam as SocketUserMessage;
            var Context = new SocketCommandContext(_client, Message);

            Console.WriteLine(Message.Content);

            if (Context.Message == null || Context.Message.Content == "") return;
            if (Context.User.IsBot) return;

            Console.WriteLine($"{DateTime.Now} at _commands] {Message.Content}");

            int ArgPos = 0;

            if (!(Message.HasStringPrefix("!hutao ", ref ArgPos) || Message.HasMentionPrefix(_client.CurrentUser, ref ArgPos))) return;

            var Result = await _commands.ExecuteAsync(Context, ArgPos, _services);

            if (!Result.IsSuccess)
                Console.WriteLine($"{DateTime.Now} at _commands] Error. Text : {Context.Message.Content}\nError : {Result.ErrorReason}");

        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            var message = await before.GetOrDownloadAsync();
            Console.WriteLine($"{message} -> {after}");
        }

    }
}
