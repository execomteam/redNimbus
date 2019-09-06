using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RedNimbus.Domain;
using System;
using UserService.Database;
using UserService.Mapping;
namespace UserService.Test
{
    [TestFixture]
    public class UserRepositoryTests
    {
        [TestCase]
        public void When_TryToLogInWithValidEmail_Expects_UserObjectsFromDatabase()
        {
            //Arrange
            var userEmail = "enisnerbajin@gmail.com";

            var mapperConfiguration = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });
            var mapper              = mapperConfiguration.CreateMapper();
            var userRepository      = new UserRepository(mapper);

            if (!userRepository.CheckIfExists(userEmail))
            {
                User newUser = new User
                {
                    FirstName   = "Emil",
                    LastName    = "NisnerBajin",
                    Password    = "@Testiranje97",
                    Email       = userEmail,
                    PhoneNumber = "075723654"
                };
                userRepository.SaveUser(newUser);
            }

            //Act
            User userFromDB = userRepository.GetUserByEmail(userEmail);

            //Assert
            Assert.AreEqual(userFromDB.Email, userEmail);
        }

        [TestCase]
        public void When_TryToLogInWithInvalidEmail_Expects_ArgumentNullExceptionThrown()
        {
            //Arange
            var userEmail = "thisIsInvalidEmailAdress@gmail.com";

            
            var mapperConfiguration = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });
            var mapper = mapperConfiguration.CreateMapper();
            var userRepository = new UserRepository(mapper);

            if (userRepository.CheckIfExists(userEmail))
            {
                userRepository.RemoveUser(userEmail);
            }

            //Act & Assert
            Assert.Catch(typeof(InvalidOperationException), () => userRepository.GetUserByEmail(userEmail));
        }

        [TestCase]
        public void When_CheckIfEmailExistsInDatabase_Expects_True()
        {
            //Arange
            var userEmail = "enisnerbajin@gmail.com";
            var mapperConfiguration = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });
            var mapper = mapperConfiguration.CreateMapper();
            var userRepository = new UserRepository(mapper);

            //Act
            var result = userRepository.CheckIfExists(userEmail);

            //Assert
            Assert.AreEqual(result, true);
        }

        [TestCase]
        public void When_CheckIfEmailExistsInDatabase_Expects_False()
        {
            //Arange
            var userEmail = "thisemailisnotstored@gmail.com";
            var mapperConfiguration = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });
            var mapper = mapperConfiguration.CreateMapper();
            var userRepository = new UserRepository(mapper);

            //Act
            var result = userRepository.CheckIfExists(userEmail);

            //Assert
            Assert.AreEqual(result, false);
        }

        [TestCase]
        public void When_TryToRegisterWithNewEmail_Expects_NoExceptionTrown()
        {
            //Arange
            var newUser = new User
            {
                FirstName = "Aleksandar",
                LastName = "Zubanov",
                Email = "aleksandarZubanov3@gmail.com",
                Password = "@Testiranje97",
                PhoneNumber = "09090909009"
            };

            var mapperConfiguration = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });
            var mapper = mapperConfiguration.CreateMapper();
            var userRepository = new UserRepository(mapper);

            if (userRepository.CheckIfExists(newUser.Email))
            {
                userRepository.RemoveUser(newUser.Email);
            }

            //Act & Assert
            Assert.DoesNotThrow(()=>userRepository.SaveUser(newUser));
        }

        [TestCase]
        public void When_TryToRegisterWithAlreadyExistingEmail_Expects_DBUpdateExceptionThrown()
        {
            //Arange
            var newUser = new User
            {
                FirstName   = "Aleksandar",
                LastName    = "Zubanov",
                Email       = "aleksandarZubanov12@gmail.com",
                Password    = "@Testiranje97",
                PhoneNumber = "09090909009"
            };

            var mapperConfiguration = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });
            var mapper = mapperConfiguration.CreateMapper();
            var userRepository = new UserRepository(mapper);

            if (!userRepository.CheckIfExists(newUser.Email))
            {
                userRepository.SaveUser(newUser);
            }

            //Act & Assert
            Assert.Throws(typeof(DbUpdateException),() => userRepository.SaveUser(newUser));
        }
    }
}
