using System.Net;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using kando_desktop.Models;
using kando_desktop.Services.Contracts;
using kando_desktop.DTOs.Requests;
using kando_desktop.Resources.Strings;
using kando_desktop.Helpers;

namespace kando_desktop.ViewModels.Components
{
    public partial class NotificationTeamCardViewModel : ObservableObject
    {

        private readonly ITeamService _teamService;

        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private NotificationItem _notification;

        public NotificationTeamCardViewModel()
        {
            _teamService = Shell.Current.Handler.MauiContext.Services.GetService<ITeamService>();
            _notificationService = Shell.Current.Handler.MauiContext.Services.GetService<INotificationService>();
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task AcceptInvitation(NotificationItem notification)
        {
            if (notification?.TeamId == null) return;

            try
            {
                var response = await _teamService.UpdateInvitationAsync(notification.TeamId.Value, new UpdateInvitationDecisionDto { Status = "Active" });

                if (response)
                {
                    _notificationService.RealTimeNotifications.Remove(notification);

                    var workspaceService = ServiceHelper.GetService<IWorkspaceService>();
                    await _notificationService.SubscribeToTeamsAsync(new List<int> { notification.TeamId.Value });
                    await workspaceService.ForceRefreshAsync();

                    _notificationService.Show(AppResources.AcceptedInvitation);
                }
                else
                {
                    _notificationService.Show(AppResources.UnexpectedError, true);
                }
            }
            catch (HttpRequestException)
            {
                _notificationService.Show(AppResources.NetworkError, true);
            }
            catch
            {
                _notificationService.Show(AppResources.UnexpectedError, true);
            }
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task DeclineInvitation(NotificationItem notification)
        {
            if (notification?.TeamId == null) return;

            try
            {
                var response = await _teamService.UpdateInvitationAsync(notification.TeamId.Value, new UpdateInvitationDecisionDto
                {
                    Status = "Rejected"
                });

                if (response)
                {
                    _notificationService.RealTimeNotifications.Remove(notification);
                    _notificationService.Show(AppResources.RejectedInvitation);
                }
                else
                {
                    _notificationService.Show(AppResources.UnexpectedError, true);
                }
            }
            catch (HttpRequestException)
            {
                _notificationService.Show(AppResources.NetworkError, true);
            }
            catch
            {
                _notificationService.Show(AppResources.UnexpectedError, true);
            }
        }
    }
}