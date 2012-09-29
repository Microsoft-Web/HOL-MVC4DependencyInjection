namespace MvcMusicStore
{
    using System.Data.Entity;
    using MvcMusicStore.Models;

    public static class AppConfig
    {
        public static void Configure()
        {
            Database.SetInitializer(new SampleData());
        }
    }
}