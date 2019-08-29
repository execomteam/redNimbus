using Microsoft.EntityFrameworkCore;
using RedNimbus.DTO.Enums;
using RedNimbus.Either.Errors;
using RedNimbus.UserService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.DatabaseModel;

namespace UserService.Database
{
    public class UserRepository
    {
        private DatabaseContext context;


        /// <summary>
        /// Adds a user to the database
        /// Throws: DbUpdateException if user with given email exists
        /// </summary>
        /// <param name="newUser"></param>
        public void SaveUser(User newUser)
        {
            UserDB userDB = ConvertUserToUserDB(newUser);

            userDB.Id               = newUser.Id;
            userDB.ActiveAccount    = true;   

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
        public bool CheckIfExists(string email)
        {
            using(context = new DatabaseContext())
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
                if(user == null)
                {
                    return null;
                }
            }
            if(!user.ActiveAccount)
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
