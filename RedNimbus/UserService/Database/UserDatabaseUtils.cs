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

        //Constructor
        public UserDatabaseUtils(){ }

        /// <summary>
        /// Adds a user to the database
        /// Throws: DbUpdateException if user with given email exists
        /// </summary>
        /// <param name="newUser"></param>
        public void RegisterUser(User newUser)
        {
            UserDB userDB = new UserDB();

            userDB.FirstName        = newUser.FirstName;
            userDB.LastName         = newUser.LastName;
            userDB.Password         = newUser.Password;
            userDB.PhoneNumber      = newUser.PhoneNumber;
            userDB.Email            = newUser.Email;
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
                UserDB user = context.Users.First(u => u.Email.Equals(email));
                if (user != null)
                {
                    return true;
                }
                else
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
            User user = new User
            {
                FirstName   = userdb.FirstName,
                LastName    = userdb.LastName,
                Email       = userdb.Email,
                PhoneNumber = userdb.PhoneNumber,
                Password    = userdb.Password
            };
            return user;
        }

        private UserDB ConvertUserToUserDB(User user)
        {
            UserDB userdb = new UserDB
            {
                FirstName   = user.FirstName,
                LastName    = user.LastName,
                Email       = user.Email,
                PhoneNumber = user.PhoneNumber,
                Password    = user.Password
            };
            return userdb;
        }

        public bool DeactivateUserAccount(String userEmail)
        {
            return true;  
        }

        public bool ActivateUserAccount(String userEmail)
        {
            return true;
        }
    }
}
