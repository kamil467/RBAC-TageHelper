using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using PSDWApp.Web.Common.Enums;
using PSDWApp.Web.Common.Utilities;
using PSDWApp.Web.Services.Interfaces;

namespace PSDWApp.TagHelpers
{
    /// <summary>
    /// Custom TagHelper for input type.
    /// </summary>
    [HtmlTargetElement(Constants.InputTypeText)]
    [HtmlTargetElement(Constants.InputTypeButton)]
    [HtmlTargetElement(Constants.InputTypeDropDown)]
    public class IntelligentControl : TagHelper
    {
        private readonly IUserRolesService userRolesService;

        /// <summary>
        /// Gets or sets viewContext.
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntelligentControl"/> class.
        /// </summary>
        /// <param name="userRolesService">userRolesService.</param>
        public IntelligentControl(IUserRolesService userRolesService)
        {
            this.userRolesService = userRolesService;
        }


        /// <summary>
        /// override process.
        /// </summary>
        /// <param name="context">context.</param>
        /// <param name="output">output.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
              await Task.Run(() => {

                if (context == null || output == null)
                {
                    throw new ArgumentNullException();
                }
				var permissionType = 'R';
               
			   if(this.viewContext.ViewData["RoleConreolAttribute"]!=null)
			   {
				   permissionType =  Convert.ToChar(this.viewContext.ViewData["RoleConreolAttribute"]);      // RoleControlAttribute will be identified during Filter pipeline and passed as view data to current view.
			   }
              
			  // below two lines are not required if RoleControlattribute identified at Authorization filter level.
			  
               // var rBacUsermodel = this.userRolesService.GetAllRoles().Result;

                // var permissionType = this.userRolesService.GetPermissionType(rBacUsermodel, this.ViewContext, context);

                
                if (permissionType == Convert.ToChar(RBACPermissionEnum.Read))
                {
                    if (context.TagName.ToUpper() == Constants.InputTypeDropDown.ToUpper() || context.TagName.ToUpper() == Constants.InputTypeButton.ToUpper())
                    {
                        output.Attributes.SetAttribute(new TagHelperAttribute(Constants.DisabledAttr));
                    }
                    else if (context.TagName.ToUpper() == Constants.InputTypeText.ToUpper())
                    {
                        output.Attributes.SetAttribute(new TagHelperAttribute(Constants.ReadonlyAttr));
                    }
                }
                else
                {
                    if (context.TagName.ToUpper() == Constants.InputTypeDropDown.ToUpper() || context.TagName.ToUpper() == Constants.InputTypeButton.ToUpper())
                    {
                        output.Attributes.Remove(new TagHelperAttribute(Constants.DisabledAttr));
                    }
                    else if (context.TagName.ToUpper() == Constants.InputTypeText.ToUpper())
                    {
                        output.Attributes.Remove(new TagHelperAttribute(Constants.ReadonlyAttr));
                    }
                }
            }).ConfigureAwait(false);
        }
    }
}
