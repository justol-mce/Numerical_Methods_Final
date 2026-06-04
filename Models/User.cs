using System;

namespace BusTicketingSystem.Models
{
    /// <summary>
    /// Domain Model - User
    /// Represents a user account with authentication and profile information
    /// </summary>
    public class User
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string UserType { get; set; } // CUSTOMER, ADMIN
        public bool IsActive { get; set; }

        public User()
        {
            UserId = Guid.NewGuid().ToString();
            IsActive = true;
            UserType = "CUSTOMER";
        }

        public User(string username, string email, string password, string fullName,
                    string phoneNumber, string address)
            : this()
        {
            Username = username;
            Email = email;
            Password = password;
            FullName = fullName;
            PhoneNumber = phoneNumber;
            Address = address;
        }

        public bool ValidateCredentials(string inputPassword)
        {
            return Password.Equals(inputPassword);
        }

        public override string ToString()
        {
            return $"User{{ UserId={UserId}, Username={Username}, Email={Email}, FullName={FullName} }}";
        }
    }
}
