using System;
using System.Collections.Generic;
using System.IO;
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
                    return View(ne.Notes.Where(x => x.id == id).FirstOrDefault());
                }
            }
        }

        [HttpPost]
        public ActionResult Edit(int id, Note note)
        {
            using (NoteAppEntities ne = new NoteAppEntities())
            {
                if (Session["email"] == null)
                {
                    return RedirectToAction("Login", "User");
                }
                else
                {
                    ne.Entry(note).State = System.Data.Entity.EntityState.Modified;
                    note.email = Session["email"].ToString();
                    ne.SaveChanges();
                    return RedirectToAction("Dashboard", "Note");
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
                    if (note.image != null)
                    {
                        String imagePath = Request.MapPath(note.image);
                        System.IO.File.Delete(imagePath);
                    }
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

        //Create
        public ActionResult Create()
        {
            if (Session["email"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Create(Note note)
        {
            if (Session["email"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            else
            {
                using (NoteAppEntities ne = new NoteAppEntities())
                {
                    note.email = Session["email"].ToString();

                    if (note.ImageFile != null)
                    {
                        String fileName = Path.GetFileNameWithoutExtension(note.ImageFile.FileName);
                        String fileExt = Path.GetExtension(note.ImageFile.FileName);
                        fileName = fileName + DateTime.Now.ToString("ddmmyyyyhhmmss") + fileExt;
                        note.image = "~/Images/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/Images"), fileName);
                        note.ImageFile.SaveAs(fileName);
                    }

                    ne.Notes.Add(note);
                    ne.SaveChanges();
                }
                return RedirectToAction("Dashboard");
            }       
            
        }
        
    }
}