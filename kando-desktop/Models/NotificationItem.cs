using System;
using kando_desktop.Resources.Strings;
using Microsoft.Maui.Graphics;

namespace kando_desktop.Models
{
    public class NotificationItem
    {
        public int Id { get; set; }
        public string NotificationType { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public int? TeamId { get; set; }
        public string TeamName { get; set; }
        public string TeamIcon { get; set; }
        public Color TeamColor { get; set; }
        public string OwnerName { get; set; }
        public int? TaskId { get; set; }
        public string TaskName { get; set; }
        public string BoardName { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }

        public string TimeAgo
        {
            get
            {
                var span = DateTime.UtcNow - CreatedAt;

                bool isSpanish = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLower() == "es";

                if (span.TotalMinutes < 1)
                    return AppResources.JustNow;

                if (span.TotalMinutes < 60)
                {
                    if (span.Minutes == 1) return AppResources.OneMinuteAgo;

                    return isSpanish
                        ? $"{AppResources.Ago} {span.Minutes} {AppResources.Minutes}"
                        : $"{span.Minutes} {AppResources.Minutes} {AppResources.Ago}";
                }

                if (span.TotalHours < 24)
                {
                    if (span.Hours == 1) return AppResources.OneHourAgo;

                    return isSpanish
                        ? $"{AppResources.Ago} {span.Hours} {AppResources.Hours}"
                        : $"{span.Hours} {AppResources.Hours} {AppResources.Ago}";
                }

                if (span.TotalDays < 7)
                {
                    if (span.Days == 1) return AppResources.OneDayAgo;

                    return isSpanish
                        ? $"{AppResources.Ago} {span.Days} {AppResources.Days}"
                        : $"{span.Days} {AppResources.Days} {AppResources.Ago}";
                }

                return CreatedAt.ToString("dd MMM yyyy", System.Globalization.CultureInfo.CurrentUICulture);
            }
        }
    }
}