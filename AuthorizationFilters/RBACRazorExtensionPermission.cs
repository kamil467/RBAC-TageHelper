using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PSDWApp.Web.Common.Helpers;
using PSDWApp.Web.Services.Interfaces;
using PSDWApp.Web.ViewModels.ViewModel;

namespace PSDWApp.Web.Services.AuthorizationFilters
{
    /// <summary>
    /// RBACRazorExtensionPermission static Class.
    /// </summary>
    public static class RBACRazorExtensionPermission
    {
        private static List<RBACUserAuthViewModel> rBACUserAuthViewModel;

        private static string username = string.Empty;

        static RBACRazorExtensionPermission()
        {
        }

        /// <summary>
        /// HasRoleAsync.
        /// </summary>
        /// <param name="currentuser">currentuser.</param>
        /// <param name="role">role.</param>
        /// <param name="distributedCache">distributedCache.</param>
        /// <param name="iRBACUserAuthorizationService">iRBACUserAuthorizationService.</param>
        /// <param name="endPointsConfig">endPointsConfig.</param>
        /// <param name="cacheTimeOff">cacheTimeOff.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public static async Task<bool> HasRoleAsync(string currentuser, string role, IDistributedCache distributedCache, IRBACUserAuthorizationService iRBACUserAuthorizationService, IOptions<EndPoints> endPointsConfig, double cacheTimeOff)
        {
            bool bFound = false;
            username = currentuser;
            string cachedOutput = distributedCache.GetString(username);

            try
            {
                // Check if the requesting user has the specified role...
                if (cachedOutput != null)
                {
                    // deserialized cachedOutput object
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
        /// <param name="currentuser">currentuser.</param>
        /// <param name="roles">roles.</param>
        /// <param name="distributedCache">distributedCache.</param>
        /// <param name="iRBACUserAuthorizationService">iRBACUserAuthorizationService.</param>
        /// <param name="endPointsConfig">endPointsConfig.</param>
        /// <param name="cacheTimeOff">cacheTimeOff.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public static async Task<bool> HasRolesAsync(string currentuser, string roles, IDistributedCache distributedCache, IRBACUserAuthorizationService iRBACUserAuthorizationService, IOptions<EndPoints> endPointsConfig, double cacheTimeOff)
        {
            bool bFound = false;
            username = currentuser;
            string cachedOutput = distributedCache.GetString(username);

            try
            {
                // Check if the requesting user has any of the specified roles...
                // Make sure you separate the roles using ';' (ie "Sales Manager;Sales Operator")
                if (cachedOutput != null)
                {
                    // deserialized cachedOutput object
                    rBACUserAuthViewModel = JsonConvert.DeserializeObject<List<RBACUserAuthViewModel>>(cachedOutput);
                    if (iRBACUserAuthorizationService != null)
                    {
                        bFound = await iRBACUserAuthorizationService.HasRoles(roles).ConfigureAwait(true);
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
                        bFound = await iRBACUserAuthorizationService.HasRole(roles).ConfigureAwait(true);
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
        /// HasPermissionAsync method.
        /// </summary>
        /// <param name="currentuser">currentuser.</param>
        /// <param name="permission">permission.</param>
        /// <param name="distributedCache">distributedCache.</param>
        /// <param name="iRBACUserAuthorizationService">iRBACUserAuthorizationService.</param>
        /// <param name="endPointsConfig">endPointsConfig.</param>
        /// <param name="cacheTimeOff">cacheTimeOff.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public static async Task<bool> HasPermissionAsync(string currentuser, string permission, IDistributedCache distributedCache, IRBACUserAuthorizationService iRBACUserAuthorizationService, IOptions<EndPoints> endPointsConfig, double cacheTimeOff)
        {
            bool bFound = false;
            username = currentuser;
            string cachedOutput = distributedCache.GetString(username);
            try
            {
                // Check if the requesting user has the specified application permission...
                if (cachedOutput != null)
                {
                    // deserialized cachedOutput object
                    rBACUserAuthViewModel = JsonConvert.DeserializeObject<List<RBACUserAuthViewModel>>(cachedOutput);
                    if (iRBACUserAuthorizationService != null)
                    {
                        bFound = await iRBACUserAuthorizationService.HasPermission(permission).ConfigureAwait(true);
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
                    if (cachedOutput != null)
                    {
                        rBACUserAuthViewModel = JsonConvert.DeserializeObject<List<RBACUserAuthViewModel>>(cachedOutput);
                    }

                    // Check if the requesting user has the specified role...
                    if (iRBACUserAuthorizationService != null)
                    {
                        bFound = await iRBACUserAuthorizationService.HasPermission(permission).ConfigureAwait(true);
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
