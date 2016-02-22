using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orias
{
    /// <summary>
    /// Ligne de commande et valeurs par défaut pour les catégorie d'inscription.
    /// </summary>
    public static class Constantes
    {
        /// <summary>
        /// 1er Parametres de la ligne de commande, il définit la base à utiliser.
        /// </summary>
        public const string CONST_ARGCMDLINE_DBPROD = "db_prod";
        public const string CONST_ARGCMDLINE_DBPREPROD = "db_preprod";
        public const string CONST_ARGCMDLINE_DBRECETTE = "db_recette";

        /// <summary>
        /// Ajout de la base KLESIA
        /// </summary>
        public const string CONST_ARGCMDLINE_KLESIA = "db_klesia";

        /// <summary>
        /// 2nd parametre de la ligne de commande, il indiquer l'opération à réaliser
        /// </summary>


        /// <summary>
        /// L'application met à jour les catégories d'intermédiation pour tous les codes ORIAS de Jazz.
        /// A lancer 1 fois par jour.
        /// </summary>
        public const string CONST_ARGCMDLINE_MAJ_COMPLETE = "orias_full_update";
        /// <summary>
        /// L'application met à jour les catégories d'intermédiation pour tous les nouveaux codes ORIAS de Jazz.
        /// A lancer toutes les 20 minutes.
        /// </summary>
        public const string CONST_ARGCMDLINE_AJOUT_NOUVEAU = "orias_new_only";

        /// <summary>
        /// Aucune opération lancée.
        /// </summary>
        public const string CONST_ARGCMDLINE_MANUEL = "orias_manuel";

        /// <summary>
        /// Valeur par défaut si le courtier n'est pas inscript (ou radié, ou introuvabel...)
        /// </summary>
        public const string CONST_ORIAS_INSCRIPTION_ENCOURS = "Inscription à l’Orias en cours.";

        /// <summary>
        /// Catéogies ORIAS "standards"
        /// </summary>
        public const string CONST_ORIAS_INSCRIPTION_COA = "Courtier d'assurance ou de réassurance (COA)";
        public const string CONST_ORIAS_INSCRIPTION_AGA = "Agent général d'assurance (AGA)";
        public const string CONST_ORIAS_INSCRIPTION_MA = "Mandataire d'assurance (MA)";
        public const string CONST_ORIAS_INSCRIPTION_MAL = "Mandataire d’Assurance Lié (MAL)";
        public const string CONST_ORIAS_INSCRIPTION_MIA = "Mandataire d’Intermédiaire en Assurance (MIA)";
        public const string CONST_ORIAS_INSCRIPTION_NONINSCRIT = "Non inscrit à l'Orias en tant que COA, AGA, MA, MAL ou MIA.";

    }
}
