namespace MvcMusicStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class ActionLog
    {
        public int ActionLogId { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string IP { get; set; }

        public DateTime? DateTime { get; set; }
    }
}