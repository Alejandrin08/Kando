using System.Collections.ObjectModel;
using System.Net.Http.Json;
using kando_desktop.DTOs.Responses;
using kando_desktop.Helpers;
using kando_desktop.Models;
using kando_desktop.Resources.Strings;
using kando_desktop.Services.Contracts;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace kando_desktop.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private HubConnection _hubConnection;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public event Action<string, bool> OnShowNotification;
        public event Action<int> OnTeamUpdated;

        public ObservableCollection<NotificationItem> RealTimeNotifications { get; } = new();
        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public NotificationService(IConfiguration configuration, HttpClient httpClient)
        {
            _baseUrl = configuration["ApiSettings:BaseUrl"];
            _httpClient = httpClient;
        }

        public void Show(string message, bool isError = false)
        {
            OnShowNotification?.Invoke(message, isError);
        }

        public async Task InitializeSignalRAsync(string token, int userId)
        {
            if (_hubConnection != null)
            {
                await DisconnectSignalRAsync();
            }

            _hubConnection = BuildHubConnection(token);
            RegisterEventHandlers();

            await StartConnectionAsync(userId);
        }

        public async Task UnsubscribeFromTeamAsync(int teamId)
        {
            if (IsConnected)
            {
                try
                {
                    await _hubConnection.InvokeAsync("LeaveTeamGroup", teamId);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error leaving group SignalR: {ex.Message}");
                }
            }
        }

        private HubConnection BuildHubConnection(string token)
        {
            string hubUrl = _baseUrl.EndsWith("api/")
                ? _baseUrl.Replace("api/", "notificationHub")
                : $"{_baseUrl}notificationHub";

            return new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                })
                .WithAutomaticReconnect()
                .Build();
        }

        private void RegisterEventHandlers()
        {
            _hubConnection.Reconnected += OnReconnectedAsync;

            _hubConnection.On<NotificationResponseDto>("ReceiveNotification", HandleReceiveNotification);
            _hubConnection.On<int>("TeamMembersChanged", HandleTeamMembersChanged);
            _hubConnection.On<int, string, string, int>("TeamUpdated", HandleTeamUpdated);
            _hubConnection.On<int, string, string, int>("TeamDeleted", HandleTeamDeleted);
            _hubConnection.On<int, string, string>("RemovedFromTeam", HandleRemovedFromTeam);
            _hubConnection.On<int>("InvitationRevoked", HandleInvitationRevoked);
            _hubConnection.On("BoardsChanged", HandleBoardsChanged);
        }

        private async Task OnReconnectedAsync(string connectionId)
        {
            var workspaceService = ServiceHelper.GetService<IWorkspaceService>();
            var teamIds = workspaceService?.Teams.Select(t => t.Id).ToList();

            if (teamIds != null && teamIds.Any())
            {
                await SubscribeToTeamsAsync(teamIds);
            }
        }

        private void HandleReceiveNotification(NotificationResponseDto notification)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var item = MapNotificationDto(notification);
                RealTimeNotifications.Insert(0, item);

                string toastMsg = notification.NotificationType == "InviteTeam"
                    ? AppResources.NewTeamInvitation
                    : AppResources.YouHaveNewInvitation;

                Show(toastMsg);
            });
        }

        private void HandleBoardsChanged()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await RefreshWorkspaceAsync();
            });
        }

        private void HandleTeamMembersChanged(int teamId)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                OnTeamUpdated?.Invoke(teamId);
                await RefreshWorkspaceAsync();
            });
        }

        private void HandleTeamUpdated(int teamId, string teamName, string ownerName, int actionOwnerId)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await RefreshWorkspaceAsync();

                if (!IsCurrentUser(actionOwnerId))
                {
                    string message = $"{ownerName} {AppResources.TeamUpdated} {teamName}.";
                    Show(message);
                }
            });
        }

        private void HandleTeamDeleted(int teamId, string teamName, string ownerName, int actionOwnerId)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                RemoveTeamFromWorkspace(teamId);
                await UnsubscribeFromTeamAsync(teamId);

                if (!IsCurrentUser(actionOwnerId))
                {
                    string message = $"{ownerName} {AppResources.TeamDeleted} {teamName}.";
                    Show(message);
                }
            });
        }

        private void HandleRemovedFromTeam(int teamId, string teamName, string ownerName)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                RemoveTeamFromWorkspace(teamId);
                await UnsubscribeFromTeamAsync(teamId);

                string message = $"{ownerName} {AppResources.RemovedFromTeam} {teamName}.";
                Show(message);
            });
        }

        private void HandleInvitationRevoked(int teamId)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var notifToRemove = RealTimeNotifications
                    .FirstOrDefault(n => n.TeamId == teamId && n.NotificationType == "InviteTeam");

                if (notifToRemove != null)
                {
                    RealTimeNotifications.Remove(notifToRemove);
                }
            });
        }

        private async Task StartConnectionAsync(int userId)
        {
            try
            {
                await _hubConnection.StartAsync();
                await _hubConnection.InvokeAsync("JoinUserGroup", userId.ToString());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error starting SignalR: {ex.Message}");
            }
        }

        private NotificationItem MapNotificationDto(NotificationResponseDto dto)
        {
            return new NotificationItem
            {
                Id = dto.Id,
                NotificationType = dto.NotificationType,
                CreatedAt = dto.CreatedAt,
                IsRead = dto.IsRead,
                TeamId = dto.TeamId,
                TeamName = dto.TeamName,
                TeamIcon = dto.TeamIcon,
                TeamColor = !string.IsNullOrEmpty(dto.TeamColor)
                    ? Color.FromArgb(dto.TeamColor)
                    : Colors.Gray,
                OwnerName = dto.OwnerName,
                TaskId = dto.TaskId,
                TaskName = dto.TaskName,
                BoardName = dto.BoardName,
                Title = dto.Title,
                Message = dto.Message
            };
        }

        private async Task RefreshWorkspaceAsync()
        {
            var workspaceService = ServiceHelper.GetService<IWorkspaceService>();
            if (workspaceService != null)
            {
                await workspaceService.ForceRefreshAsync();
            }
        }

        private void RemoveTeamFromWorkspace(int teamId)
        {
            var workspaceService = ServiceHelper.GetService<IWorkspaceService>();
            var teamToRemove = workspaceService?.Teams.FirstOrDefault(t => t.Id == teamId);

            if (teamToRemove != null)
            {
                workspaceService.DeleteTeam(teamToRemove);
            }
        }

        private bool IsCurrentUser(int actionOwnerId)
        {
            var sessionService = ServiceHelper.GetService<ISessionService>();
            var currentUserId = sessionService?.CurrentUser?.UserId;
            return currentUserId == actionOwnerId;
        }

        public async Task DisconnectSignalRAsync()
        {
            if (_hubConnection != null)
            {
                try
                {
                    await _hubConnection.StopAsync();
                    await _hubConnection.DisposeAsync();
                    _hubConnection = null;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error disconnecting SignalR: {ex.Message}");
                }
            }
        }

        public async Task SubscribeToTeamsAsync(List<int> teamIds)
        {
            if (!teamIds.Any()) return;

            try
            {
                await WaitForConnectionAsync();

                if (_hubConnection.State == HubConnectionState.Connected)
                {
                    await _hubConnection.InvokeAsync("JoinTeamsGroups", teamIds);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error subscribing to teams: {ex.Message}");
            }
        }

        private async Task WaitForConnectionAsync(int maxRetries = 5, int delayMs = 500)
        {
            int retry = 0;
            while (_hubConnection.State != HubConnectionState.Connected && retry < maxRetries)
            {
                await Task.Delay(delayMs);
                retry++;
            }
        }

        public void ClearNotifications()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                RealTimeNotifications.Clear();
            });
        }

        public async Task<List<NotificationResponseDto>> GetMyNotificationsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("notification");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<NotificationResponseDto>>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting notifications: {ex.Message}");
            }
            return new List<NotificationResponseDto>();
        }

        public async Task LoadHistoricalNotificationsAsync()
        {
            try
            {
                var history = await GetMyNotificationsAsync();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    RealTimeNotifications.Clear();

                    if (history != null && history.Any())
                    {
                        foreach (var notif in history)
                        {
                            var item = MapNotificationDto(notif);
                            RealTimeNotifications.Add(item);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading historical notifications: {ex.Message}");
            }
        }
    }
}