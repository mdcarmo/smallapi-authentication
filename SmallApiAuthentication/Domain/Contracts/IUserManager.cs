using SmallApiAuthentication.Domain.Entity;
using SmallApiAuthentication.Domain.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmallApiAuthentication.Domain.Contracts
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Result<User> Validate(string username, string password);
    }
}
