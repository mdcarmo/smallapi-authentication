using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using SmallApiAuthentication.Domain.Contracts;
using SmallApiAuthentication.Domain.Entity;
using SmallApiAuthentication.Domain.Infra;
using SmallApiAuthentication.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SmallApiAuthentication.Domain.Manager
{
    /// <summary>
    /// 
    /// </summary>
    public class UserManager : IUserManager
    {
        private readonly LdapSettings _ldapconfig;
        private readonly LdapConnection _connection;
        private readonly AppSettings _appConfig;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ldapConfig"></param>
        /// <param name="appConfig"></param>
        public UserManager(IOptions<LdapSettings> ldapConfig, IOptions<AppSettings> appConfig)
        {
            _ldapconfig = ldapConfig.Value;
            _appConfig = appConfig.Value;

            _connection = new LdapConnection
            {
                SecureSocketLayer = true
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Result<User> Validate(string username, string password)
        {
            Result<User> result = new Result<User>
            {
                Content = GetUserInActiveDirectory(username, password)
            };

            if (result.Content == null)
                result.BusinessErrorMessages.Add("User not registered in Active Directory");

            return result;
        }

        private User GetUserInActiveDirectory(string username, string password)
        {
            User user = null;

            _connection.Connect(_ldapconfig.Hostname, LdapConnection.DEFAULT_SSL_PORT);
            _connection.Bind(string.IsNullOrWhiteSpace(_ldapconfig.Domain) ? username : $"{_ldapconfig.Domain}\\{username}", password);

            var resultSearch = _connection.Search(
                _ldapconfig.SearchBase,
                LdapConnection.SCOPE_SUB,
                string.Format(_ldapconfig.SearchFilter, username),
                new[] { "displayName", "sAMAccountName", "mail" },
                false
            );

            try
            {
                LdapEntry userAD = resultSearch.next();

                if (userAD != null)
                {
                    _connection.Bind(userAD.DN, password);

                    if (_connection.Bound)
                    {
                        user = new User
                        {
                            Name = userAD.getAttribute("displayName").StringValue,
                            Identity = userAD.getAttribute("sAMAccountName").StringValue,
                            Email = userAD.getAttribute("mail").StringValue
                        };
                    }

                    //get roles
                    List<string> groups = GetRoles(username).ToList();
                    IEnumerable<string> roles = groups.Where(stringToCheck => stringToCheck.Contains(_appConfig.RoleSystem));
                    user.Role = roles.Contains(_appConfig.RoleSystemAdm) ? "manager" : "user";
                }
            }
            catch
            {
                throw new Exception("Login failed.");
            }

            return user;
        }

        #region [ GetRoles(string user) ]
        /// <summary>
        /// Get Roles
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private IEnumerable<string> GetRoles(string user)
        {
            LdapSearchQueue searchQueue = _connection.Search(
                _ldapconfig.SearchBase,
                LdapConnection.SCOPE_SUB,
                string.Format(_ldapconfig.SearchFilter, user),
                new string[] { "cn", "memberOf" },
                false,
                null as LdapSearchQueue);


            LdapMessage message;
            while ((message = searchQueue.getResponse()) != null)
            {
                if (message is LdapSearchResult searchResult)
                {
                    LdapEntry entry = searchResult.Entry;
                    foreach (string value in HandleEntry(entry))
                        yield return value;
                }
                else
                    continue;
            }

            IEnumerable<string> HandleEntry(LdapEntry entry)
            {
                LdapAttribute attr = entry.getAttribute("memberOf");

                if (attr == null) yield break;

                foreach (string value in attr.StringValueArray)
                {
                    string groupName = GetGroup(value);
                    yield return groupName;
                }
            }

            string GetGroup(string value)
            {
                Match match = Regex.Match(value, "^CN=([^,]*)");

                if (!match.Success) return null;

                return match.Groups[1].Value;
            }
        }
        #endregion
    }

}
