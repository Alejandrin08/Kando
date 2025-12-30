using kando_desktop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public ObservableCollection<Team> Teams { get; set; } = new ();
        public HomeViewModel() 
        { 
            LoadMockData();
        }

        private void LoadMockData()
        {
            Teams.Clear();

            List<Member> CreateMembers(int count, Color teamColor)
            {
                var list = new List<Member>();
                for (int i = 0; i < count; i++)
                {
                    list.Add(new Member { Initials = $"U{i}", BaseColor = teamColor });
                }
                return list;
            }

            var purpleColor = Color.FromArgb("#913ded");
            Teams.Add(new Team
            {
                Name = "Ingeniería Kando",
                MemberCount = 18,
                NumberBoards = 12,
                Icon = "group.png",
                TeamColor = purpleColor,
                Members = CreateMembers(3, purpleColor) 
            });

            var pinkColor = Color.FromArgb("#EC4899");
            Teams.Add(new Team
            {
                Name = "Marketing",
                MemberCount = 6,
                NumberBoards = 4,
                Icon = "group.png",
                TeamColor = pinkColor,
                Members = CreateMembers(2, pinkColor)
            });

            Teams.Add(new Team
            {
                Name = "Marketing",
                MemberCount = 6,
                NumberBoards = 4,
                Icon = "group.png",
                TeamColor = pinkColor,
                Members = CreateMembers(2, pinkColor)
            });

            Teams.Add(new Team
            {
                Name = "Marketing",
                MemberCount = 6,
                NumberBoards = 4,
                Icon = "group.png",
                TeamColor = pinkColor,
                Members = CreateMembers(2, pinkColor)
            });

            Teams.Add(new Team
            {
                Name = "Marketing",
                MemberCount = 6,
                NumberBoards = 4,
                Icon = "group.png",
                TeamColor = pinkColor,
                Members = CreateMembers(4, pinkColor)
            });

            Teams.Add(new Team
            {
                Name = "Marketing",
                MemberCount = 6,
                NumberBoards = 4,
                Icon = "group.png",
                TeamColor = pinkColor,
                Members = CreateMembers(5, pinkColor)
            });

            Teams.Add(new Team
            {
                Name = "Marketing",
                MemberCount = 6,
                NumberBoards = 4,
                Icon = "group.png",
                TeamColor = pinkColor,
                Members = CreateMembers(6, pinkColor)
            });
        }
    }
}
