﻿namespace MvcMusicStore.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using MvcMusicStore.Models;

    public class CustomActionFilter : ActionFilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            // TODO: Add your acction filter's tasks here

            // Log Action Filter Call
            MusicStoreEntities storeDB = new MusicStoreEntities();

            ActionLog log = new ActionLog()
            {
                Controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                Action = filterContext.ActionDescriptor.ActionName + " (Logged By: Custom Action Filter)",
                IP = filterContext.HttpContext.Request.UserHostAddress,
                DateTime = filterContext.HttpContext.Timestamp
            };

            storeDB.ActionLog.Add(log);
            storeDB.SaveChanges();

            this.OnActionExecuting(filterContext);
        }
    }
}