namespace MvcMusicStore.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class MessageService : IMessageService
    {
        public string Message { get; set; }

        public string ImageUrl { get; set; } 
    }
}