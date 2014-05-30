﻿using Castle.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UmbracoUserControl.Models;
using UmbracoUserControl.Services.Interfaces;

namespace UmbracoUserControl.Controllers
{
    public class ToolsController : Controller
    {
        private readonly IPermissionsControlService permissionsControlService;
        private readonly IUserControlService userControlService;

        public ToolsController(IPermissionsControlService permissionsControlService, IUserControlService userControlService)
        {
            this.permissionsControlService = permissionsControlService;
            this.userControlService = userControlService;
        }

        // GET: Tools
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CheckPagePermissions(string url)
        {
            var modelList = permissionsControlService.CheckPagePermissions(url);

            return !modelList.IsNullOrEmpty()
                ? PartialView("CheckPagePermissions", modelList)
                : PartialView("CheckPageError");
        }

        public ActionResult CheckUserPermissions(FindUserModel model)
        {
            var modelList = permissionsControlService.CheckUserPermissions(model);

            return !modelList.IsNullOrEmpty()
                ? PartialView("CheckUserPermissions", modelList)
                : PartialView("CheckUserError");
        }

        public ActionResult CheckPageWithoutAuthor()
        {
            var modelList = permissionsControlService.PagesWithoutAuthor();

            return !modelList.IsNullOrEmpty()
                ? PartialView("PagesWithoutAuthors", modelList)
                : PartialView("CheckPageError");
        }
    }
}