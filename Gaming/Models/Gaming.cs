using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Gaming
{

    // Classe de référence pour un image
    public class ImageGUIDReference
    {
        public String DefaultImage { get; set; }
        public String BasePath { get; set; }
        public String GUID { get; set; }
        public ImageGUIDReference(String basePath, String defautImage)
        {
            this.BasePath = basePath;
            this.DefaultImage = defautImage;
            GUID = "";
        }

        public String GetURL()
        {
            String url;
            if (String.IsNullOrEmpty(GUID))
                url = BasePath + DefaultImage;
            else
                url = BasePath + GUID + ".png";
            return url;
        }

        public String GetURI()
        {
            String uri;
            if (String.IsNullOrEmpty(GUID))
                uri = "";
            else
                uri = BasePath + GUID + ".png";
            return uri;
        }

        public String GetImageURL(String GUID)
        {
            String url;
            if (String.IsNullOrEmpty(GUID))
                url = BasePath + DefaultImage;
            else
                url = BasePath + GUID + ".png";
            return url;
        }

        public String UpLoadImage(HttpRequestBase Request, String PreviousGUID)
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    GUID = PreviousGUID;
                    if (!String.IsNullOrEmpty(GUID))
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(GetURI()));
                    }
                    GUID = Guid.NewGuid().ToString();
                    file.SaveAs(HttpContext.Current.Server.MapPath(GetURI()));
                    return GUID;
                }
            }
            return PreviousGUID;
        }

        public void Remove(String GUID)
        {
            if (!String.IsNullOrEmpty(GUID))
            {
                this.GUID = GUID;
                System.IO.File.Delete(HttpContext.Current.Server.MapPath(GetURI()));
            }
        }
    }


    // La classe pour les personnes
    public class Personne
    {
        public long Id { get; set; }

        [Display(Name = "Nom")]
        [StringLength(50), Required]
        [RegularExpression(@"^((?!^Name$)[-a-zA-Z0-9 àâäçèêëéìîïòôöùûüÿñÀÂÄÇÈÊËÉÌÎÏÒÔÖÙÛÜ_'])+$", ErrorMessage = "Caractères illégaux.")]
        public String Nom { get; set; }

        [Display(Name = "Prenom")]
        [StringLength(50), Required]
        [RegularExpression(@"^((?!^Name$)[-a-zA-Z0-9 àâäçèêëéìîïòôöùûüÿñÀÂÄÇÈÊËÉÌÎÏÒÔÖÙÛÜ_'])+$", ErrorMessage = "Caractères illégaux.")]
        public String Prenom { get; set; }

        [Display(Name = "Date de naissance")]
        [DataType(DataType.Date)]
        public DateTime DateNaissance { get; set; }

        [Display(Name = "Photo")]
        public String Photo { get; set; }

        private ImageGUIDReference ImageReference;

        public Personne() 
        {
            Nom = "";
            Prenom = "";
            DateNaissance = (DateTime.Now).Date;
            Photo = "";
            ImageReference = new ImageGUIDReference(@"/Images/Photos/", @"Anonymous.png");
        }

        public String GetImageURL()
        {
            return ImageReference.GetImageURL(Photo);
        }

        public void UpLoadImage(HttpRequestBase Request)
        {
            Photo = ImageReference.UpLoadImage(Request, Photo);
        }

        public void RemoveImage()
        {
            ImageReference.Remove(Photo);
        }
    }

    public class Personnes : SqlExpressUtilities.SqlExpressWrapper
    {
        public Personne personne { get; set; }
        public Personnes(object cs)
            : base(cs)
        {
            personne = new Personne();
        }
        public Personnes() { personne = new Personne(); }

        public List<Personne> ToList()
        {
            List<object> list = this.RecordsList();
            List<Gaming.Personne> personnes_list = new List<Personne>();
            foreach (Personne personne in list)
            {
                personnes_list.Add(personne);
            }
            return personnes_list;
        }

        public override void DeleteRecordByID(String ID)
        {
            if (this.SelectByID(ID))
            {
                this.personne.RemoveImage();
                base.DeleteRecordByID(ID);
            }
        }
    }


    // La classe pour les jeux
    public class Jeu
    {
        public long Id { get; set; }

        [Display(Name = "Nom")]
        [StringLength(50), Required]
        public String Nom { get; set; }

        [Display(Name = "Type")]
        [StringLength(50), Required]
        public String Type { get; set; }

        [Display(Name = "Description")]
        [StringLength(200), Required]
        public String Description { get; set; }

        [Display(Name = "Adresse site web")]
        [StringLength(50), Required]
        public String SiteWeb { get; set; }


        [Display(Name = "Photo")]
        public String Photo { get; set; }

        private ImageGUIDReference ImageReference;

        public Jeu()
        {
            Nom = "";
            Type = "";
            Description = "";
            SiteWeb = "";
            Photo = "";
            ImageReference = new ImageGUIDReference(@"/Images/Photos/", @"UnknownGame.png");
        }

        public String GetImageURL()
        {
            return ImageReference.GetImageURL(Photo);
        }

        public void UpLoadImage(HttpRequestBase Request)
        {
            Photo = ImageReference.UpLoadImage(Request, Photo);
        }

        public void RemoveImage()
        {
            ImageReference.Remove(Photo);
        }
    }

    public class Jeux : SqlExpressUtilities.SqlExpressWrapper
    {
        public Jeu jeu { get; set; }
        public Jeux(object cs)
            : base(cs)
        {
            jeu = new Jeu();
        }
        public Jeux() { jeu = new Jeu(); }

        public List<Jeu> ToList()
        {
            List<object> list = this.RecordsList();
            List<Gaming.Jeu> jeux_list = new List<Jeu>();
            foreach (Jeu jeu in list)
            {
                jeux_list.Add(jeu);
            }
            return jeux_list;
        }

        public override void DeleteRecordByID(String ID)
        {
            if (this.SelectByID(ID))
            {
                this.jeu.RemoveImage();
                base.DeleteRecordByID(ID);
            }
        }
    }


    // La classe pour la liste
    public class UnLienListe
    {
        public long Id { get; set; }

        [Display(Name = "IdPersonne")]
        public long IdPersonne { get; set; }

        [Display(Name = "IdJeu")]
        public long IdJeu { get; set; }

        public UnLienListe()
        {
            IdPersonne = 0;
            IdJeu = 0;
        }
    }

    public class Liste : SqlExpressUtilities.SqlExpressWrapper
    {
        public UnLienListe unLienListe { get; set; }
        public Liste(object cs)
            : base(cs)
        {
            unLienListe = new UnLienListe();
        }
        public Liste() { unLienListe = new UnLienListe(); }

        public List<UnLienListe> ToList()
        {
            List<object> list = this.RecordsList();
            List<Gaming.UnLienListe> liste_list = new List<UnLienListe>();
            foreach (UnLienListe unLienListe in list)
            {
                liste_list.Add(unLienListe);
            }
            return liste_list;
        }

        public override void DeleteRecordByID(String ID)
        {
            if (this.SelectByID(ID))
            {
                base.DeleteRecordByID(ID);
            }
        }

		public void SpecialSelectAll(string orderBy = "")
		{
			// Pour l'instant, le where se fais avec le nom de la personne. Donc, faut
			// pas deux personnes avec le même nom! 
			string sql = "SELECT " +
							"Personnes.Id, " +
							"Personnes.Nom, " +	
							"Personnes.Prenom, " +
							"Jeux.Nom, " +
							"Jeux.Id, " +
							"liste.Id, " +
							"liste.IdPersonne, " +
							"liste.IdJeu " +
							"FROM Personnes INNER JOIN Liste " +
							"ON Personnes.Id = Liste.IdPersonne INNER JOIN Jeux " +
							"ON Liste.IdJeu = Jeux.Id ";

			if (orderBy != "")
			{
				sql += " ORDER BY " + orderBy;
			}

			QuerySQL(sql);
		}
    }

    public class Jeux_Par_Personne : SqlExpressUtilities.SqlExpressWrapper
    {
        public Jeu jeu { get; set; }

        private String nom_Personne = "";

        public Jeux_Par_Personne(String nom_Personne, object cs)
            : base(cs)
        {
            this.nom_Personne = nom_Personne;
            jeu = new Jeu();
        }

        public List<Jeu> ToList()
        {
            List<object> list = this.RecordsList();
            List<Gaming.Jeu> Jeu_list = new List<Jeu>();
            foreach (Jeu jeu in list)
            {
                Jeu_list.Add(jeu);
            }
            return Jeu_list;
        }

        public Jeux_Par_Personne() { jeu = new Jeu(); }

        public override void SelectAll(string orderBy = "")
        {
            // Pour l'instant, le where se fais avec le nom de la personne. Donc, faut
            // pas deux personnes avec le même nom! 
            string sql = "SELECT " +
                            "Jeux.Nom, " +
                            "Jeux.Type, " +
                            "Jeux.Photo, " +
                            "Jeux.Description, " +
                            "Jeux.SiteWeb " +
                            "FROM Personnes INNER JOIN Liste " +
                            "ON Personnes.Id = Liste.IdPersonne INNER JOIN Jeux " +
                            "ON Liste.IdJeu = Jeux.Id " +
                            "WHERE Personnes.Nom = " + SqlExpressUtilities.SQLHelper.ConvertValueFromMemberToSQL(nom_Personne);

            if (orderBy != "")
                sql += " ORDER BY " + orderBy;

            QuerySQL(sql);
        }
    }

    public class Personnes_Par_Jeu : SqlExpressUtilities.SqlExpressWrapper
    {
        public Personne personne { get; set; }

        private String nom_Jeu = "";

        public Personnes_Par_Jeu(String nom_Jeu, object cs)
            : base(cs)
        {
            this.nom_Jeu = nom_Jeu;
            personne = new Personne();
        }

        public List<Personne> ToList()
        {
            List<object> list = this.RecordsList();
            List<Gaming.Personne> Personne_list = new List<Personne>();
            foreach (Personne personne in list)
            {
                Personne_list.Add(personne);
            }
            return Personne_list;
        }

        public Personnes_Par_Jeu() { personne = new Personne(); }

        public override void SelectAll(string orderBy = "")
        {
            // Pour l'instant, le where se fais avec le nom de la personne. Donc, faut
            // pas deux personnes avec le même nom! 
            string sql = "SELECT " +
                            "Personnes.Nom, " +
                            "Personnes.Prenom, " +
                            "Personnes.DateNaissance, " +
                            "Personnes.Photo " +
                            "FROM Personnes INNER JOIN Liste " +
                            "ON Personnes.Id = Liste.IdPersonne INNER JOIN Jeux " +
                            "ON Liste.IdJeu = Jeux.Id " +
                            "WHERE Jeux.Nom = " + SqlExpressUtilities.SQLHelper.ConvertValueFromMemberToSQL(nom_Jeu);

            if (orderBy != "")
                sql += " ORDER BY " + orderBy;

            QuerySQL(sql);
        }
    }

}