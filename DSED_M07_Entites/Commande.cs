using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSED_M07_Entites;

namespace DSED_M07_Entites
{
    public class Commande
    {   
        private static string[] m_noms = new string[10] { "Pierre", "Paul", "Jacques", "Jean", "Marie", "Julie", "Sophie", "Pierre", "Paul", "Jacques" };
        public Guid Reference { get; set; }
        public string NomDuClient { get; set; }
        public List<Article> Articles { get; set; }

        public Commande()
        {
            ;
        } 

        public static Commande GenererCommandeAleatoire()
        {
            Random random = new Random();
            Commande commande = new Commande();
            commande.Reference = Guid.NewGuid();
            commande.NomDuClient = m_noms[random.Next(0, m_noms.Length - 1)];
            commande.Articles = new List<Article>();

            int nombreArticles = random.Next(1, 10);

            for (int i = 0; i < nombreArticles; i++)
            {
                commande.Articles.Add(Article.GenererArticles());
            }

            return commande;
        }
    }
}
