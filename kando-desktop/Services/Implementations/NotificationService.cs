
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using kando_desktop.DTOs.Responses;
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

            string hubUrl = _baseUrl.EndsWith("api/")
                ? _baseUrl.Replace("api/", "notificationHub")
                : $"{_baseUrl}notificationHub";

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                })
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<NotificationResponseDto>("ReceiveNotification", (notification) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var item = new NotificationItem
                    {
                        Id = notification.Id,
                        NotificationType = notification.NotificationType,
                        CreatedAt = notification.CreatedAt,
                        IsRead = notification.IsRead,
                        TeamId = notification.TeamId,
                        TeamName = notification.TeamName,
                        TeamIcon = notification.TeamIcon,
                        TeamColor = !string.IsNullOrEmpty(notification.TeamColor)
                                                ? Color.FromArgb(notification.TeamColor)
                                                : Colors.Gray,
                        OwnerName = notification.OwnerName,
                        TaskId = notification.TaskId,
                        TaskName = notification.TaskName,
                        BoardName = notification.BoardName,
                        Title = notification.Title,
                        Message = notification.Message
                    };

                    RealTimeNotifications.Insert(0, item);

                    string toastMsg = notification.NotificationType == "InviteTeam"
                                      ? AppResources.NewTeamInvitation
                                      : AppResources.YouHaveNewInvitation;

                    Show(toastMsg);
                });
            });

            try
            {
                await _hubConnection.StartAsync();
                await _hubConnection.InvokeAsync("JoinUserGroup", userId.ToString());
                System.Diagnostics.Debug.WriteLine($"✅ SignalR connected for user {userId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SignalR Error: {ex.Message}");
            }
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
                    System.Diagnostics.Debug.WriteLine("✅ SignalR disconnected");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error disconnecting SignalR: {ex.Message}");
                }
            }
        }

        public void ClearNotifications()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                RealTimeNotifications.Clear();
                System.Diagnostics.Debug.WriteLine("✅ Notifications cleared");
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
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
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
                            var item = new NotificationItem
                            {
                                Id = notif.Id,
                                NotificationType = notif.NotificationType,
                                CreatedAt = notif.CreatedAt,
                                IsRead = notif.IsRead,
                                TeamId = notif.TeamId,
                                TeamName = notif.TeamName,
                                TeamIcon = notif.TeamIcon,
                                TeamColor = string.IsNullOrEmpty(notif.TeamColor)
                                                                ? Colors.Gray
                                                                : Color.FromArgb(notif.TeamColor),
                                OwnerName = notif.OwnerName,
                                TaskId = notif.TaskId,
                                TaskName = notif.TaskName,
                                BoardName = notif.BoardName,
                                Title = notif.Title,
                                Message = notif.Message
                            };
                            RealTimeNotifications.Add(item);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}