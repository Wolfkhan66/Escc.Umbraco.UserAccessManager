﻿using System.Collections.Generic;
using UmbracoUserControl.Models;
using UmbracoUserControl.ViewModel;

namespace UmbracoUserControl.Services.Interfaces
{
    public interface IUserControlService
    {
        IList<UmbracoUserModel> LookupUsers(FindUserModel model);

        ContentTreeViewModel LookupUserById(int id);

        bool InitiatePasswordReset(PasswordResetModel model, string url);

        bool ResetPassword(PasswordResetModel model);

        bool CreateUser(UmbracoUserModel model);

        bool ToggleLock(UmbracoUserModel umbracoUserModel);
    }
}