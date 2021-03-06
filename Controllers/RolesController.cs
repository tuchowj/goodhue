﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Goodhue.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Goodhue.Controllers;
using System.Net;
using System.Web.Security;

namespace Goodhue.Controllers
{
    [Authorize(Roles = "Super_Admin")]
    public class RolesController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();

        //
        // GET: /Roles/
        public ActionResult Index()
        {
            return RedirectToAction("ManageUserRoles");
        }

        //
        // GET: /Roles/Users
        public ActionResult Users()
        {
            var account = new AccountController();
            return View(account.UserManager.Users.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter(string role)
        {
            if (role == "All")
            {
                return RedirectToAction("Users");
            }
            var account = new AccountController();
            List<ApplicationUser> usersInRole = new List<ApplicationUser>();
            foreach (ApplicationUser user in account.UserManager.Users)
            {
                var accControl = new AccountController(); //won't work without different account controller
                List<string> userRoles = accControl.UserManager.GetRoles(user.Id).ToList();
                if (userRoles.Contains(role)) {
                    usersInRole.Add(user);
                }
            }
            ViewBag.Role = "in Role: " + role;
            return View("Users",usersInRole);
        }

        //
        //GET: /Roles/DeleteUser
        public ActionResult DeleteUser(string id)
        {
            var account = new AccountController();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = account.UserManager.Users.Where(u => u.Id.Equals(id)).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Reservations/DeleteUser/5
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var account = new AccountController();
            ApplicationUser user = account.UserManager.Users.Where(u => u.Id.Equals(id)).FirstOrDefault();
            account.UserManager.Delete(user);
            return RedirectToAction("Users");
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
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
                {
                    Name = collection["RoleName"]
                });
                context.SaveChanges();
                ViewBag.ResultMessage = "Role created successfully";
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Roles/Edit/5
        public ActionResult Edit(string roleName)
        {
            var thisRole = context.Roles.Where(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            return View(thisRole);
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Microsoft.AspNet.Identity.EntityFramework.IdentityRole role)
        {
            try
            {
                context.Entry(role).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Roles/Delete/5
        public ActionResult Delete(string RoleName)
        {
            var thisRole = context.Roles.Where(r => r.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            context.Roles.Remove(thisRole);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ManageUserRoles()
        {
            var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRoleToUser(string Email, string RoleName)
        {
            ApplicationUser user = context.Users.Where(u => u.Email.Equals(Email, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var account = new AccountController();
            if (user == null)
            {

            }
            else
            {
                account.UserManager.AddToRole(user.Id, RoleName);
            }

            ViewBag.ResultMessage = "Role created successfully !";

            // prepopulate roles for the view dropdown
            var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;

            return View("ManageUserRoles");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetRoles(string Email)
        {
            if (!string.IsNullOrWhiteSpace(Email))
            {
                ApplicationUser user = context.Users.Where(u => u.Email.Equals(Email, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                if (user == null)
                {
                    ViewBag.Roles = null;
                    return View("ManageUserRoles");
                }

                var account = new AccountController();
                ViewBag.RolesForThisUser = account.UserManager.GetRoles(user.Id);

                // prepopulate roles for the view dropdown
                var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = list;
            }

            return View("ManageUserRoles");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleFromUser(string Email, string RoleName)
        {
            var account = new AccountController();
            ApplicationUser user = context.Users.Where(u => u.Email.Equals(Email, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            if (user == null)
            {
                ViewBag.Roles = null;
                return View("ManageUserRoles");
            }

            if (Email == User.Identity.Name)
            {
                ViewBag.ResultMessage = "Cannot remove yourself from roles.";
            }
            else if (account.UserManager.IsInRole(user.Id, RoleName)) //successful deletion
            {
                account.UserManager.RemoveFromRole(user.Id, RoleName);
                ViewBag.ResultMessage = "Role removed from this user successfully.";
            }
            else
            {
                ViewBag.ResultMessage = "This user doesn't belong to selected role.";
            }
            // prepopulate roles for the view dropdown
            var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;

            return View("ManageUserRoles");
        }
    }
}