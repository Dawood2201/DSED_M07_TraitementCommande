using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSED_M07_Entites
{
    public class Article
    {
        public Guid Reference { get; set; }
        public string Nom { get; set; }
        public double Prix { get; set; }
        public int Quantitee { get; set; }

        public static Article GenererArticles()
        {
            Random random = new Random();
            Article article = new Article();
            article.Reference = Guid.NewGuid();
            article.Nom = "Article" + random.Next(1, 1000);
            article.Prix = random.Next(1, 1000);
            article.Quantitee = random.Next(1, 1000);

            return article;
        }
    }
}
