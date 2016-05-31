﻿using Castle.Core.Logging;
using Microsoft.ApplicationBlocks.ExceptionManagement;
using System;
using UmbracoUserControl.Models;

namespace UmbracoUserControl.Services
{
    public class UserControlService : IUserControlService
    {
        private IUmbracoService umbracoService;
        private IDatabaseService databaseService;
        private IEmailService emailService;

        public ILogger Logger { get; set; }

        public UserControlService(IDatabaseService databaseService, IUmbracoService umbracoService, IEmailService emailService)
        {
            this.databaseService = databaseService;
            this.umbracoService = umbracoService;
            this.emailService = emailService;
            this.Logger = Logger;
        }

        /// <summary>
        /// Creates database record and emails user to start reset process
        /// </summary>
        /// <param name="model">PasswordResetModel - EmailAddress and UserId Required</param>
        /// <param name="url">root url for the site eg http://localhost:53201/ </param>
        /// <returns>success bool - all operations complete without error</returns>
        public bool InitiatePasswordReset(PasswordResetModel model, string url)
        {
            model.UniqueResetId = Guid.NewGuid().ToString();

            model.TimeLimit = DateTime.Now;

            try
            {
                databaseService.SetResetDetails(model);

                emailService.PasswordResetEmail(model, url);

                return true;
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("{0} Password reset {1}, {2} could not be initiated - error message {3} - Stack trace {4} - inner exception {5}", DateTime.Now, model.UniqueResetId, model.UserId, ex.Message, ex.StackTrace, ex.InnerException);

                ExceptionManager.Publish(ex);

                throw;
            }
        }

        /// <summary>
        /// Given a valid model, validate agaist database then reset password
        /// </summary>
        /// <param name="model">PasswordResetModel - UniqueResetId, UserId, NewPassword</param>
        /// <returns>success bool - all operations complete without error</returns>
        public bool ResetPassword(PasswordResetModel model)
        {
            try
            {
                var validRequests = databaseService.GetResetDetails(model);

                model.EmailAddress = validRequests.EmailAddress;

                if (DateTime.Now.AddSeconds(-5) <= validRequests.TimeLimit.AddDays(1))
                {
                    umbracoService.ResetPassword(model);

                    databaseService.DeleteResetDetails(model);

                    emailService.PasswordResetConfirmationEmail(model);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("{0} Password reset {1}, {2} could not be actioned - error message {3} - Stack trace {4} - inner exception {5}", DateTime.Now, model.UniqueResetId, model.UserId, ex.Message, ex.StackTrace, ex.InnerException);

                ExceptionManager.Publish(ex);

                throw;
            }
            return false;
        }

        /// <summary>
        /// Given a valid model, Create a new user and email the admin team
        /// </summary>
        /// <param name="model">UmbracoUserModel - UserName, FullName, EmailAddress</param>
        /// <returns>success bool - all operations complete without error</returns>
        public bool CreateUser(UmbracoUserModel model)
        {
            try
            {
                umbracoService.CreateNewUser(model);

                emailService.CreateNewUserEmail(model);

                return true;
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("{0} User creation {1}, {2} could not be actioned - error message {3} - Stack trace {4} - inner exception {5}", DateTime.Now, model.UserName, model.UserId, ex.Message, ex.StackTrace, ex.InnerException);

                ExceptionManager.Publish(ex);

                throw;
            }
        }

        /// <summary>
        /// Locks / unlock the users umbraco account
        /// </summary>
        /// <param name="model">UmbracoUserModel - UserId</param>
        /// <returns>success bool - all operations complete without error</returns>
        public bool ToggleLock(UmbracoUserModel model)
        {
            try
            {
                if (model.Lock == false)
                {
                    umbracoService.DisableUser(model);
                }
                else
                {
                    umbracoService.EnableUser(model);
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("{0} lock / unlock on user account {1}, {2} could not be actioned - error message {3} - Stack trace {4} - inner exception {5}", DateTime.Now, model.UserName, model.UserId, ex.Message, ex.StackTrace, ex.InnerException);

                ExceptionManager.Publish(ex);

                throw;
            }
        }
    }
}