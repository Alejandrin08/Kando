using CommunityToolkit.Mvvm.Input;
using kando_desktop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        public ObservableCollection<Team> Teams { get; set; } = new();
        public ObservableCollection<Board> Boards { get; set; } = new();
        public Action RequestShowCreateTeam;

        public HomeViewModel()
        {
            LoadMockDataTeams();
            LoadMockDataBoards();
        }

        private void LoadMockDataTeams()
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

        private void LoadMockDataBoards()
        {
            Boards.Clear();

            if (Teams.Count == 0) return;

            var random = new Random();

            Boards.Add(new Board
            {
                Name = "Desarrollo Kando v1.0",
                Icon = "menu.png",
                TeamName = Teams[0],
                TeamColor = Teams[0].TeamColor,
                TaskCount = 32,
                TotalTasks = 55,
                TotalTaskPorcentage = 58
            });

            Boards.Add(new Board
            {
                Name = "Campaña Q3 Redes Sociales",
                Icon = "trend.png",
                TeamName = Teams[1],
                TeamColor = Teams[1].TeamColor,
                TaskCount = 12,
                TotalTasks = 12,
                TotalTaskPorcentage = 100
            });

            Boards.Add(new Board
            {
                Name = "Refactorización Backend",
                Icon = "menu.png",
                TeamName = Teams[0],
                TeamColor = Teams[0].TeamColor,
                TaskCount = 5,
                TotalTasks = 20,
                TotalTaskPorcentage = 25
            });

            Boards.Add(new Board
            {
                Name = "Roadmap 2025",
                Icon = "trend.png",
                TeamName = Teams[2],
                TeamColor = Teams[2].TeamColor,
                TaskCount = 0,
                TotalTasks = 10,
                TotalTaskPorcentage = 0
            });
        }

        [RelayCommand]
        private void CreateTeam()
        {
            RequestShowCreateTeam?.Invoke();
        }
    }
}
