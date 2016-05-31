﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using UmbracoUserControl.Models;
using UmbracoUserControl.Services.Interfaces;
using UmbracoUserControl.ViewModel;

namespace UmbracoUserControl.Services
{
    public class UmbracoService : IUmbracoService
    {
        private readonly HttpClient client;

        public UmbracoService()
        {
            var siteUri = ConfigurationManager.AppSettings["SiteUri"];

            var handler = new HttpClientHandler
            {
                Credentials =
                    new NetworkCredential(ConfigurationManager.AppSettings["apiuser"],
                        ConfigurationManager.AppSettings["apikey"])
            };
            client = new HttpClient(handler) { BaseAddress = new Uri(siteUri) };
        }

        /// <summary>
        /// Sends a Get request to a web API with the given URL
        /// </summary>
        /// <param name="uriPath">URL of the web API method</param>
        /// <returns> Response code - Ok or BadGateway</returns>
        private HttpResponseMessage GetMessage(string uriPath)
        {
            var response = client.GetAsync(uriPath).Result;
            return response;
        }

        /// <summary>
        /// Sends a Post request to a web API wit the given URL
        /// </summary>
        /// <typeparam name="T model">Custom model dependong on method invoked</typeparam>
        /// <param name="uriPath"> of the web API method</param>
        /// <returns>List of items that matches the given model</returns>
        private HttpResponseMessage PostMessage<T>(string uriPath, T model)
        {
            var response = client.PostAsJsonAsync(uriPath, model).Result;
            return response;
        }

        /// <summary>
        /// Get request to Umbraco web service and retun a list of users with matching emailaddress
        /// </summary>
        /// <param name="emailaddress"></param>
        /// <returns>List of Umbraco User</returns>
        public IList<UmbracoUserModel> GetAllUsersByEmail(string emailaddress)
        {
            var response = GetMessage("GetAllUsersByEmail?emailaddress=" + emailaddress);

            if (response.IsSuccessStatusCode)
            {
                var modelList = response.Content.ReadAsAsync<IList<UmbracoUserModel>>().Result;
                return modelList;
            }
            var ex = response.Content.ReadAsAsync<Exception>().Result;
            throw ex;
        }

        public IList<UmbracoUserModel> GetAllUsersByUsername(string username)
        {
            var response = GetMessage("GetAllUsersByUsername?username=" + username);

            if (response.IsSuccessStatusCode)
            {
                var modelList = response.Content.ReadAsAsync<IList<UmbracoUserModel>>().Result;
                return modelList;
            }
            var ex = response.Content.ReadAsAsync<Exception>().Result;
            throw ex;
        }

        public ContentTreeViewModel GetAllUsersById(int id)
        {
            var response = GetMessage("GetUserById?id=" + id);

            if (response.IsSuccessStatusCode)
            {
                var model = response.Content.ReadAsAsync<ContentTreeViewModel>().Result;

                return model;
            }
            var ex = response.Content.ReadAsAsync<Exception>().Result;
            throw ex;
        }

        /// <summary>
        /// Post request to Umbraco web service to create a new user
        /// </summary>
        /// <param name="model">UmbracoUserModel - UserName, FullName and EmailAddress</param>
        public void CreateNewUser(UmbracoUserModel model)
        {
            var response = PostMessage("PostNewUsers", model);

            if (!response.IsSuccessStatusCode)
            {
                var ex = response.Content.ReadAsAsync<Exception>().Result;
                throw ex;
            }
        }

        /// <summary>
        /// Post request to Umbraco web service to reset a users password
        /// </summary>
        /// <param name="model">PasswordResetModel - UserId and NewPassword</param>
        public void ResetPassword(PasswordResetModel model)
        {
            var response = PostMessage("PostResetPassword", model);

            if (!response.IsSuccessStatusCode)
            {
                var ex = response.Content.ReadAsAsync<Exception>().Result;
                throw ex;
            }
        }

        /// <summary>
        /// Post request to umbraco web service to lock the users account
        /// </summary>
        /// <param name="model">UmbracoUserModel - UserId</param>
        public void DisableUser(UmbracoUserModel model)
        {
            var response = PostMessage("PostDisableUser", model);

            if (!response.IsSuccessStatusCode)
            {
                var ex = response.Content.ReadAsAsync<Exception>().Result;
                throw ex;
            }
        }

        /// <summary>
        /// Post request to umbraco web service to unlock the users account
        /// </summary>
        /// <param name="model">UmbracoUserModel - UserId</param>
        public void EnableUser(UmbracoUserModel model)
        {
            var response = PostMessage("PostEnableUser", model);

            if (!response.IsSuccessStatusCode)
            {
                var ex = response.Content.ReadAsAsync<Exception>().Result;
                throw ex;
            }
        }

        public IList<ContentTreeViewModel> GetContentRoot()
        {
            var response = GetMessage("GetContentRoot");

            if (response.IsSuccessStatusCode)
            {
                var modelList = response.Content.ReadAsAsync<IList<ContentTreeViewModel>>().Result;
                return modelList;
            }
            var ex = response.Content.ReadAsAsync<Exception>().Result;
            throw ex;
        }

        public IList<ContentTreeViewModel> GetContentChild(int id)
        {
            var response = GetMessage("GetContentChild?id=" + id);

            if (response.IsSuccessStatusCode)
            {
                var modelList = response.Content.ReadAsAsync<IList<ContentTreeViewModel>>().Result;
                return modelList;
            }
            var ex = response.Content.ReadAsAsync<Exception>().Result;
            throw ex;
        }

        public bool SetContentPermissions(PermissionsModel model)
        {
            var response = PostMessage("PostSetPermissions", model);

            if (response.IsSuccessStatusCode) return true;
            var ex = response.Content.ReadAsAsync<Exception>().Result;
            throw ex;
        }

        public bool RemoveContentPermissions(PermissionsModel model)
        {
            var response = PostMessage("PostRemovePermissions", model);

            if (response.IsSuccessStatusCode) return true;
            var ex = response.Content.ReadAsAsync<Exception>().Result;
            throw ex;
        }

        public IList<PermissionsModel> CheckUserPremissions(int userId)
        {
            var response = GetMessage("GetCheckUserPermissions?userId=" + userId);

            if (response.IsSuccessStatusCode)
            {
                var model = response.Content.ReadAsAsync<IList<PermissionsModel>>().Result;

                return model;
            }
            var ex = response.Content.ReadAsAsync<Exception>().Result;
            throw ex;
        }
    }
}