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

                String sql = "SELECT * from Personnes where Prenom='" + personne.Prenom.ToString() + "'";
                personnes.QuerySQL(sql);
                // Le même prenom
                if(personnes.Next())
                {
                    //Le même nom
                    sql = "SELECT * from Personnes where Nom='" + personne.Nom.ToString() + "'";
                    personnes.QuerySQL(sql);
                    if (personnes.Next())
                    {
                        ViewBag.Erreur = "Un gamer possède déja ce nom et prénom";
                        return View(personne);
                    }
                }

                // Tout est valide
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
                    String sql = "SELECT * from Personnes where Prenom='" + personne.Prenom.ToString() + "'";
                    personnes.QuerySQL(sql);
                    // Le même prenom
                    if (personnes.Next())
                    {
                        //Le même nom
                        sql = "SELECT * from Personnes where Nom='" + personne.Nom.ToString() + "'";
                        personnes.QuerySQL(sql);
                        if (personnes.Next())
                        {
                            // Vérifier si c'est pas moi-même
                            personnes.SelectByID(personne.Id);
                            if (personnes.personne.Prenom != personne.Prenom || 
                                personnes.personne.Nom != personne.Nom)
                            {
                                ViewBag.Erreur = "Un gamer possède déja ce nom et prénom";
                                return View(personne);
                            }                                
                        }
                    }
                    try
                    {
                        personnes.personne = personne;
                        personnes.personne.UpLoadImage(Request);
                        personnes.Update();
                        return RedirectToAction("Lister", "Personnes");
                    }
                    catch
                    {
                        ViewBag.Erreur = "Une erreur lors de l'update! Avez-vous une image?";
                    }

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