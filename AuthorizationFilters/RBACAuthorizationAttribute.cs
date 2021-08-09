using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PSDWApp.Web.Common.Helpers;
using PSDWApp.Web.Common.Utilities;
using PSDWApp.Web.Services.Interfaces;
using PSDWApp.Web.ViewModels.ViewModel;

namespace PSDWApp.Web.Services.AuthorizationFilters
{
    /// <summary>
    /// RBACAuthorizationAttribute class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RBACAuthorizationAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter, IFilterMetadata
    {
        private readonly IRBACUserAuthorizationService iRBACUserAuthorizationService;
        private readonly IOptions<GlobalSettings> globalSettings;
        private readonly IDistributedCache distributedCache;
        private readonly IOptions<EndPoints> endPointsConfig;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly double cacheTiomeOff;
        private string username = string.Empty;

        // private RBACUser requestingUser;
        private List<RBACUserAuthViewModel> rBACUserAuthViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="RBACAuthorizationAttribute"/> class.
        /// </summary>
        /// <param name="distributedCache">distributedCache.</param>
        /// <param name="globalSettings">globalSettings.</param>
        /// <param name="iRBACUserAuthorizationService">iRBACUserAuthorizationService.</param>
        /// <param name="endPointsConfig">endPointsConfig.</param>
        /// <param name="httpContextAccessor">httpContextAccessor.</param>
        public RBACAuthorizationAttribute(IDistributedCache distributedCache, IOptions<GlobalSettings> globalSettings, IRBACUserAuthorizationService iRBACUserAuthorizationService, IOptions<EndPoints> endPointsConfig, IHttpContextAccessor httpContextAccessor)
        {
            this.globalSettings = globalSettings;
            this.endPointsConfig = endPointsConfig;
            this.httpContextAccessor = httpContextAccessor;
            if (globalSettings != null)
            {
                this.cacheTiomeOff = globalSettings.Value.TimeOff;
            }

            this.distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            this.iRBACUserAuthorizationService = iRBACUserAuthorizationService;
        }

        /// <inheritdoc/>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context != null)
            {
                var controllerInfo = context.ActionDescriptor as ControllerActionDescriptor;
                string controllerName = controllerInfo.ControllerName;
                object controller;
                context.RouteData.Values.TryGetValue("controller", out controller);
                object action;
                context.RouteData.Values.TryGetValue("action", out action);
                string requiredPermission = string.Format("{0}-{1}", controller, action);
                var isErrorController = context.ActionDescriptor.RouteValues[Constants.Controller].ToString().ToUpper() == Constants.Error.ToUpper() ? true : false;
                if (isErrorController)
                {
                    return;
                }

                /*Create permission string based on the requested controller
                name and action name in the format 'controllername-action'*/

                // Get from cache and if condtion
                // this.username = WindowsIdentity.GetCurrent().Name.ToString();
                // query for email address CC
                // query for department
                // query for Location
                var userCLaimCollection = this.httpContextAccessor.HttpContext.User.Claims;
                var currentUser = userCLaimCollection.Where(e => e.Type == ClaimTypes.Sid);
                this.username = currentUser.SingleOrDefault().Value;
                var cachedOutput = this.distributedCache.GetString(this.username);
                if (cachedOutput != null)
                {
                    // deserialized cachedOutput
                    this.rBACUserAuthViewModel = JsonConvert.DeserializeObject<List<RBACUserAuthViewModel>>(cachedOutput);
                }
                else
                {
                    this.rBACUserAuthViewModel = await this.iRBACUserAuthorizationService.GetRBACUserAuthList(this.endPointsConfig, this.username).ConfigureAwait(false);

                    if (this.rBACUserAuthViewModel != null)
                    {
                        string serializeUserRolePermissionData = JsonConvert.SerializeObject(this.rBACUserAuthViewModel);
                        this.distributedCache.SetString(this.username, serializeUserRolePermissionData, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromSeconds(this.cacheTiomeOff) });
                    }

                    // Get cache data
                    string getUserRolePermissionData = this.distributedCache.GetString(this.username);
                    this.rBACUserAuthViewModel = JsonConvert.DeserializeObject<List<RBACUserAuthViewModel>>(getUserRolePermissionData);
                }

                bool result = await this.iRBACUserAuthorizationService.HasPermission(requiredPermission).ConfigureAwait(false);
                if (!result)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "UnAuthorizedAccess" }, { "controller", "Error" } });
                    return;
                }
            }
        }
    }
}
