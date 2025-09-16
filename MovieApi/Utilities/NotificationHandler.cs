// This file is part of the project. Copyright (c) Company.

using MovieApi.Enums;
using MovieApi.Services.Notifications;

namespace MovieApi.Utilities;

public sealed class NotificationHandler(
    [FromKeyedServices(NotificationChannel.Email)]
    INotificationService emailService,
    [FromKeyedServices(NotificationChannel.Sms)]
    INotificationService smsService)
{
    public async Task HandleAsync(string message)
    {
        await emailService.SendAsync(message);
        await smsService.SendAsync(message);
    }
}
