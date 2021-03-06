﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gaming.Controllers
{
    public class JeuxController : Controller
    {
        public ActionResult Sort(String sortBy, String Nom = "")
        {
            if (Session["Jeu_SortBy"] == null)
            {
                Session["Jeu_SortBy"] = sortBy;
                Session["Jeu_SortOrder"] = "ASC";
            }
            else
            {
                if ((String)Session["Jeu_SortBy"] == sortBy)
                {
                    if ((String)Session["Jeu_sortOrder"] == "ASC")
                        Session["Jeu_SortOrder"] = "DESC";
                    else
                        Session["Jeu_SortOrder"] = "ASC";
                }
                else
                {
                    Session["Jeu_SortBy"] = sortBy;
                    Session["Jeu_SortOrder"] = "ASC";
                }
            }

			if (Nom != "")
			{
				return RedirectToAction("Lister", "Jeux", new { Nom });
			}
			else
			{
				return RedirectToAction("Lister", "Jeux");
			}

			
        }
        public ActionResult Lister()
        {
			Jeux jeux = new Jeux(Session["DB_REPO"]);

			if (Request["nom"] != null)
			{
				Jeux_Par_Personne jeuxParPersonnes = new Jeux_Par_Personne(Request["nom"], Session["DB_REPO"]);

				String orderBy = "";
				if (Session["Jeu_SortBy"] != null)
					orderBy = (String)Session["Jeu_SortBy"] + " " + (String)Session["Jeu_SortOrder"];

				jeuxParPersonnes.SelectAll(orderBy);
				return View(jeuxParPersonnes.ToList());
			}
			else
			{
				String orderBy = "";
				if (Session["Jeu_SortBy"] != null)
					orderBy = (String)Session["Jeu_SortBy"] + " " + (String)Session["Jeu_SortOrder"];

				jeux.SelectAll(orderBy);
				return View(jeux.ToList());
			}
        }
        public ActionResult Ajouter()
        {
            return View(new Jeu());
        }

        [HttpPost]
        public ActionResult Ajouter(Gaming.Jeu jeu)
        {
            if (ModelState.IsValid)
            {
                Jeux jeux = new Jeux(Session["DB_REPO"]);

                String sql = "SELECT * from jeux where Nom='" + jeu.Nom.ToString() + "'";
                jeux.QuerySQL(sql);
                // Le même nom
                if (jeux.Next())
                {
                    ViewBag.Erreur = "Un jeu possède déja ce nom";
                    return View(jeu);
                }                           

                // Si tout est valide
                jeux.jeu = jeu;
                jeux.jeu.UpLoadImage(Request);
                jeux.Insert();
                return RedirectToAction("Lister", "Jeux");
            }
            return View(jeu);
        }

        public ActionResult Details(String Id)
        {
            Jeux jeux = new Jeux(Session["DB_REPO"]);
            if (jeux.SelectByID(Id))
                return View(jeux.jeu);
            else
                return RedirectToAction("Lister", "Jeux");
        }

        public ActionResult Editer(String Id)
        {
            Jeux jeux = new Jeux(Session["DB_REPO"]);
            if (jeux.SelectByID(Id))
                return View(jeux.jeu);
            else
                return RedirectToAction("Lister", "Jeux");
        }
        [HttpPost]
        public ActionResult Editer(Gaming.Jeu jeu)
        {
            Jeux jeux = new Jeux(Session["DB_REPO"]);
            if (ModelState.IsValid)
            {
                if (jeux.SelectByID(jeu.Id))
                {
                    //Le même nom
                    String sql = "SELECT * from jeux where Nom='" + jeu.Nom.ToString() + "'";
                    jeux.QuerySQL(sql);
                    if (jeux.Next())
                    {
                        // Vérifier si c'est pas moi-même
                        jeux.SelectByID(jeu.Id);
                        if (jeux.jeu.Nom != jeu.Nom)
                        {
                            ViewBag.Erreur = "Un jeu possède déja ce nom";
                            return View(jeu);
                        }
                    }
                    try
                    {                  
                        jeux.jeu = jeu;
                        jeux.jeu.UpLoadImage(Request);
                        jeux.Update();
                        return RedirectToAction("Lister", "Jeux");
                    }
                    catch
                    {
                        ViewBag.Erreur = "Une erreur lors de l'update! Avez-vous une image?";
                    }
                }
            }
            return View(jeu);
        }

        public ActionResult Effacer(String Id)
        {
            Jeux jeux = new Jeux(Session["DB_REPO"]);
            jeux.DeleteRecordByID(Id);
            return RedirectToAction("Lister", "Jeux");
        }
	}
}