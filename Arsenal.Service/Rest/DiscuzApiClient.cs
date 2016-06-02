﻿using System.Diagnostics.Contracts;
using System.Linq;
using Arsenalcn.Core;

namespace Arsenal.Service
{
    public class DiscuzApiClient : RestClient
    {
        public DiscuzApiClient()
        {
            ServiceUrl = ConfigGlobal_Arsenal.APIServiceURL;
            AppKey = ConfigGlobal_Arsenal.APIAppKey;
            CryptographicKey = ConfigGlobal_Arsenal.APICryptographicKey;
        }

        #region Members and Properties

        public string AuthToken { get; set; }

        #endregion

        public string AuthValidate(string username, string password, string passwordFormat = "md5")
        {
            Contract.Requires(!string.IsNullOrEmpty(username));
            Contract.Requires(!string.IsNullOrEmpty(password));

            Method = "auth.validate";
            Format = ResponseType.JSON;

            SetDefaultParameters();

            Parameters.Add("user_name", username);
            Parameters.Add("password", password);

            if (!string.IsNullOrEmpty(passwordFormat))
            {
                Parameters.Add("password_format", passwordFormat);
            }

            return GetResponse();
        }

        public string AuthRegister(string username, string password, string email, string passwordFormat = "md5")
        {
            Contract.Requires(!string.IsNullOrEmpty(username));
            Contract.Requires(!string.IsNullOrEmpty(password));
            Contract.Requires(!string.IsNullOrEmpty(email));

            Method = "auth.register";
            Format = ResponseType.JSON;

            SetDefaultParameters();

            Parameters.Add("user_name", username);
            Parameters.Add("password", password);
            Parameters.Add("email", email);

            if (!string.IsNullOrEmpty(passwordFormat))
            {
                Parameters.Add("password_format", passwordFormat);
            }

            return GetResponse();
        }

        public string UsersChangePassword(int uid, string oldPassword, string newPassword, string passwordFormat = "md5")
        {
            Contract.Requires(uid > 0);
            Contract.Requires(!string.IsNullOrEmpty(oldPassword));
            Contract.Requires(!string.IsNullOrEmpty(newPassword));

            Method = "users.changepassword";
            Format = ResponseType.JSON;

            SetDefaultParameters();

            Parameters.Add("uid", uid.ToString());
            Parameters.Add("original_password", oldPassword);
            Parameters.Add("new_password", newPassword);
            Parameters.Add("confirm_new_password", newPassword);

            if (!string.IsNullOrEmpty(passwordFormat))
            {
                Parameters.Add("password_format", passwordFormat);
            }

            return GetResponse();
        }

        public string UsersGetID(string username)
        {
            Contract.Requires(!string.IsNullOrEmpty(username));

            Method = "users.getid";
            Format = ResponseType.JSON;

            SetDefaultParameters();

            Parameters.Add("user_name", username);

            return GetResponse();
        }

        public string UsersGetInfo(int[] uids, string[] fields)
        {
            Contract.Requires(uids.Length > 0);
            Contract.Requires(fields.Length > 0);

            Method = "users.getinfo";
            Format = ResponseType.JSON;

            SetDefaultParameters();

            Parameters.Add("uids", string.Join(",", uids.ToArray()));
            Parameters.Add("fields", string.Join(",", fields.ToArray()));

            return GetResponse();
        }
    }
}