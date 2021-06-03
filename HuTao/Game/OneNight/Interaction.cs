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

namespace HuTao.Game.OneNight
{
    public class Interaction : ModuleBase<SocketCommandContext>
    {
        private async void Werewolves(string input)
        {
            Game.Manager manager = Game.Manager.Instance();
            int pos;

            if (!OneNight.EqualsUserRole(Context.User, OneNight.EnumRole.Werewolves))
            {
                await Context.User.SendMessageAsync(Context.User.Mention + " 사용할 수 없습니다!");
                return;
            }
            if (!(input.Equals("left") || input.Equals("mid") || input.Equals("right")))
            {
                await Context.User.SendMessageAsync(Context.User.Mention + " left, mid, right 중에 선택하여 주세요!");
                return;
            }
            pos = 0;
            if (input.Equals("left"))
                pos = 0;
            else if (input.Equals("mid"))
                pos = 1;
            else if (input.Equals("right"))
                pos = 2;
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.Color = new Color(143, 48, 48);
            Embed.Title = $":wolf: {( new string[] { "왼쪽", "가운데", "오른쪽"} )[pos]} 카드";
            Embed.Description = $"**{((OneNight)manager.game).tableCard[pos].name}**";
            Embed.WithImageUrl(((OneNight)manager.game).tableCard[pos].profile);
            await Context.User.SendMessageAsync("", false, Embed.Build());
            manager.session = Status.Minion;
            manager.game.Call(Context, manager.Users);
        }

        private async void Seer(string value1, string value2)
        {
            Game.Manager manager = Game.Manager.Instance();
            string[] value;
            int pos;

            if (value2.Equals(""))
            {
                value = new string[1];
                value[0] = value1;
            }
            else
            {
                value = new string[2];
                value[0] = value1;
                value[1] = value2;
            }
            if (!OneNight.EqualsUserRole(Context.User, OneNight.EnumRole.Seer))
            {
                await Context.User.SendMessageAsync(Context.User.Mention + " 사용할 수 없습니다!");
                return;
            }
            if (value.Length != 1 && value.Length != 2)
            {
                await Context.User.SendMessageAsync(Context.User.Mention + " 제대로 입력해 주세요!");
                return;
            }
            if (value.Length == 1)
            {
                OneNightUser user;
                if (Context.User.Username.Equals(value[0]) || (user = OneNight.getUserName(value[0])) == null)
                {
                    await Context.User.SendMessageAsync(Context.User.Mention + " 제대로 입력해 주세요!");
                    return;
                }
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.Color = new Color(143, 48, 48);
                Embed.Title = $":seedling: 카드";
                Embed.Description = $"**{user.UserSocket.Username}의 카드를 예언합니다!**\n\n" +
                    $"{user.role.name}";
                Embed.WithImageUrl(user.role.profile);
                await Context.User.SendMessageAsync("", false, Embed.Build());

                manager.session = Status.Robber;
                manager.game.Call(Context, manager.Users);
            }
            if (value.Length == 2)
            {
                for (int i = 0; i < 2; i++)
                    if (!(value[i].Equals("left") || value[i].Equals("mid") || value[i].Equals("right")))
                    {
                        await Context.User.SendMessageAsync(Context.User.Mention + " left, mid, right 중에 선택하여 주세요!");
                        return;
                    }
                for (int i = 0; i < 2; i++)
                {
                    pos = 0;
                    if (value[i].Equals("left"))
                        pos = 0;
                    else if (value[i].Equals("mid"))
                        pos = 1;
                    else if (value[i].Equals("right"))
                        pos = 2;
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.Color = new Color(143, 48, 48);
                    Embed.Title = $":seedling: {(new string[] { "왼쪽", "가운데", "오른쪽" })[pos]} 카드";
                    Embed.Description = $"**{((OneNight)manager.game).tableCard[pos].name}**";
                    Embed.WithImageUrl(((OneNight)manager.game).tableCard[pos].profile);
                    await Context.User.SendMessageAsync("", false, Embed.Build());
                }

                manager.session = Status.Robber;
                manager.game.Call(Context, manager.Users);
            }
        }

        private async void Robber(string input)
        {
            Game.Manager manager = Game.Manager.Instance();

            if (!OneNight.EqualsUserRole(Context.User, OneNight.EnumRole.Robber))
            {
                await Context.User.SendMessageAsync(Context.User.Mention + " 사용할 수 없습니다!");
                return;
            }

            OneNightUser user;
            if (Context.User.Username.Equals(input) || (user = OneNight.getUserName(input)) == null)
            {
                await Context.User.SendMessageAsync(Context.User.Mention + " 제대로 입력해 주세요!");
                return;
            }
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.Color = new Color(143, 48, 48);
            Embed.Title = $":moneybag: 강도";
            Embed.Description = $"**{user.role.name}과 카드를 교환합니다!**\n\n" +
                $"이제부터 당신은 {user.role.name}입니다";
            Embed.WithImageUrl(user.role.profile);
            await Context.User.SendMessageAsync("", false, Embed.Build());

            OneNight.changeUserRole(Context.User, user.UserSocket);

            manager.session = Status.Troublemaker;
            manager.game.Call(Context, manager.Users);
        }

        private async void Troublemaker(string value1, string value2)
        {
            Game.Manager manager = Game.Manager.Instance();
            string[] value;

            if (value2.Equals(""))
            {
                value = new string[1];
                value[0] = value1;
            }
            else
            {
                value = new string[2];
                value[0] = value1;
                value[1] = value2;
            }
            if (!OneNight.EqualsUserRole(Context.User, OneNight.EnumRole.Troublemaker))
            {
                await Context.User.SendMessageAsync(Context.User.Mention + " 사용할 수 없습니다!");
                return;
            }
            if (value.Length != 2)
            {
                await Context.User.SendMessageAsync(Context.User.Mention + " 제대로 입력해 주세요!");
                return;
            }
            if (value.Length == 2)
            {
                OneNightUser[] users = new OneNightUser[2];

                if (Context.User.Username.Equals(value[0]) || (users[0] = OneNight.getUserName(value[0])) == null ||
                    Context.User.Username.Equals(value[1]) || (users[1] = OneNight.getUserName(value[1])) == null)
                {
                    await Context.User.SendMessageAsync(Context.User.Mention + " 제대로 입력해 주세요!");
                    return;
                }
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.Color = new Color(143, 48, 48);
                Embed.Title = $":recycle: 말썽쟁이";
                Embed.Description = $"**{users[0].UserSocket.Username}와 {users[1].UserSocket.Username}의 카드를 교체합니다!**\n" +
                await Context.User.SendMessageAsync("", false, Embed.Build());

                manager.session = Status.Drunk;
                manager.game.Call(Context, manager.Users);
            }
        }

        private async void Drunk(string input)
        {
            Game.Manager manager = Game.Manager.Instance();
            Game.OneNight.OneNight.Role role;
            int pos;

            if (!OneNight.EqualsUserRole(Context.User, OneNight.EnumRole.Drunk))
            {
                await Context.User.SendMessageAsync(Context.User.Mention + " 사용할 수 없습니다!");
                return;
            }
            if (!(input.Equals("left") || input.Equals("mid") || input.Equals("right")))
            {
                await Context.User.SendMessageAsync(Context.User.Mention + " left, mid, right 중에 선택하여 주세요!");
                return;
            }
            pos = 0;
            if (input.Equals("left"))
                pos = 0;
            else if (input.Equals("mid"))
                pos = 1;
            else if (input.Equals("right"))
                pos = 2;
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.Color = new Color(143, 48, 48);
            Embed.Title = $":beers: {( new string[] { "왼쪽", "가운데", "오른쪽"} )[pos]} 카드";
            Embed.Description = $"**카드를 바꾸었습니다**";
            await Context.User.SendMessageAsync("", false, Embed.Build());

            role = OneNight.getUserName(Context.User.Username).role;
            OneNight.getUserName(Context.User.Username).role = ((OneNight)manager.game).tableCard[pos];
            ((OneNight)manager.game).tableCard[pos] = role;

            manager.session = Status.Insomniac;
            manager.game.Call(Context, manager.Users);
        }

        [Command("card")]
        public async Task open_tablecard(string value1, [Remainder]string value2 = "")
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Game.Manager manager = Game.Manager.Instance();

            if (manager.session != Status.Werewolves &&
                manager.session != Status.Seer &&
                manager.session != Status.Robber &&
                manager.session != Status.Troublemaker &&
                manager.session != Status.Drunk)
            {
                Embed.Color = new Color(143, 48, 48);
                Embed.Title = ":x: ";
                Embed.Description = "" +
                    "**현재 사용할 수 없습니다!**\n\n" +
                    "```" +
                    "새로운 세션이 필요합니다" +
                    "```" +
                    "";
                await Context.User.SendMessageAsync("", false, Embed.Build());
                return;
            }

            if (manager.session == Status.Werewolves)
                Werewolves(value1);
            else if (manager.session == Status.Seer)
                Seer(value1, value2);
            else if (manager.session == Status.Robber)
                Robber(value1);
            else if (manager.session == Status.Troublemaker)
                Troublemaker(value1, value2);
            else if (manager.session == Status.Drunk)
                Drunk(value1);
        }

        [Command("vote")]
        public async Task vote_user(SocketUser target)
        {
            Game.Manager manager = Game.Manager.Instance();

            if (manager.session != Status.Ingame)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.Color = new Color(143, 48, 48);
                Embed.Title = ":x: ";
                Embed.Description = "" +
                    "**현재 사용할 수 없습니다!**\n\n" +
                    "```" +
                    "새로운 세션이 필요합니다" +
                    "```" +
                    "";
                await Context.User.SendMessageAsync("", false, Embed.Build());
                return;
            }
            if (OneNight.getUserName(Context.User.Username).vote)
            {
                await Context.Channel.SendMessageAsync($"{Context.User.Mention} 투표는 한번만 가능합니다!");
                return;
            }
            if (OneNight.getUserName(target.Username) == null)
            {
                await Context.Channel.SendMessageAsync($"{Context.User.Mention} 게임에 참여하고 있는 대상만 투표해주세요!");
                return;
            }

            for (int i = 0; i < manager.Users.Count; i++)
            {
                if (manager.Users[i].UserSocket == target)
                {
                    ((OneNight)manager.game).vote[i]++;
                    break;
                }
            }
            OneNight.getUserName(Context.User.Username).vote = true;
            

            foreach (OneNightUser u in manager.Users)
            {
                if (!u.vote)
                    return;
            }

            await Context.Channel.SendMessageAsync($"{Context.User.Mention} {target.Username}에게 투표했습니다!");
            manager.game.Call(Context, manager.Users);
        }

        [Command("hunter")]
        public async Task hunter_kill(SocketUser target)
        {
            Game.Manager manager = Game.Manager.Instance();

            if (!OneNight.EqualsUserRole(Context.User, OneNight.EnumRole.Hunter))
            {
                await Context.User.SendMessageAsync(Context.User.Mention + " 사용할 수 없습니다!");
                return;
            }
            if (manager.session != Status.Hunter)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.Color = new Color(143, 48, 48);
                Embed.Title = ":x: ";
                Embed.Description = "" +
                    "**현재 사용할 수 없습니다!**\n\n" +
                    "```" +
                    "새로운 세션이 필요합니다" +
                    "```" +
                    "";
                await Context.User.SendMessageAsync("", false, Embed.Build());
                return;
            }
            if (OneNight.getUserName(target.Username) == null)
            {
                await Context.Channel.SendMessageAsync($"{Context.User.Mention} 게임에 참여하고 있는 대상만 입력해주세요!");
                return;
            }
            
            await Context.Channel.SendMessageAsync($"{target.Mention} 사냥꾼이 당신을 지목했습니다!");
            if (!((OneNight)manager.game).target.Contains(OneNight.getUserName(target.Username)))
                ((OneNight)manager.game).target.Add(OneNight.getUserName(target.Username));

            manager.game.Call(Context, manager.Users);
        }

    }
}
