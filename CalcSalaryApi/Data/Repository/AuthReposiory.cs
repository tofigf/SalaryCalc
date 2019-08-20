using CalcSalaryApi.Data.Repository.Interface;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;

namespace CalcSalaryApi.Data.Repository
{
    public class AuthReposiory: IAuthRepository
    {
        private readonly DataContext _context;
        public AuthReposiory(DataContext context)
        {
            _context = context;
        }
        //Login:
        public async Task<User> Login(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
                return null;
            if (!Crypto.VerifyHashedPassword(user.Password, password))
                return null;
            if (user == null)
                return null;

            return user;
        }
        //Check:
        public async Task<bool> UserExists(string email, string password)
        {
            var loginned = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (!await _context.Users.AnyAsync(x => x.Email == email))
                return true;

            if (!Crypto.VerifyHashedPassword(loginned.Password, password))
                return true;

            return false;
        }
    }
}