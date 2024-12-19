namespace DataBaseLayer.Classes
{
    public class DataAccessLayer
    {
        public static void TransferUserData(User user)
        {
            UserDataBus.Name = user.Name;
            UserDataBus.Patronymic = user.Patronymic;
            UserDataBus.Surname = user.Surname;
            UserDataBus.Login = user.Login;
            UserDataBus.Password = user.Password;
            UserDataBus.RoleId = user.RoleId;
            UserDataBus.UserId = user.UserId;
        }
    }
}
