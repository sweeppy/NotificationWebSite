using NotificationWebsite.Models;

namespace NotificationWebsite.DataAccess.Data
{
    public class UserRepository : IUserRepository
    {
        private UsersDbContext _db;
        public UserRepository(UsersDbContext db)
        {
            _db = db;
        }
        public User Create(User user)
        {
            _db.Users.Add(user);
            _db.SaveChanges();
            return user;
        }
    }
}