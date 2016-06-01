﻿using System.Collections.Generic;
using ESCC.Umbraco.UserAccessManager.Models;
using ESCC.Umbraco.UserAccessManager.ViewModel;

namespace ESCC.Umbraco.UserAccessManager.Services.Interfaces
{
    public interface IPermissionsControlService
    {
        IList<ContentTreeViewModel> GetContentRoot(ContentTreeViewModel userId);

        IList<ContentTreeViewModel> GetContentChild(ContentTreeViewModel userId);

        bool SetContentPermissions(ContentTreeViewModel model);

        bool RemoveContentPermissions(ContentTreeViewModel model);

        //bool SyncUserPermissions(int userId);

        bool ClonePermissions(int sourceId, int targetId);

        IEnumerable<PagePermissionsModel> CheckPagePermissions(string url);

        IList<PagePermissionsModel> CheckUserPermissions(FindUserModel model);

        IEnumerable<PagePermissionsModel> PagesWithoutAuthor();

        //void ToggleEditor(ContentTreeViewModel model);
    }
}