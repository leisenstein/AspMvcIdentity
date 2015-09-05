using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCInBuiltFeatures.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using System.Data.Entity;

namespace MVCInBuiltFeatures.Controllers
{
    public class RolesController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();

        //
        // GET: /Roles/
        public async Task<ActionResult> Index()
        {
            var roles = await context.Roles.ToListAsync();
            return View(roles);
        }

        //
        // GET: /Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        public async Task<ActionResult> Create(FormCollection collection)
        {
            try
            {
                context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
                {
                    Name = collection["RoleName"]
                });
                await context.SaveChangesAsync();
                ViewBag.ResultMessage = "Role created successfully !";
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Roles/Edit/5
        public async Task<ActionResult> Edit(string roleName)
        {
            var thisRole = await context.Roles.Where(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync();

            return View(thisRole);
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Microsoft.AspNet.Identity.EntityFramework.IdentityRole role)
        {
            try
            {
                context.Entry(role).State = System.Data.Entity.EntityState.Modified;
                await context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Roles/Delete/5
        public async Task<ActionResult> Delete(string RoleName)
        {
            var thisRole = context.Roles.Where(r => r.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            context.Roles.Remove(thisRole);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public async Task<ActionResult> ManageUserRoles()
        {
            var roleList = await context.Roles.OrderBy(r => r.Name).ToListAsync();
            var selectRoleList = roleList.Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = selectRoleList;            
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RoleAddToUser(string UserName, string RoleName)
        {
            ApplicationUser user = await context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync();
            var account = new AccountController();
            await account.UserManager.AddToRoleAsync(user.Id, RoleName);
            
            ViewBag.ResultMessage = "Role created successfully !";

            // prepopulate roles for the view dropdown
            var roleList = await context.Roles.OrderBy(r => r.Name).ToListAsync();
            var selectRoleList = roleList.Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = selectRoleList;   

            return View("ManageUserRoles");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetRoles(string UserName)
        {            
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                ApplicationUser user = await context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync();
                var account = new AccountController();

                ViewBag.RolesForThisUser = await account.UserManager.GetRolesAsync(user.Id);

                // prepopulate roles for the view dropdown
                var roleList = await context.Roles.OrderBy(r => r.Name).ToListAsync();
                var selectRoleList = roleList.Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = selectRoleList;            
            }

            return View("ManageUserRoles");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteRoleForUser(string UserName, string RoleName)
        {
            var account = new AccountController();
            ApplicationUser user = await context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync();

            if (account.UserManager.IsInRole(user.Id, RoleName))  
            {
                await account.UserManager.RemoveFromRoleAsync(user.Id, RoleName);
                ViewBag.ResultMessage = "Role removed from this user successfully !";
            }
            else
            {
                ViewBag.ResultMessage = "This user doesn't belong to selected role.";
            }
            // prepopulat roles for the view dropdown
            var roleList = await context.Roles.OrderBy(r => r.Name).ToListAsync();
            var selectRoleList = roleList.Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = selectRoleList;

            return View("ManageUserRoles");
        }
    }
}
