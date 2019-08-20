using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CalcSalaryApi.Data.Repository.Interface
{
    public interface IAuthRepository
    {
        Task<User> Login(string email, string password);
        Task<bool> UserExists(string email, string password);
    }
}