using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PSDWApp.Web.Common.Helpers;
using PSDWApp.Web.Services.Interfaces;
using PSDWApp.Web.ViewModels.ViewModel;

namespace PSDWApp.Web.Services.AuthorizationFilters
{
    /// <summary>
    /// RBACControllerExtendedPermission static Class.
    /// </summary>
    public static class RBACControllerExtendedPermission
    {
        private static string username = string.Empty;
        private static List<RBACUserAuthViewModel> rBACUserAuthViewModel = null;

        static RBACControllerExtendedPermission()
        {
        }

        /// <summary>
        /// HasRoleAsync method.
        /// </summary>
        /// <param name="controller">controller.</param>
        /// <param name="currentuser">currentuser.</param>
        /// <param name="role">role.</param>
        /// <param name="distributedCache">distributedCache.</param>
        /// <param name="iRBACUserAuthorizationService">iRBACUserAuthorizationService.</param>
        /// <param name="endPointsConfig">endPointsConfig.</param>
        /// <param name="cacheTimeOff">cacheTimeOff.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public static async Task<bool> HasRoleAsync(this ControllerBase controller, string currentuser, string role, IDistributedCache distributedCache, IRBACUserAuthorizationService iRBACUserAuthorizationService, IOptions<EndPoints> endPointsConfig, double cacheTimeOff)
        {
            bool bFound = false;
            username = currentuser;
            string cachedOutput = distributedCache.GetString(username);
            try
            {
                if (cachedOutput != null)
                {
                    // deserialized cachedOutput data
                    rBACUserAuthViewModel = JsonConvert.DeserializeObject<List<RBACUserAuthViewModel>>(cachedOutput);
                    if (iRBACUserAuthorizationService != null)
                    {
                        bFound = await iRBACUserAuthorizationService.HasRole(role).ConfigureAwait(true);
                    }
                }
                else
                {
                    if (iRBACUserAuthorizationService != null)
                    {
                        rBACUserAuthViewModel = await iRBACUserAuthorizationService.GetRBACUserAuthList(endPointsConfig, currentuser).ConfigureAwait(true);
                    }

                    // Set cache data
                    string serializeUserRolePermissionData = JsonConvert.SerializeObject(rBACUserAuthViewModel);
                    distributedCache.SetString(username, serializeUserRolePermissionData, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromSeconds(cacheTimeOff) });

                    // Get cache data
                    cachedOutput = distributedCache.GetString(username);
                    rBACUserAuthViewModel = JsonConvert.DeserializeObject<List<RBACUserAuthViewModel>>(cachedOutput);

                    // Check if the requesting user has the specified role...
                    if (iRBACUserAuthorizationService != null)
                    {
                        bFound = await iRBACUserAuthorizationService.HasRole(role).ConfigureAwait(true);
                    }
                }
            }
            catch
            {
                throw;
            }

            return bFound;
        }

        /// <summary>
        /// HasRolesAsync method.
        /// </summary>
        /// <param name="controller">controller.</param>
        /// <param name="currentuser">currentuser.</param>
        /// <param name="roles">roles.</param>
        /// <param name="distributedCache">distributedCache.</param>
        /// <param name="iRBACUserAuthorizationService">iRBACUserAuthorizationService.</param>
        /// <param name="endPointsConfig">endPointsConfig.</param>
        /// <param name="cacheTimeOff">cacheTimeOff.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public static async Task<bool> HasRolesAsync(this ControllerBase controller, string currentuser, string roles, IDistributedCache distributedCache, IRBACUserAuthorizationService iRBACUserAuthorizationService, IOptions<EndPoints> endPointsConfig, double cacheTimeOff)
        {
            bool bFound = false;
            username = currentuser;
            string cachedOutput = distributedCache.GetString(username);

            try
            {
                if (cachedOutput != null)
                {
                    // deserialized cachedOutput data
                    rBACUserAuthViewModel = JsonConvert.DeserializeObject<List<RBACUserAuthViewModel>>(cachedOutput);
                    if (iRBACUserAuthorizationService != null)
                    {
                        bFound = await iRBACUserAuthorizationService.HasRoles(roles).ConfigureAwait(true);
                    }
                }
                else
                {
                    // Set cache data
                    if (iRBACUserAuthorizationService != null)
                    {
                        rBACUserAuthViewModel = await iRBACUserAuthorizationService.GetRBACUserAuthList(endPointsConfig, currentuser).ConfigureAwait(true);
                    }

                    string serializeUserRolePermissionData = JsonConvert.SerializeObject(rBACUserAuthViewModel);

                    distributedCache.SetString(username, serializeUserRolePermissionData, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromSeconds(cacheTimeOff) });

                    // Get cache data
                    cachedOutput = distributedCache.GetString(username);
                    rBACUserAuthViewModel = JsonConvert.DeserializeObject<List<RBACUserAuthViewModel>>(cachedOutput);

                    if (iRBACUserAuthorizationService != null)
                    {
                        bFound = await iRBACUserAuthorizationService.HasRoles(roles).ConfigureAwait(true);
                    }
                }
            }
            catch
            {
                throw;
            }

            return bFound;
        }

        /// <summary>
        /// HasPermission.
        /// </summary>
        /// <param name="controller">controller.</param>
        /// <param name="currentuser">currentuser.</param>
        /// <param name="permission">permission.</param>
        /// <param name="distributedCache">distributedCache.</param>
        /// <param name="iRBACUserAuthorizationService">iRBACUserAuthorizationService.</param>
        /// <param name="endPointsConfig">endPointsConfig.</param>
        /// <param name="cacheTimeOff">cacheTimeOff.</param>
        /// <returns>Task Of Bool.</returns>
        public static async Task<bool> HasPermissionAsync(this ControllerBase controller, string currentuser, string permission, IDistributedCache distributedCache, IRBACUserAuthorizationService iRBACUserAuthorizationService, IOptions<EndPoints> endPointsConfig, double cacheTimeOff)
        {
            bool bFound = false;
            username = currentuser;
            string cachedOutput = distributedCache.GetString(username);

            try
            {
                if (cachedOutput != null)
                {
                    // deserialized cachedOutput data
                    rBACUserAuthViewModel = JsonConvert.DeserializeObject<List<RBACUserAuthViewModel>>(cachedOutput);
                    if (iRBACUserAuthorizationService != null)
                    {
                        bFound = await iRBACUserAuthorizationService.HasPermission(permission).ConfigureAwait(false);
                    }
                }
                else
                {
                    // Set cache data
                    if (iRBACUserAuthorizationService != null)
                    {
                        rBACUserAuthViewModel = await iRBACUserAuthorizationService.GetRBACUserAuthList(endPointsConfig, currentuser).ConfigureAwait(false);
                    }

                    string serializeUserRolePermissionData = JsonConvert.SerializeObject(rBACUserAuthViewModel);
                    distributedCache.SetString(username, serializeUserRolePermissionData, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromSeconds(cacheTimeOff) });

                    // Get cache data
                    cachedOutput = distributedCache.GetString(username);
                    rBACUserAuthViewModel = JsonConvert.DeserializeObject<List<RBACUserAuthViewModel>>(cachedOutput);

                    if (iRBACUserAuthorizationService != null)
                    {
                        bFound = await iRBACUserAuthorizationService.HasPermission(permission).ConfigureAwait(false);
                    }
                }
            }
            catch
            {
                throw;
            }

            return bFound;
        }
    }
}
