// This file is part of the project. Copyright (c) Company.

namespace MovieApi.Services.Notifications;

public class EmailNotificationService : INotificationService
{
    public Task SendAsync(string message)
    {
        Console.WriteLine($"Email: {message}");
        return Task.CompletedTask;
    }
}
