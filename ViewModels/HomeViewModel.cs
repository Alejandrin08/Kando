using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kando_desktop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace kando_desktop.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        public ObservableCollection<Team> Teams { get; set; } = new();
        public ObservableCollection<Board> Boards { get; set; } = new();
        public Action RequestShowCreateTeam;
        public Action RequestShowCreateBoard;

        [ObservableProperty]
        private bool isTeamDropdownOpen;

        [ObservableProperty]
        private Team selectedTeam;

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

            var blueColor = Color.FromArgb("#3B82F6");
            Teams.Add(new Team
            {
                Name = "Diseño UX/UI",
                MemberCount = 8,
                NumberBoards = 7,
                Icon = "group.png",
                TeamColor = blueColor,
                Members = CreateMembers(4, blueColor)
            });

            var greenColor = Color.FromArgb("#10B981");
            Teams.Add(new Team
            {
                Name = "Ventas",
                MemberCount = 12,
                NumberBoards = 9,
                Icon = "group.png",
                TeamColor = greenColor,
                Members = CreateMembers(3, greenColor)
            });

            var orangeColor = Color.FromArgb("#F59E0B");
            Teams.Add(new Team
            {
                Name = "Recursos Humanos",
                MemberCount = 5,
                NumberBoards = 3,
                Icon = "group.png",
                TeamColor = orangeColor,
                Members = CreateMembers(2, orangeColor)
            });

            var tealColor = Color.FromArgb("#14B8A6");
            Teams.Add(new Team
            {
                Name = "Operaciones",
                MemberCount = 15,
                NumberBoards = 11,
                Icon = "group.png",
                TeamColor = tealColor,
                Members = CreateMembers(5, tealColor)
            });

            var indigoColor = Color.FromArgb("#6366F1");
            Teams.Add(new Team
            {
                Name = "Soporte al Cliente",
                MemberCount = 10,
                NumberBoards = 6,
                Icon = "group.png",
                TeamColor = indigoColor,
                Members = CreateMembers(3, indigoColor)
            });

            var redColor = Color.FromArgb("#EF4444");
            Teams.Add(new Team
            {
                Name = "Finanzas",
                MemberCount = 7,
                NumberBoards = 5,
                Icon = "group.png",
                TeamColor = redColor,
                Members = CreateMembers(2, redColor)
            });

            if (Teams.Count > 0)
            {
                SelectedTeam = Teams[0];
            }
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
                Name = "Rediseño App Mobile",
                Icon = "puzzle.png",
                TeamName = Teams[2],
                TeamColor = Teams[2].TeamColor,
                TaskCount = 18,
                TotalTasks = 30,
                TotalTaskPorcentage = 60
            });

            Boards.Add(new Board
            {
                Name = "Pipeline Q4 2024",
                Icon = "startup.png",
                TeamName = Teams[3],
                TeamColor = Teams[3].TeamColor,
                TaskCount = 45,
                TotalTasks = 60,
                TotalTaskPorcentage = 75
            });

            Boards.Add(new Board
            {
                Name = "Proceso de Onboarding",
                Icon = "cat.png",
                TeamName = Teams[4],
                TeamColor = Teams[4].TeamColor,
                TaskCount = 8,
                TotalTasks = 15,
                TotalTaskPorcentage = 53
            });
        }

        [RelayCommand]
        private void CreateTeam()
        {
            RequestShowCreateTeam?.Invoke();
        }

        [RelayCommand]
        private void CreateBoard()
        {
            if (Teams.Count > 0)
            {
                SelectedTeam = Teams[0];
            }
            IsTeamDropdownOpen = false; 
            RequestShowCreateBoard?.Invoke();
        }

        [RelayCommand]
        private void ToggleTeamDropdown()
        {
            IsTeamDropdownOpen = !IsTeamDropdownOpen;
        }

        [RelayCommand]
        private void SelectTeam(Team team)
        {
            SelectedTeam = team;
            IsTeamDropdownOpen = false;
        }
    }
}