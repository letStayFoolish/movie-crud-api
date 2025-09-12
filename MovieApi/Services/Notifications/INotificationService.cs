// This file is part of the project. Copyright (c) Company.

namespace MovieApi.Services.Notifications;

public interface INotificationService
{
    Task SendAsync(string message);
}
