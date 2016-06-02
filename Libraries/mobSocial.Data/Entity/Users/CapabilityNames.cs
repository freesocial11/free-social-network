namespace mobSocial.Data.Entity.Users
{
    public static class Capability<T>
    {
        public static string Insert
        {
            get { return "Insert." + typeof(T).FullName;}
        }
        public static string Update
        {
            get { return "Update." + typeof(T).FullName; }
        }
        public static string Delete
        {
            get { return "Delete." + typeof(T).FullName; }
        }
        public static string View
        {
            get { return "View." + typeof(T).FullName; }
        }
        public static string Print
        {
            get { return "Print." + typeof(T).FullName; }
        }
    }
}