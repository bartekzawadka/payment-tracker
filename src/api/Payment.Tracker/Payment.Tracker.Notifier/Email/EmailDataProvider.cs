using System;
using Payment.Tracker.Notifier.Models;

namespace Payment.Tracker.Notifier.Email
{
    public static class EmailDataProvider
    {
        public static string GetEmailTitle(NotificationType notificationType) =>
            notificationType switch
            {
                NotificationType.Precursory => "Nowy okres rozliczeniowy",
                NotificationType.CreateSetReminder => "Nowy okres rozliczeniowy - przypomnienie",
                NotificationType.ClosePeriodReminder => "Zamknij miesiąc rozliczeniowy!",
                _ => throw new ArgumentException($"Unknown notification type: {notificationType}")
            };

        public static EmailContent GetEmailContent(
            string appEndpoint,
            NotificationType notificationType,
            DateTime month)
        {
            switch (notificationType)
            {
                case NotificationType.Precursory:
                    var html = $"Nadchodzi nowy okres rozliczeniowy za {month:MM-yyyy}." +
                               $"\r\n<a href=\"{appEndpoint}/payment-set\">Kliknij tutaj</a> aby utworzyć nowy set.";
                    var plain = $"Nadchodzi nowy okres rozliczeniowy za {month:MM-yyyy}." +
                                $"\r\nKliknij tu: {appEndpoint}/payment-set aby utworzyć nowy set.";
                    return new EmailContent
                    {
                        PlainText = plain,
                        HtmlText = html
                    };
                case NotificationType.CreateSetReminder:
                    return new EmailContent
                    {
                        HtmlText = "Przypomnienie o rozpoczętym nowym okresie rozliczeniowym.\r\n" +
                                   $"<a href=\"{appEndpoint}/payment-set\">Kliknij tutaj</a> aby utworzyć nowy set.",
                        PlainText = "Przypomnienie o rozpoczętym nowym okresie rozliczeniowym.\r\n" +
                                    $"Kliknij tu: {appEndpoint}/payment-set aby utworzyć nowy set."
                    };
                case NotificationType.ClosePeriodReminder:
                    var text = "Zakończ wszystkie płatności i pamiętaj o zaksięgowaniu faktur za bieżący miesiąc!";
                    return new EmailContent
                    {
                        HtmlText = $"{text}\r\nBieżące płatności:" +
                                   $" <a href=\"{appEndpoint}/payment-set/current\">" +
                                   $"{appEndpoint}/payment-set/current" +
                                   $"</a>",
                        PlainText =
                            $"{text}\r\nBieżące płatności: {appEndpoint}/payment-set/current"
                    };
                default:
                    throw new ArgumentException($"Unknown notification type: {notificationType}");
            }
        }
    }
}