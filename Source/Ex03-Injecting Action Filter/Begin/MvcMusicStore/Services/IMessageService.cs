using System;
namespace MvcMusicStore.Services
{
    public interface IMessageService
    {
        string Message { get; set; }
        string ImageUrl { get; set; }
    }
}
