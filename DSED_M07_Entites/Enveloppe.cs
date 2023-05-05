using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSED_M07_Entites
{
    public class Enveloppe
    {
        public TypeEnveloppe Type { get; set; }
        public Commande Commande { get; set; }

        public Enveloppe()
        {
            ;
        }

        public Enveloppe(TypeEnveloppe p_type, Commande p_commande)
        {
            this.Type = p_type;
            this.Commande = p_commande;
        }
    }

    public enum TypeEnveloppe
    {
        normal,
        premium
    }
}
