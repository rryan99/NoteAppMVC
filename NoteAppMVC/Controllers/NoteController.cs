using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NoteAppMVC.Controllers
{
    public class NoteController : Controller
    {
        //Dashboard
        public ActionResult Dashboard()
        {
            using (NoteAppEntities ne = new NoteAppEntities())
            {
                if (Session["email"] == null)
                {
                    return RedirectToAction("Login", "User");
                }
                else
                {
                    //var notes = ne.Notes.SqlQuery("SELECT * FROM dbo.notes WHERE email='" + Session["email"] + "'").ToList();
                    var session = Session["email"].ToString();
                    var notes = ne.Notes.Where(x => x.email == session).ToList();
                    return View(notes);
                }
            }
        }

        //View
        public ActionResult ViewNote(int id)
        {
            using (NoteAppEntities ne = new NoteAppEntities())
            {
                Note note = ne.Notes.Find(id);
                if (note.shared == true)
                {
                    ViewBag.noteDetails = ne.Notes.Where(x => x.id == id).ToList();
                    return View(note);
                }
                else
                {
                    if (Session["email"] != null)
                    {
                        if (Session["email"].ToString() == note.email)
                        {
                            ViewBag.noteDetails = ne.Notes.Where(x => x.id == id).ToList();
                            return View(note);
                        }
                        else
                        {
                            return RedirectToAction("Dashboard", "Note");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Login", "User");
                    }
                }
            }
        }

        //Toggle Sharing
        public ActionResult ToggleSharing(int id)
        {
            using (NoteAppEntities ne = new NoteAppEntities())
            {
                Note note = ne.Notes.Find(id);
                if (note.shared == true)
                {
                    note.shared = false;
                    ne.SaveChanges();
                }
                else
                {
                    note.shared = true;
                    ne.SaveChanges();
                }
                return RedirectToAction("ViewNote", "Note", new { id = id });
            }
        }

        //Edit
        public ActionResult Edit(int id)
        {
            using (NoteAppEntities ne = new NoteAppEntities())
            {
                if (Session["email"] == null)
                {
                    return RedirectToAction("Login", "User");
                }
                else
                {
                    Note note = ne.Notes.Find(id);
                    return RedirectToAction("Edit", "Note");
                }
            }
        }

        //Delete
        public ActionResult Delete(int id)
        {
            using (NoteAppEntities ne = new NoteAppEntities())
            {
                if (Session["email"] == null)
                {
                    return RedirectToAction("Login", "User");
                }
                else
                {
                    Note note = ne.Notes.Find(id);
                    ne.Notes.Remove(note);
                    ne.SaveChanges();
                    return RedirectToAction("Dashboard");
                }
            }
        }

        //Search
        public ActionResult Search(String keyword)
        {
            using (NoteAppEntities ne = new NoteAppEntities())
            {
                if (Session["email"] == null)
                {
                    return RedirectToAction("Login", "User");
                }
                else
                {
                    var session = Session["email"].ToString();
                    var notes = ne.Notes.Where(x => x.email == session && x.title.Contains(keyword)).ToList();
                    return View(notes);
                }
            }
        }
    }
}