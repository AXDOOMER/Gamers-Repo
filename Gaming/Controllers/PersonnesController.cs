using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gaming.Controllers
{
    public class PersonnesController : Controller
    {
        public ActionResult Sort(String sortBy, String Nom = "")
        {
            if (Session["Personne_SortBy"] == null)
            {
                Session["Personne_SortBy"] = sortBy;
                Session["Personne_SortOrder"] = "ASC";
            }
            else
            {
                if ((String)Session["Personne_SortBy"] == sortBy)
                {
                    if ((String)Session["Personne_sortOrder"] == "ASC")
                        Session["Personne_SortOrder"] = "DESC";
                    else
                        Session["Personne_SortOrder"] = "ASC";
                }
                else
                {
                    Session["Personne_SortBy"] = sortBy;
                    Session["Personne_SortOrder"] = "ASC";
                }
            }

			if (Nom != "")
			{
				return RedirectToAction("Lister", "Personnes", new { Nom });
			}
			else
			{
				return RedirectToAction("Lister", "Personnes");
			}
        }
        public ActionResult Lister()
        {
			Personnes personnes = new Personnes(Session["DB_REPO"]);

			if (Request["nom"] != null)
			{
				Personnes_Par_Jeu personnesParjeu = new Personnes_Par_Jeu(Request["nom"], Session["DB_REPO"]);

				String orderBy = "";
				if (Session["Personne_SortBy"] != null)
					orderBy = (String)Session["Personne_SortBy"] + " " + (String)Session["Personne_SortOrder"];

				personnesParjeu.SelectAll(orderBy);
				return View(personnesParjeu.ToList());
			}
			else
			{
				String orderBy = "";
				if (Session["Personne_SortBy"] != null)
					orderBy = (String)Session["Personne_SortBy"] + " " + (String)Session["Personne_SortOrder"];

				personnes.SelectAll(orderBy);
				return View(personnes.ToList());
			}
        }
        public ActionResult Ajouter()
        {
            return View(new Personne());
        }

        [HttpPost]
        public ActionResult Ajouter(Gaming.Personne personne)
        {
            if (ModelState.IsValid)
            {
                Personnes personnes = new Personnes(Session["DB_REPO"]);
                personnes.personne = personne;
                personnes.personne.UpLoadImage(Request);
                personnes.Insert();
                return RedirectToAction("Lister", "Personnes");
            }
            return View(personne);
        }

        public ActionResult Details(String Id)
        {
            Personnes personnes = new Personnes(Session["DB_REPO"]);
            if (personnes.SelectByID(Id))
                return View(personnes.personne);
            else
                return RedirectToAction("Lister", "Personnes");
        }

        public ActionResult Editer(String Id)
        {
            Personnes personnes = new Personnes(Session["DB_REPO"]);
            if (personnes.SelectByID(Id))
                return View(personnes.personne);
            else
                return RedirectToAction("Lister", "Personnes");
        }
        [HttpPost]
        public ActionResult Editer(Gaming.Personne personne)
        {
            Personnes personnes = new Personnes(Session["DB_REPO"]);
            if (ModelState.IsValid)
            {
                if (personnes.SelectByID(personne.Id))
                {
                    personnes.personne = personne;
                    personnes.personne.UpLoadImage(Request);
                    personnes.Update();
                    return RedirectToAction("Lister", "Personnes");
                }
            }
            return View(personne);
        }

        public ActionResult Effacer(String Id)
        {
            Personnes personnes = new Personnes(Session["DB_REPO"]);
            personnes.DeleteRecordByID(Id);
            return RedirectToAction("Lister", "Personnes");
        }
	}
}