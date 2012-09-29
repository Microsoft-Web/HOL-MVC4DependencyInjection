namespace MvcMusicStore.Services
{
    using System;

    public interface IMessageService
    {
        string Message { get; set; }

        string ImageUrl { get; set; }
    }
}
