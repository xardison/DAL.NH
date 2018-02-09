namespace DAL.NH
{
    public static class DalParams
    {
        public const int DefaultPageSize = 100;

        public static string ClientId { get; set; }
        public static string ClientInfo { get; set; }
        public static string ModuleName { get; set; }
        public static string ActionName { get; set; }

        public static void Init()
        {
        }
    }
}