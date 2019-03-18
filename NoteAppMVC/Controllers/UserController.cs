using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NoteAppMVC.Controllers
{
    public class UserController : Controller
    {
        //Register
        public ActionResult Register()
        {
            if (Session["email"] != null)
            {
                return RedirectToAction("Dashboard", "Note");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                using (NoteAppEntities ne = new NoteAppEntities())
                {
                    var auth = ne.Users.Where(x => x.email == user.email).Count();
                    if (auth == 1)
                    {
                        ViewBag.Message = "An account is already associated with this email.  Please try again.";
                        return View("Register", user);
                    }
                    else
                    {
                        ne.Users.Add(user);
                        ne.SaveChanges();
                        ModelState.Clear();
                        user = null;
                        ViewBag.Message = "Account created!";
                    }
                }
            }
            return View(user);
        }

        //Login
        public ActionResult Login()
        {
            if (Session["email"] != null)
            {
                return RedirectToAction("Dashboard", "Note");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user)
        {
            using (NoteAppEntities ne = new NoteAppEntities())
            {
                var auth = ne.Users.Where(x => x.email == user.email && x.password == user.password).Count();
                if (auth == 0)
                {
                    ViewBag.Message = "Incorrect email/password.  Please try again.";
                    return View("Login", user);
                }
                else
                {
                    Session["email"] = user.email;
                    return RedirectToAction("Dashboard", "Note");
                }
            }
        }

        //Logout
        public ActionResult Logout()
        {
            Session["email"] = null;
            return RedirectToAction("Login", "User");
        }
    }
}