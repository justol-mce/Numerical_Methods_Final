using System;
using BusTicketingSystem.Models;
using BusTicketingSystem.Repositories;
using BusTicketingSystem.Exceptions;

namespace BusTicketingSystem.Controllers
{
    /// <summary>
    /// Controller - User
    /// Handles business logic for user operations (MVC Pattern)
    /// </summary>
    public class UserController
    {
        private UserRepository userRepository;

        public UserController()
        {
            userRepository = new UserRepository();
        }

        public void RegisterUser(User user)
        {
            User existingUser = userRepository.FindByUsername(user.Username);
            if (existingUser != null)
                throw new ValidationException("username", "Username already exists");

            userRepository.Save(user);
        }

        public User Login(string username, string password)
        {
            User user = userRepository.FindByUsername(username);
            if (user == null)
                throw new ValidationException("username", "User not found");

            if (!user.ValidateCredentials(password))
                throw new ValidationException("password", "Invalid password");

            return user;
        }

        public User GetUserProfile(string userId)
        {
            return userRepository.FindById(userId);
        }

        public void UpdateUserProfile(User user)
        {
            userRepository.Update(user);
        }

        public void DeactivateUser(string userId)
        {
            User user = userRepository.FindById(userId);
            if (user != null)
            {
                user.IsActive = false;
                userRepository.Update(user);
            }
        }
    }
}
