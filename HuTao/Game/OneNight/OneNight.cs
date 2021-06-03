using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading;

/*
 *  Werewolves, // * 2
    Minion, // 2
    Mason,
    Seer, // * 1
    Robber, // * 1
    Troublemaker, // * 1
    Drunk, // * 1
    Insomniac,
    Villager,
    Tanner, // * 1
    Hunter
 */

namespace HuTao.Game.OneNight
{
    public class OneNight : IGame
    {
        public Role[] tableCard;
        public int[] vote;
        public List<User> origin;
        public List<User> target;

        public enum EnumRole
        {
            Werewolves, // * 2
            Seer, // * 1
            Robber, // * 1
            Troublemaker, // * 1
            Drunk, // * 1
            Tanner, // * 1
            Mason, // 2
            Minion,
            Insomniac,
            Villager,
            Hunter
        }

        public string[] nameRole = {
            "늑대인간", "예언자", "강도", "말썽쟁이", "주정뱅이", "무두장이", "프리메이슨", "하수인", "불면증환자", "마을시민", "사냥꾼"
        };

        public string[] profileRole = {
            "https://i.imgur.com/oEZMM9V.jpg",
            "https://i.imgur.com/xp6Lusb.png",
            "https://i.imgur.com/GgVHevP.png",
            "https://i.imgur.com/3fIB9v2.png",
            "https://i.imgur.com/iWmfJzb.png",
            "https://i.imgur.com/jsIyLY0.png",
            "https://i.imgur.com/67UicnS.png",
            "https://i.imgur.com/se2Ip2n.jpg",
            "https://i.imgur.com/kZeg7js.png",
            "https://i.imgur.com/5bDvyMU.png",
            "https://i.imgur.com/idh0AJv.png"
        };

        public class Role
        {
            public EnumRole enumRole;
            public string name;
            public string profile;
            public string description;
        }

        async void SendMessageAsync(SocketCommandContext _context, EmbedBuilder Embed, SocketUser socketUser = null)
        {
            if (socketUser != null)
                await socketUser.SendMessageAsync("", false, Embed.Build());
            else
                await _context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        public static bool EqualsUserRole(SocketUser socketUser, EnumRole enumRole)
        {
            Game.Manager manager = Game.Manager.Instance();

            foreach (OneNightUser u in manager.Users)
            {
                if (u.UserSocket.Id == socketUser.Id && u.role.enumRole == enumRole)
                    return (true);
            }
            return (false);
        }

        public static OneNightUser getUserName(string name)
        {
            Game.Manager manager = Game.Manager.Instance();

            foreach (OneNightUser u in manager.Users)
            {
                if (u.UserSocket.Username.Equals(name))
                    return (u);
            }
            return (null);
        }

        public static void changeUserRole(SocketUser src, SocketUser dst)
        {
            Game.Manager manager = Game.Manager.Instance();
            OneNightUser[] users = new OneNightUser[2];
            Role role;

            foreach (OneNightUser u in manager.Users)
            {
                if (u.UserSocket.Id == src.Id)
                    users[0] = u;
                if (u.UserSocket.Id == dst.Id)
                    users[1] = u;
            }
            role = users[0].role;
            users[0].role = users[1].role;
            users[1].role = role;
        }

        private void Werewolves(SocketCommandContext _context, List<User> users)
        {
            Game.Manager manager = Game.Manager.Instance();
            EmbedBuilder Embed = new EmbedBuilder();
            OneNightUser[] user;
            int count;

            count = 0;
            user = new OneNightUser[2];
            foreach (OneNightUser u in users)
            {
                if (u.role.enumRole == EnumRole.Werewolves)
                {
                    user[count] = u;
                    count++;
                }
            }
            if (count == 0)
            {
                manager.session = Status.Minion;
                Call(_context, users);
            }
            else if (count == 1)
            {
                Embed.Color = new Color(143, 48, 48);
                Embed.Title = $":wolf: 늑대인간";
                Embed.Description = "**당신은 혼자입니다!**\n\n중앙에 놓인 3장의 카드 중 하나를 확인할 수 있습니다.\n\n```!hutao card [위치]```\nleft mid right 중 하나를 선택하여 입력해주세요";
                Embed.WithImageUrl("https://i.imgur.com/YEFzSdh.png");
                
                SendMessageAsync(_context, Embed, user[0].UserSocket);
            }
            else if (count == 2)
            {
                Embed.Color = new Color(143, 48, 48);
                Embed.Title = $":wolf: 늑대인간";
                Embed.Description = $"**당신은 늑대인간입니다!**\n아래는 당신의 동료입니다\n\n```{user[0].UserSocket.Username} {user[1].UserSocket.Username}```";
                SendMessageAsync(_context, Embed, user[0].UserSocket);
                SendMessageAsync(_context, Embed, user[1].UserSocket);

                manager.session = Status.Minion;
                Call(_context, users);
            }
        }

        private void Minion(SocketCommandContext _context, List<User> users)
        {
            Game.Manager manager = Game.Manager.Instance();
            EmbedBuilder Embed = new EmbedBuilder();
            User minion = new User();
            User[] werewolves = new User[2];
            int count;
            int count_w;

            count = 0;
            foreach (OneNightUser u in users)
            {
                if (u.role.enumRole == EnumRole.Minion)
                {
                    minion = u;
                    count++;
                }
            }

            count_w = 0;
            foreach (OneNightUser u in users)
                if (u.role.enumRole == EnumRole.Werewolves)
                    werewolves[count_w++] = u;
            if (count == 0)
            {
                manager.session = Status.Mason;
                Call(_context, users);
            }
            else if (count == 1)
            {
                Embed.Color = new Color(143, 48, 48);
                Embed.Title = $":second_place: 하수인";
                if (count_w == 0)
                    Embed.Description = $"**늑대인간이 없습니다!**";
                else if (count_w == 1)
                    Embed.Description = $"**아래는 늑대인간입니다!**\n\n```{werewolves[0].UserSocket.Username}```";
                else if (count_w == 2)
                    Embed.Description = $"**아래는 늑대인간입니다!**\n\n```{werewolves[0].UserSocket.Username} {werewolves[1].UserSocket.Username}```";
            }

            manager.session = Status.Mason;
            Call(_context, users);
        }

        private void Mason(SocketCommandContext _context, List<User> users)
        {
            Game.Manager manager = Game.Manager.Instance();
            EmbedBuilder Embed = new EmbedBuilder();
            OneNightUser[] user;
            int count;

            count = 0;
            user = new OneNightUser[2];
            foreach (OneNightUser u in users)
            {
                if (u.role.enumRole == EnumRole.Mason)
                {
                    user[count] = u;
                    count++;
                }
            }
            if (count == 0)
            {
                manager.session = Status.Seer;
                Call(_context, users);
            }
            else if (count == 2)
            {
                Embed.Color = new Color(143, 48, 48);
                Embed.Title = $":cyclone: 프리메이슨";
                Embed.Description = $"**위대한 프리메이슨은 언제나 함께하는 이가 있습니다!**\n아래는 당신의 동료입니다**\n\n```{user[0].UserSocket.Username} {user[1].UserSocket.Username}```";
                SendMessageAsync(_context, Embed, user[0].UserSocket);
                SendMessageAsync(_context, Embed, user[1].UserSocket);

                manager.session = Status.Seer;
                Call(_context, users);
            }
        }

        private void Seer(SocketCommandContext _context, List<User> users)
        {
            Game.Manager manager = Game.Manager.Instance();
            EmbedBuilder Embed = new EmbedBuilder();
            User seer = new User();
            int count;

            count = 0;
            foreach (OneNightUser u in users)
            {
                if (u.role.enumRole == EnumRole.Seer)
                {
                    seer = u;
                    count++;
                    break;
                }
            }
            if (count == 0)
            {
                manager.session = Status.Robber;
                Call(_context, users);
            }
            else if (count == 1)
            {
                string userList = "\n";
                foreach (User user in users)
                    userList += user.UserSocket.Username + "\n";
                Embed.Color = new Color(143, 48, 48);
                Embed.Title = $":seedling: 예언자";
                Embed.Description = $"**예언의 시간입니다!**\n\n" +
                    "카드더미 3장 중 2장을 확인하거나, 남의 카드 한장을 확인할 수 있습니다\n\n" +
                    "테이블의 있는 카드 두장을 확인하려면\n" +
                    "```!hutao card [위치] [위치]```\n" +
                    "left, mid, right 중에 선택하여 주세요\n\n" +
                    "다른 사람의 카드를 보려면\n" +
                    $"**목록**```{userList}```\n" +
                    "```!hutao card [이름]```\n" +
                    "를 입력해주세요";
                Embed.WithImageUrl("https://i.imgur.com/YEFzSdh.png");
                
                SendMessageAsync(_context, Embed, seer.UserSocket);
            }
        }

        private void Robber(SocketCommandContext _context, List<User> users)
        {
            Game.Manager manager = Game.Manager.Instance();
            EmbedBuilder Embed = new EmbedBuilder();
            User robber = new User();
            int count;

            count = 0;
            foreach (OneNightUser u in origin)
            {
                if (u.role.enumRole == EnumRole.Robber)
                {
                    robber = u;
                    count++;
                    break;
                }
            }
            if (count == 0)
            {
                manager.session = Status.Troublemaker;
                Call(_context, users);
            }
            else if (count == 1)
            {
                string userList = "\n";
                foreach (User user in users)
                    userList += user.UserSocket.Username + "\n";
                Embed.Color = new Color(143, 48, 48);
                Embed.Title = $":moneybag: 강도";
                Embed.Description = $"**바꾸기**\n\n" +
                    "내 카드를 남의 카드와 바꿉니다!\n\n" +
                    $"**목록**\n```{userList}```\n" +
                    "```!hutao card [이름]```\n\n" +
                    "을 입력해주세요";
                Embed.WithImageUrl("https://i.imgur.com/GgVHevP.png");
                
                SendMessageAsync(_context, Embed, robber.UserSocket);
            }
        }

        private void Troublemaker(SocketCommandContext _context, List<User> users)
        {
            Game.Manager manager = Game.Manager.Instance();
            EmbedBuilder Embed = new EmbedBuilder();
            User trouble = new User();
            int count;

            count = 0;
            foreach (OneNightUser u in origin)
            {
                if (u.role.enumRole == EnumRole.Troublemaker)
                {
                    trouble = u;
                    count++;
                    break;
                }
            }
            if (count == 0)
            {
                manager.session = Status.Drunk;
                Call(_context, users);
            }
            else if (count == 1)
            {
                string userList = "\n";
                foreach (User user in users)
                    userList += user.UserSocket.Username + "\n";
                Embed.Color = new Color(143, 48, 48);
                Embed.Title = $":recycle: 말썽쟁이";
                Embed.Description = $"**바꾸기**\n\n" +
                    "내 카드를 남의 카드와 바꿉니다!\n\n" +
                    $"**목록**\n```{userList}```\n" +
                    "```!hutao card [이름] [이름]```\n\n" +
                    "을 입력해주세요";
                
                SendMessageAsync(_context, Embed, trouble.UserSocket);
            }
        }

        private void Drunk(SocketCommandContext _context, List<User> users)
        {
            Game.Manager manager = Game.Manager.Instance();
            EmbedBuilder Embed = new EmbedBuilder();
            User drunk = new User();
            int count;

            count = 0;
            foreach (OneNightUser u in origin)
            {
                if (u.role.enumRole == EnumRole.Drunk)
                {
                    drunk = u;
                    count++;
                }
            }
            if (count == 0)
            {
                manager.session = Status.Insomniac;
                Call(_context, users);
            }
            else if (count == 1)
            {
                Embed.Color = new Color(143, 48, 48);
                Embed.Title = $":beers: 주정뱅이";
                Embed.Description = "**실수를 저지를 시간입니다!**\n\n" +
                    "중앙에 있는 카드 중 하나와 자신의 카드를 바꿉니다\n\n" +
                    "```!hutao card [위치]```\n" +
                    "left mid right 중 하나를 선택하여 입력해주세요";
                Embed.WithImageUrl("https://i.imgur.com/YEFzSdh.png");

                SendMessageAsync(_context, Embed, drunk.UserSocket);
            }
        }

         private void Insomniac(SocketCommandContext _context, List<User> users)
         {
            Game.Manager manager = Game.Manager.Instance();
            EmbedBuilder Embed = new EmbedBuilder();
            User insomn = new User();
            int count;

            count = 0;
            foreach (OneNightUser u in origin)
            {
                if (u.role.enumRole == EnumRole.Insomniac)
                {
                    insomn = u;
                    count++;
                }
            }
            if (count == 0)
            {
                manager.session = Status.Ingame;

                vote = new int[users.Count];
                for (int i = 0; i < users.Count; i++)
                    vote[i] = 0;

                Embed.Title = $":ballot_box_with_check: 결정";
                Embed.Description = "**우리는 이 안에 섞여있는 늑대인간을 잡아내야 합니다!**\n\n" +
                    "결정했다면 아래 명령어를 입력해주세요\n" +
                    "_한번 한 투표는 되돌릴 수 없습니다! 명심하세요!_\n" +
                    "```!hutao vote [@멘션]```";
                SendMessageAsync(manager.channel, Embed);

                Embed.Color = new Color(143, 48, 48);
                Embed.Title = $":game_die: 게임 시작";
                Embed.Description = "**각자 승리하세요!**\n";
                SendMessageAsync(manager.channel, Embed);
            }
            else if (count == 1)
            {
                Embed.Color = new Color(143, 48, 48);
                Embed.Title = $":sleeping_accommodation: 불면증";
                Embed.Description = "**잠에 들지 못했습니다..**\n\n" +
                    $"지금 당신의 카드는 {OneNight.getUserName(_context.User.Username).role.name}입니다\n";
                Embed.WithImageUrl(OneNight.getUserName(_context.User.Username).role.profile);
                SendMessageAsync(_context, Embed, insomn.UserSocket);

                manager.session = Status.Ingame;

                vote = new int[users.Count];
                for (int i = 0; i < users.Count; i++)
                    vote[i] = 0;

                Embed.Title = $":ballot_box_with_check: 결정";
                Embed.Description = "**우리는 이 안에 섞여있는 늑대인간을 잡아내야 합니다!**\n\n" +
                    "결정했다면 아래 명령어를 입력해주세요\n" +
                    "_한번 한 투표는 되돌릴 수 없습니다! 명심하세요!_\n" +
                    "```!hutao vote [@멘션]```";
                SendMessageAsync(manager.channel, Embed);

                Embed.Color = new Color(143, 48, 48);
                Embed.Title = $":game_die: 게임 시작";
                Embed.Description = "**각자 승리하세요!**\n";
                SendMessageAsync(manager.channel, Embed);
            }
        }

        private void Finish(SocketCommandContext _context, List<User> users)
        {
            Game.Manager manager = Game.Manager.Instance();
            EmbedBuilder Embed = new EmbedBuilder();
            target = new List<User>();
            int top;

            top = -1;
            for (int i = 0; i < users.Count; i++)
                if (vote[i] > top)
                    top = vote[i];
            for (int i = 0; i < users.Count; i++)
                if (vote[i] == top && vote[i] >= 2)
                    target.Add(users[i]);

            string userList = "\n";
            foreach (OneNightUser u in users)
                userList += u.UserSocket.Mention + "\n";
                 
            manager.session = Status.Hunter;
            foreach (OneNightUser u in target)
            {
                if (u.role.enumRole == EnumRole.Hunter)
                {
                    Embed.Title = $":middle_finger: HUNTER!";
                    Embed.Description = "**사냥꾼은 결코 혼자 죽지 않습니다!!**\n\n" +
                        $"사냥꾼이 한명을 데려갑니다. 사냥꾼: {u.UserSocket.Username}\n" +
                        "_사냥꾼은 아래 명령어를 입력해주세요!_\n\n" +
                        $"```{userList}```\n" +
                        "```!hutao hunter [@멘션]```";
                    Embed.WithImageUrl(u.role.profile);
                    SendMessageAsync(_context, Embed);

                    return;
                }
            }
            Result(_context, users);
        }

        private void Result(SocketCommandContext _context, List<User> users)
        {
            EmbedBuilder Embed = new EmbedBuilder();

            Embed.Title = $":wine_glass: 승리";
            // Tanner Win
            if (ContainsRoleList(target, EnumRole.Tanner))
            {
                User tanner = new User();
                foreach (OneNightUser u in users)
                    if (u.role.enumRole == EnumRole.Tanner)
                    {
                        tanner = u;
                        break;
                    }
                Embed.Description = $"**주정뱅이가 승리 했습니다!**\n\n" +
                    "_최종 승리_\n" +
                    $"```{tanner.UserSocket.Username}```\n";
                Embed.WithImageUrl(((OneNightUser)tanner).role.profile);
            }
            // Villager Win
            else if ((!ContainsRoleList(users, EnumRole.Werewolves) && target.Count == 0) ||
                (ContainsRoleList(users, EnumRole.Werewolves) && ContainsRoleList(target, EnumRole.Werewolves)))
            {
                string winList = "\n";
                foreach (OneNightUser u in users)
                    if (u.role.enumRole != EnumRole.Tanner && u.role.enumRole != EnumRole.Minion && u.role.enumRole != EnumRole.Werewolves)
                        winList += u.UserSocket.Username + "\n";
                Embed.Description = $"**시민이 승리 했습니다!**\n\n" +
                    "_최종 승리_\n" +
                    $"```{winList}```\n";
                Embed.WithImageUrl("https://i.imgur.com/5bDvyMU.png");
            }
            // Werewolves & Minion Win
            else if (ContainsRoleList(users, EnumRole.Werewolves) && !ContainsRoleList(target, EnumRole.Werewolves))
            {
                string winList = "\n";
                foreach (OneNightUser u in users)
                    if (u.role.enumRole == EnumRole.Werewolves && u.role.enumRole == EnumRole.Minion)
                        winList += u.UserSocket.Username + "\n";
                Embed.Description = $"**늑대인간 & 하수인이 승리 했습니다!**\n\n" +
                    "_최종 승리_\n" +
                    $"```{winList}```\n";
                Embed.WithImageUrl("https://i.imgur.com/oEZMM9V.jpg");
            }
            SendMessageAsync(_context, Embed);

            EmbedBuilder result = new EmbedBuilder();
            result.Color = new Color(143, 48, 48);
            string userList = "\n";
            for (int i = 0; i < users.Count; i++)
                userList += users[i].UserSocket.Username + ": " + ((OneNightUser)origin[i]).role.name + " → " + ((OneNightUser)users[i]).role.name + "\n";
            result.Title = $":dividers: Result";
            result.Description = "**직업**\n" +
                $"```{userList}```\n";
            SendMessageAsync(_context, Embed);

            Game.Manager manager = Game.Manager.Instance();
            manager.clear();
        }

        private bool ContainsRoleList(List<User> users, EnumRole role)
        {
            foreach (OneNightUser u in users)
            {
                if (u.role.enumRole == role)
                    return (true);
            }
            return (false);
        }

        public void Call(SocketCommandContext _context, List<User> users)
        {            
            Game.Manager manager = Game.Manager.Instance();

            if (manager.session == Status.Minion)
                Minion(_context, users);
            else if (manager.session == Status.Mason)
                Mason(_context, users);
            else if (manager.session == Status.Seer)
                Seer(_context, users);
            else if (manager.session == Status.Robber)
                Robber(_context, users);
            else if (manager.session == Status.Troublemaker)
                Troublemaker(_context, users);
            else if (manager.session == Status.Drunk)
                Drunk(_context, users);
            else if (manager.session == Status.Insomniac)
                Insomniac(_context, users);
            else if (manager.session == Status.Ingame)
                Finish(_context, users);
            else if (manager.session == Status.Hunter)
                Result(_context, users);
        }

        private EnumRole[] getRoles(int count)
        {
            EnumRole[] roles = new EnumRole[count];
            List<EnumRole> extraRole = new List<EnumRole>();
            Random random = new Random();
            EnumRole role;
            int i;

            extraRole.Add(EnumRole.Mason);
            extraRole.Add(EnumRole.Minion);
            extraRole.Add(EnumRole.Insomniac);
            extraRole.Add(EnumRole.Villager);
            extraRole.Add(EnumRole.Hunter);

            roles[0] = (EnumRole)0;
            for (i = 1; i < 7; i++)
                roles[i] = (EnumRole)(i - 1);
            while (i < count)
            {
                switch ((role = extraRole[random.Next(0, extraRole.Count)]))
                {
                    case EnumRole.Mason:
                        if (i + 1 < count)
                        {
                            roles[i++] = (EnumRole)role;
                            roles[i++] = (EnumRole)role;
                            extraRole.Remove((EnumRole)role);
                        }    
                        break ;

                    default:
                        roles[i++] = (EnumRole)role;
                        extraRole.Remove((EnumRole)role);
                        break ;
                }
            }
            return (roles);
        }

        private void SuffleRoles(EnumRole[] roles, int count)
        {
            Random random = new Random();
            EnumRole temp;
            int rand;

            for (int i = 0; i < count; i++)
            {
                rand = random.Next(0, count);
                temp = roles[rand];
                roles[rand] = roles[i];
                roles[i] = temp;
            }
        }

        public void New(SocketCommandContext _context, List<User> users)
        {
            Game.Manager manager = Game.Manager.Instance();

            EmbedBuilder Embed = new EmbedBuilder();
            Embed.Color = new Color(143, 48, 48);
            Embed.Title = $":dividers: 셔플";
            Embed.Description = "**카드를 섞고 있습니다!**";
            SendMessageAsync(_context, Embed);
 
            EnumRole[] roles = getRoles(users.Count + 3);
            SuffleRoles(roles, users.Count + 3);
            for (int i = 0; i < users.Count; i++)
            {
                ((OneNightUser)users[i]).role = new Role() {
                    enumRole = roles[i],
                    name = nameRole[(int)roles[i]],
                    profile = profileRole[(int)roles[i]]
                };
            }

            tableCard = new Role[3];
            for (int i = 0; i < 3; i++)
                 tableCard[i] = new Role {
                        enumRole = roles[users.Count + i],
                        name = nameRole[(int)roles[users.Count + i]],
                        profile = profileRole[(int)roles[users.Count + i]]
                    };

            origin = new List<User>();
            foreach (OneNightUser u in users)
            {
                origin.Add(u);

                Embed.Color = new Color(143, 48, 48);
                Embed.Title = u.role.name;
                Embed.Description = "";
                Embed.WithImageUrl(u.role.profile);
                
                u.UserSocket.SendMessageAsync("", false, Embed.Build());
            }

            manager.session = Status.Werewolves;
            Werewolves(_context, users);
        }
    }
}
