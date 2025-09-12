// This file is part of the project. Copyright (c) Company.

namespace MovieApi.Services.Notifications;

public class SmsNotificationService : INotificationService
{
    public Task SendAsync(string message)
    {
        Console.WriteLine($"SMS: {message}");
        return Task.CompletedTask;
    }
}
