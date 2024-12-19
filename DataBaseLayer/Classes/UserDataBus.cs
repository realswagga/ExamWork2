namespace DataBaseLayer.Classes
{
    //Класс для передачи данных пользователя между страницами
    public static class UserDataBus
    {
        public static int UserId { get; set; }
        public static int RoleId { get; set; }
        public static string Surname { get; set; }
        public static string Name { get; set; }
        public static string Patronymic { get; set; }
        public static string Login { get; set; }
        public static string Password { get; set; }
        public static string SearchQuery { get; set; }
    }
}
