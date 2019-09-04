using AutoMapper;
using RedNimbus.Domain;
using System;
using System.Linq;
using UserService.Database.Model;

namespace UserService.Database
{
    public class UserRepository
    {
        private DatabaseContext context;
        private IMapper _mapper;

        public UserRepository(IMapper mapper)
        {
            this._mapper = mapper;
        }


        /// <summary>
        /// Adds a user to the database
        /// Throws: DbUpdateException if user with given email exists
        /// </summary>
        /// <param name="newUser"></param>
        public void SaveUser(User newUser)
        {
            UserDB userDB = ConvertUserToUserDB(newUser);

            //userDB.Id = newUser.Id;
            userDB.ActiveAccount = true;

            using (context = new DatabaseContext())
            {
                context.Database.EnsureCreated();
                context.Add(userDB);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Returns true if user exists in database
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool CheckIfExists(string email)
        {
            using (context = new DatabaseContext())
            {
                return context.Users.FirstOrDefault(u => u.Email == email && u.ActiveAccount) != null;
            }
        }

        public User GetUserByEmail(string email)
        {
            UserDB user;
            using (context = new DatabaseContext())
            {
                user = context.Users.First(u => u.Email.Equals(email));
            }

            if (user == null)
            {
                return null;
            }

            if (!user.ActiveAccount)
            {
                return null;
            }

            return ConvertUserDBToUser(user); 
            
        }

        public User GetUserById(Guid guid)
        {
            UserDB userDb;

            using (context = new DatabaseContext())
            {
                userDb = context.Users.First(u => u.Id.Equals(guid));
            }

            if (userDb == null)
            {
                return null;
            }
            if (!userDb.ActiveAccount)
            {
                return null;
            }

            return ConvertUserDBToUser(userDb);
        }

        private User ConvertUserDBToUser(UserDB userdb)
        {
            return _mapper.Map<User>(userdb);
        }

        private UserDB ConvertUserToUserDB(User user)
        {
            return _mapper.Map<UserDB>(user);
        }
    }
}
