using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebstoreMVC.Data;
using WebstoreMVC.Models;

namespace WebstoreMVC.Controllers
{
    [Authorize(Policy = "RequireAdminRole")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        //DependencyInjection - IOC-container determines services for whole project.
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._db = db;
        }

        public string GetAdminRoleId()
        {
            string result = "b6e1c5ac-4573-4037-a7bd-3bb1dcc137d5";
            //string RoleIdAdmin = _roleManager.Roles.Where(r => r.Name == "Admin").Select(r => r.Id).ToString();
            //result = RoleIdAdmin;
            return result;
        }

        public string GetStoreOwnerRoleId()
        {
            string result = "0bfc3c14-a02b-4fa2-85a3-59856226f962";
            //string RoleIdStoreOwner = _roleManager.Roles.Where(r => r.Name == "StoreOwner").Select(r => r.Id).ToString();
            //result = RoleIdStoreOwner;
            return result;
        }

        public IActionResult Index()
        {
            List<string> adminUserIds = _db.UserRoles.Where(a => a.RoleId == GetAdminRoleId()).Select(b => b.UserId).Distinct().ToList();
            List<string> storeOwnerUserIds = _db.UserRoles.Where(a => a.RoleId == GetStoreOwnerRoleId()).Select(b => b.UserId).Distinct().ToList();

            var listAdmins = _userManager.Users.Where(a => adminUserIds.Any(c => c == a.Id)).ToList();
            var listStoreOwners = _userManager.Users.Where(a => storeOwnerUserIds.Any(c => c == a.Id)).ToList();
            var listNoAdmin = _userManager.Users.Where(a => adminUserIds.Any(c => c != a.Id)).ToList();
            var listNoRole = listNoAdmin.Where(a => storeOwnerUserIds.Any(c => c != a.Id)).ToList();

            var model = new ApplicationUserListViewModel();
            foreach (var user in listAdmins)
            {
                var listItem = new ApplicationUserListItemViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    RoleName = "Admin"
                };
                model.ListItems.Add(listItem);
            }
            foreach (var user in listStoreOwners)
            {
                var listItem = new ApplicationUserListItemViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    RoleName = "StoreOwner"
                };
                model.ListItems.Add(listItem);
            }
            foreach (var user in listNoRole)
            {
                var listItem = new ApplicationUserListItemViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    RoleName = "Customer"
                };
                model.ListItems.Add(listItem);
            }

            return View(model);
        }
        
        public async Task<ActionResult> AddAdminRole(string id)
        {
            var user = _userManager.Users.Where(u => u.Id == id).FirstOrDefault();
            if (user != null)
            {
                List<string> roles = new List<string>() { "Admin", "StoreOwner" };
                await _userManager.RemoveFromRolesAsync(user, roles);

                var userUser = new IdentityUser() { UserName = user.UserName, Email = user.Email, PasswordHash = user.PasswordHash, SecurityStamp = user.SecurityStamp };
                _db.Users.Add(userUser);
                await _userManager.AddToRoleAsync(userUser, "Admin");
                _db.SaveChanges();

                await _userManager.DeleteAsync(user);
            }
            else
            {
                throw new NullReferenceException();
            }

            return RedirectToAction(nameof(Index));
        }
        
        public async Task<ActionResult> RemoveRoles(string id)
        {
            List<string> roles = new List<string>() { "Admin", "StoreOwner"};
            var user = _userManager.Users.Where(u => u.Id == id).FirstOrDefault();
            
            if (user != null)
            {
                var newUser = new IdentityUser() { UserName = user.UserName, Email = user.Email, PasswordHash = user.PasswordHash, SecurityStamp = user.SecurityStamp };
                _db.Users.Add(newUser);

                await _userManager.RemoveFromRolesAsync(newUser, roles);
                _db.SaveChanges();
                await _userManager.DeleteAsync(user);
            }
            else
            {
                throw new NullReferenceException();
            }
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<ActionResult> AddStoreOwnerRole(string id)
        {
            var user = _userManager.Users.Where(u => u.Id == id).FirstOrDefault();
            if (user != null)
            {
                //List<string> roles = new List<string>() { "Admin", "StoreOwner" };
                //await _userManager.RemoveFromRolesAsync(user, roles);

                //var userUser = new IdentityUser()
                //{
                //    UserName = user.UserName,
                //    Email = user.Email,
                //    PasswordHash = user.PasswordHash,
                //    SecurityStamp = user.SecurityStamp,
                //    NormalizedUserName = user.NormalizedUserName,
                //    NormalizedEmail = user.NormalizedEmail
                //};
                //_db.Users.Add(userUser);
                
                await _userManager.AddToRoleAsync(user, "StoreOwner");
                _db.SaveChanges();
                //await _userManager.DeleteAsync(user);
            }
            else
            {
                throw new NullReferenceException();
            }

            return RedirectToAction(nameof(Index));
        }
        
        
    }
}
