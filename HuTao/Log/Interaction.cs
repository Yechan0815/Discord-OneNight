using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HuTao.Game;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace HuTao.Log
{
    public class Interaction : ModuleBase<SocketCommandContext>
    {
        [Command("clientinfo")]
        public async Task UserInfoAsync(SocketUser user = null)
        {
            var userInfo = user ?? Context.Client.CurrentUser;
            await ReplyAsync($"{userInfo.Username}#{userInfo.Discriminator}");
        }

        [Command("help"), Summary("help")]
        public async Task print_help()
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.Color = new Color(139, 124, 168);
            Embed.Title = ":question: 도움말";
            Embed.Description = "" +
                "Ao" +
                "";
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        [Command("game")]
        public async Task new_game([Remainder]string input = "None")
        {
            Game.Manager manager = Game.Manager.Instance();
            EmbedBuilder Embed = new EmbedBuilder();

            if (manager.session != Status.None)
            {
                Embed.Color = new Color(143, 48, 48);
                Embed.Title = ":x: ";
                Embed.Description = "" +
                    "**새로 시작할 수 없습니다**\n\n" +
                    "";
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                return ;
            }
            if (!input.Equals("None"))
            {
                string[] value = input.Split(' ');
                manager.clear();
                manager.GameName = (GameName)(int.Parse(value[0]) - 1);
                manager.Count = int.Parse(value[1]);
                manager.session = Status.Join;

                manager.setChannel(Context);

                Embed.Color = new Color(143, 48, 48);
                Embed.Title = ":page_facing_up: 참여";
                Embed.Description = "" +
                    "**참여하실 분은 아래 명령어를 입력해주세요**\n\n" +
                    "```" +
                    "!hutao join" +
                    "```" +
                    "";
            }
            else
            {
                Embed.Color = new Color(143, 48, 48);
                Embed.Title = ":game_die: 새 게임";
                Embed.Description = "" +
                    "**아래 명령어대로 입력해주세요**\n\n" +
                    "```" +
                    "1. 한밤의 늑대인간" +
                    "```\n" +
                    "사용법```" +
                    "!hutao game [게임 번호] [인원 수 4~]" +
                    "```" +
                    "";
            }
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        [Command("join")]
        public async Task join_game()
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Game.Manager manager = Game.Manager.Instance();

            if (manager.session != Status.Join)
            {
                Embed.Color = new Color(143, 48, 48);
                Embed.Title = ":x: ";
                Embed.Description = "" +
                    "**현재 가능한 Join이 없습니다**\n\n" +
                    "```" +
                    "새로운 세션이 필요합니다" +
                    "```" +
                    "";
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                return ;
            }
            switch (manager.Join(Context.User))
            {
                case 0:
                    await Context.Channel.SendMessageAsync($"{Context.User.Mention} 이미 참여 중입니다!");
                    return ;

               case 1:
                    await Context.Channel.SendMessageAsync($"{Context.User.Mention} 참여할 수 없습니다!");
                    return ;
            }
            string userList = "\n";
            foreach (User u in manager.Users)
                userList += u.UserSocket.Username + "\n";
            Embed.Color = new Color(143, 48, 48);
            Embed.Title = $":exclamation: 참여자 ({Game.Manager.gameNameStr[(int)manager.GameName]})";
            Embed.Description = "목록 (" + manager.Users.Count + "/" + manager.Count + "명)```" + userList + "```";
            await Context.Channel.SendMessageAsync("", false, Embed.Build());

            if (manager.Users.Count == manager.Count)
            {
                userList = "\n";
                foreach (User u in manager.Users)
                    userList += u.UserSocket.Mention + " ";
                Embed.Color = new Color(143, 48, 48);
                Embed.Title = $":exclamation: 참여 ({Game.Manager.gameNameStr[(int)manager.GameName]})";
                Embed.Description = "**곧 게임이 시작됩니다!**\n\n" + userList;
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                manager.Start(Context);
            }
        }
    }
}
