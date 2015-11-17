﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Gaming
{

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
        public DateTime DataNaissance { get; set; }

        [Display(Name = "Photo")]
        public String Photo { get; set; }

        private ImageGUIDReference ImageReference;

        public Personne() 
        {
            Nom = "";
            Prenom = "";
            DataNaissance = DateTime.Now;
            Photo = "";
            ImageReference = new ImageGUIDReference(@"/Images/Photos/", @"Anonymous.png");
        }

        public String GetPosterURL()
        {
            return ImageReference.GetImageURL(Photo);
        }

        public void UpLoadPoster(HttpRequestBase Request)
        {
            Photo = ImageReference.UpLoadImage(Request, Photo);
        }

        public void RemovePoster()
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
                this.personne.RemovePoster();
                base.DeleteRecordByID(ID);
            }
        }
    }


}