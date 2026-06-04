using System;
using System.Collections.Generic;
using System.Linq;
using BusTicketingSystem.Models;

namespace BusTicketingSystem.Repositories
{
    /// <summary>
    /// Repository - User
    /// Handles data access for user operations (In-Memory)
    /// </summary>
    public class UserRepository
    {
        private List<User> users = new List<User>();

        public void Save(User user)
        {
            users.Add(user);
        }

        public User FindById(string userId)
        {
            return users.FirstOrDefault(u => u.UserId == userId);
        }

        public User FindByUsername(string username)
        {
            return users.FirstOrDefault(u => u.Username == username);
        }

        public User FindByEmail(string email)
        {
            return users.FirstOrDefault(u => u.Email == email);
        }

        public List<User> FindAll()
        {
            return new List<User>(users);
        }

        public void Update(User user)
        {
            var existingUser = FindById(user.UserId);
            if (existingUser != null)
            {
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Address = user.Address;
                existingUser.IsActive = user.IsActive;
            }
        }
    }
}
