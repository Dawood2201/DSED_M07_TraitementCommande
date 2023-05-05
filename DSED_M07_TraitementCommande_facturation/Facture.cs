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
        public double SousTotal { get; set; }
        public double MontantTaxes { get; set; }
        public double Total { get; set; }
    }
}
