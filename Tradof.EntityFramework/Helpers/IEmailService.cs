﻿namespace Tradof.EntityFramework.Helpers
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
