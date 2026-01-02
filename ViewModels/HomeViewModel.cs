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
        }

        public void AddNewTeam(string name, string iconSource, Color teamColor)
        {
            if (string.IsNullOrWhiteSpace(name)) return;

            var myself = new Member
            {
                Initials = "YO",
                BaseColor = teamColor
            };

            var newTeam = new Team
            {
                Name = name,
                Icon = iconSource,
                TeamColor = teamColor,
                MemberCount = 1,
                NumberBoards = 0,
                Members = new List<Member> { myself }
            };

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Teams.Add(newTeam);

                OnPropertyChanged(nameof(Teams));
            });
        }

        public void AddNewBoard(string name, string iconSource, Team team)
        {
            if (string.IsNullOrWhiteSpace(name) || team == null) return;

            var newBoard = new Board
            {
                Name = name,
                Icon = iconSource, 
                TeamName = team,   
                TeamColor = team.TeamColor, 
                TaskCount = 0,
                TotalTasks = 0,
                TotalTaskPorcentage = 0
            };

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Boards.Add(newBoard);

                team.NumberBoards++;
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