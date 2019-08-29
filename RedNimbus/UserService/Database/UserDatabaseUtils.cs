using Microsoft.EntityFrameworkCore;
using RedNimbus.UserService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.DatabaseModel;

namespace UserService.Database
{
    public class UserDatabaseUtils
    {
        private DatabaseContext context;


        /// <summary>
        /// Adds a user to the database
        /// Throws: DbUpdateException if user with given email exists
        /// </summary>
        /// <param name="newUser"></param>
        public void RegisterUser(User newUser)
        {
            UserDB userDB = ConvertUserToUserDB(newUser);

            userDB.Id               = newUser.Id;
            userDB.ActiveAccount    = 1;   

            using(context = new DatabaseContext())
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
        public bool CheckIfAlreadyRegistered(string email)
        {
            using(context = new DatabaseContext())
            {
                try
                {
                    UserDB user = context.Users.First(u => u.Email.Equals(email));
                    return true;
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            }
        }

        public User GetUserByEmail(string email)
        {
            UserDB user;
            using (context = new DatabaseContext())
            {
                user = context.Users.First(u => u.Email.Equals(email));
                if(user == null)
                {
                    return null;
                }
            }
            if(user.ActiveAccount == 0)
            {
                return null;
            }
            else
            {
                User result = ConvertUserDBToUser(user);
                return result;
            }
        }

        private User ConvertUserDBToUser(UserDB userdb)
        {
            return new User
            {
                FirstName   = userdb.FirstName,
                LastName    = userdb.LastName,
                Email       = userdb.Email,
                PhoneNumber = userdb.PhoneNumber,
                Password    = userdb.Password
            };
        }

        private UserDB ConvertUserToUserDB(User user)
        {
            return new UserDB
            {
                FirstName   = user.FirstName,
                LastName    = user.LastName,
                Email       = user.Email,
                PhoneNumber = user.PhoneNumber,
                Password    = user.Password
            };
        }
    }
}
