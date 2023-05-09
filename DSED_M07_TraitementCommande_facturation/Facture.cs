using DSED_M07_Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSED_M07_TraitementCommande_facturation
{
    public class Facture
    {
        public Guid ReferenceCommande { get; set; }
        public List<Article> Articles { get; set; }
        public double SousTotal { get; set; }
        public double Rabais { get; set; }
        public double MontantTaxes { get; set; }
        public double Total { get; set; }


        public Facture()
        {
            ;
        }

        public Facture(Commande p_commande, double p_pourcentRabais)
        {
            this.ReferenceCommande = p_commande.Reference;
            this.Articles = p_commande.Articles;
            this.Rabais = p_pourcentRabais;
            this.SousTotal = p_commande.SousTotal() - (p_commande.SousTotal() * p_pourcentRabais / 100);
            this.MontantTaxes = this.SousTotal * 14.975 / 100;
            this.Total = this.SousTotal + this.MontantTaxes;
        }
    }
}
