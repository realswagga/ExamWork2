using DataBaseLayer.Classes;
using DataBaseLayer.Data;

namespace DataBaseLayer.Services
{
    public class ExamUserService
    {
        private readonly ExamContext _context = new();

        public bool IsUser(string login, string password)
        {
            return _context.ExamUsers.Any(u => u.UserLogin == login && u.UserPassword == password);
        }

        public User GetUserData(string login, string password)
        {
            Models.ExamUser examUser = _context.ExamUsers
                .FirstOrDefault(u => u.UserLogin == login && u.UserPassword == password);

            User user = new User(examUser);

            if (user == null)
                throw new Exception("Пользователь с указанными данными не найден.");

            return user;
        }
    }
}
