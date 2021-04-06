using System;
using System.Collections.Generic;
using System.Linq;
using FoosballApi.Data;
using FoosballApi.Models;

namespace FoosballApi.Services
{
    public interface IUserService
    {
        bool SaveChanges();
        IEnumerable<User> GetAllUsers();
        User GetUserByEmail(string email);
        User GetUserById(int id);
        void UpdateUser(User user);
        void DeleteUser(User user);
    }

    public class UserService : IUserService
    {
        private readonly DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public void DeleteUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            _context.Users.Remove(user);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public User GetUserByEmail(string email)
        {
            var query = from u in _context.Users
                        where u.Email == email
                        select u;
            var user = query.FirstOrDefault<User>();
            return user;
        }

        public User GetUserById(int id)
        {
            return _context.Users.FirstOrDefault(p => p.Id == id);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateUser(User user)
        {
            // Do nothing
        }
    }
}