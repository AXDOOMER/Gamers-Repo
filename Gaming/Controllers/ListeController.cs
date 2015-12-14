﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gaming.Controllers
{
    public class ListeController : Controller
    {
        //
        // GET: /Liste/
        public ActionResult Lister()
        {
            Liste liste = new Liste(Session["DB_REPO"]);

            //String orderBy = "";
            //if (Session["Jeu_SortBy"] != null)
            //    orderBy = (String)Session["Jeu_SortBy"] + " " + (String)Session["Jeu_SortOrder"];

            liste.SelectAll(/*orderBy*/);
            return View(liste.ToList());
        }
        public ActionResult Ajouter()
        {
            return View(new UnLienListe());
        }

        [HttpPost]
        public ActionResult Ajouter(Gaming.UnLienListe unLienListe,string radioJeu, string radioPersonne)
        {
            if (ModelState.IsValid)
            {
                Liste liste = new Liste(Session["DB_REPO"]);
                //liste.unLienListe.IdJeu = Int64.Parse(unLienListe.IdJeu.ToString());
                //liste.unLienListe.IdPersonne = Int64.Parse(unLienListe.IdPersonne.ToString());

                liste.unLienListe.IdJeu = Int64.Parse(radioJeu);
                liste.unLienListe.IdPersonne = Int64.Parse(radioPersonne);

                //liste.liste = liste;
                try
                {
                    liste.Insert();
                }
                catch
                {
                    ViewBag.Erreur = "Une erreur lors de l'insertion";
                    return View(unLienListe);
                }
                
                return RedirectToAction("Lister", "Liste");
            }
            return View(unLienListe);
        }

        public ActionResult Effacer(String Id)
        {
            Liste liste = new Liste(Session["DB_REPO"]);
            liste.DeleteRecordByID(Id);
            return RedirectToAction("Lister", "Liste");
        }

        public ActionResult Editer(String Id)
        {
            Liste liste = new Liste(Session["DB_REPO"]);
            if (liste.SelectByID(Id))
                return View(liste.unLienListe);
            else
                return RedirectToAction("Lister", "Liste");
        }
        [HttpPost]
        public ActionResult Editer(Gaming.UnLienListe unLienListe, string radioJeu, string radioPersonne)
        {
            Liste liste = new Liste(Session["DB_REPO"]);
            if (ModelState.IsValid)
            {
                if (liste.SelectByID(unLienListe.Id))
                {
                    liste.unLienListe.IdJeu = Int64.Parse(radioJeu);
                    liste.unLienListe.IdPersonne = Int64.Parse(radioPersonne);
                    try
                    {
                        liste.Update();
                    }
                    catch
                    {
                        ViewBag.Erreur = "Une erreur lors de l'update";
                        return View(unLienListe);
                    }
                    return RedirectToAction("Lister", "Liste");
                }
            }
            return View(unLienListe);
        }
	}
}