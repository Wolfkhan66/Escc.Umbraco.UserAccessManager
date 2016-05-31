﻿using System.Collections.Generic;
using UmbracoUserControl.Models;
using UmbracoUserControl.ViewModel;

namespace UmbracoUserControl.Services.Interfaces
{
    public interface IDatabaseService
    {
        void DeleteResetDetails(UmbracoUserControl.Models.PasswordResetModel model);

        UmbracoUserControl.Models.PasswordResetModel GetResetDetails(UmbracoUserControl.Models.PasswordResetModel model);

        void SetResetDetails(UmbracoUserControl.Models.PasswordResetModel model);

        IEnumerable<PermissionsModel> CheckUserPermissions(ContentTreeViewModel model);

        void AddUserPermissions(PermissionsModel model);

        void RemoveUserPermissions(PermissionsModel databaseModel);

        void UpdateUserPermissions(int userId, IList<PermissionsModel> permissionsModelList);
    }
}