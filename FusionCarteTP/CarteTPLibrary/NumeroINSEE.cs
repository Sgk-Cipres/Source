using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CarteTPLibrary
{
    public static class NumeroINSEE
    {
        /// <summary>
        /// Constante pour le calcul de la clé
        /// </summary>
        private const Int16 CLE_VERIF = 97;
        /// <summary>
        /// Nombre de caractère du numéro INSEE
        /// </summary>
        private const Int16 NB_CARACTERES = 13;

        #region "Méthodes publiques"
        /// <summary>
        /// Verifie le numéro INSEE passé en paramètre (numero + clé)
        /// </summary>
        /// <param name="strNumero">Numéro INSEE</param>
        /// <param name="strCle">Clé de verification du numéro INSEE</param>
        /// <returns>True si le numéro et la clé sont cohérents, sinon false</returns>
        public static bool VerifierINSEE(string strNumero, string strCle)
        {
            if (CalculerCleINSEE(strNumero).ToString("D2") == strCle)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Verifie le numéro INSEE passé en paramètre
        /// </summary>
        /// <param name="strNumero">Numéro INSEE avec la clé</param>
        /// <returns>True si le numéro et la clé sont cohérent, sinon false</returns>
        public static bool VerifierINSEE(string strNumero)
        {
            string strCle = "";
            strNumero = NettoyerString(strNumero);

            try
            {
                strCle = strNumero.Remove(0, NB_CARACTERES);
                strNumero = strNumero.Remove(NB_CARACTERES);
            }
            catch
            {
                // il manque des caractères
                return false;
            }

            return VerifierINSEE(strNumero, strCle);
        }

        /// <summary>
        /// Calcul la clé correspondante au numéro INSEE passé en paramètre
        /// </summary>
        /// <param name="strNumero">Numero INSEE</param>
        /// <returns>Clé du numéro INSEE passé en paramètre, 0 si numéro invalide</returns>
        public static Int16 CalculerCleINSEE(string strNumero)
        {
            // clé retournée
            Int16 cle = 0;
            // numéro apres convertion
            Int64 numero = NumeroEnInt(strNumero);

            if (numero != 0)
            {
                //le calcul de la cle n'est autre que le reste de la division euclidienne numero / CLE_VERIF
                //cle = (short)(CLE_VERIF - (numero - (numero / CLE_VERIF) * CLE_VERIF));
                //version par modulo
                cle = (short) (CLE_VERIF - (numero%CLE_VERIF));
            }

            return cle;
        }

        /// <summary>
        /// extrait un numero INSEE à partir d'une chaine
        /// </summary>
        /// <param name="strNumero">chaine à traiter</param>
        /// <returns>numéro INSEE</returns>
        public static string MatchNumeroInsee(string strNumero)
        {
            /*
            #####################################################################################################################
            # Par GV le 21 / 01 / 2014. Dans la REGEX, chaque ligne représente un groupe de règles ci-dessous.                  #
            #####################################################################################################################
            # Position 1 : 1 pour un homme, 2 pour une femme, 3 pour pour les personnes étrangère en cours d'imatriculation,    #
            # 7 et 8 pour les numéros provisoir                                                                                 #
            #####################################################################################################################
            # Position 2 et 3 : Les deux derniers chiffres de l'année de naissance, de 00 à 99                                  #
            #####################################################################################################################
            # Position 4 et 5 : Mois de naissance, de 01 (janvier) à 12 (décembre), de 20 à 30 et de 50 à 99 pour les           #
            # personnes dont la pièce d'état civil ne précise pas le mois de naissance, de 31 à 42 pour celle dont la pièce     #                
            # d'état civile est incomplète mais précise quand même le mois de naissance                                         #
            #####################################################################################################################
            # Position 6 à 10 : Trois cas de figures                                                                            #
            # CAS 1 :                                                                                                           # 
            # Position 6 et 7 : Département de naissance métropolitain, de 01 à 95 (plus 2A ou 2B pour la Corse)                #
            # Dans des cas exceptionnels, il est possible de trouver le numéro 96 qui correspondais à la Tunisie avant 1956.    #
            # Position 8, 9 et 10 : Numéro d'ordre de naissance dans le département, de 001 à 989 ou 990                        #
            # CAS 2 :                                                                                                           #
            # Position 6, 7 et 8 : Département de naissance Outre-mer, de 970 à 989                                             #
            # Position 9 et 10 : Numéro d'odre de naissance dans le département, de 01 à 89, ou 90                              #
            # CAS 3 :                                                                                                           #
            # Position 6 et 7 : Naissance hors de France, une seule valeur : 99                                                 #
            # Position 8, 9 et 10 : Identifiant du pays de naissance, de 001 à 989, ou 990                                      #
            #####################################################################################################################
            # Position 11, 12 et 13 : Numéro d'ordre de l'acte de naissance dans le mois et la commune (ou pays) de 001 à 999   #
            #####################################################################################################################
            # Position 14 et 15 : Clé de contrôle, de 01 à 97 (Non cotrôlé dans ce cas)                                         #
            #####################################################################################################################
            */

            Regex regINSEE = new Regex("/^([1 - 37 - 8])"
             + "([0 - 9]{ 2})"
             + "(0[0 - 9] |[2 - 35 - 9][0 - 9] |[14][0 - 2])"
             + "((0[1 - 9] |[1 - 8][0 - 9] | 9[0 - 69] | 2[abAB])(00[1 - 9] | 0[1 - 9][0 - 9] |[1 - 8][0 - 9]{ 2}| 9[0 - 8][0 - 9] | 990)| (9[78][0 - 9])(0[1 - 9] |[1 - 8][0 - 9] | 90))"
             + "([0 - 9]{ 3})"
             + "?([0 - 8][0 - 9] | 9[0 - 7])/ x");//regex qui ne semble pas match certains cas (à controler plus tard)

            Regex regInseeSimple = new Regex("^[1-3][0-9]{2}(0[1-9]|10|[235-9][0-9]|[14][12])(2[AB]|[0-9]{2})[0-9]{6}$");

            return regInseeSimple.Match(strNumero).Value;
        }

        /// <summary>
        /// Enlève les caractères ne pouvant faire partie du numéro
        /// A-Z0-9 uniquement
        /// </summary>
        /// <param name="strNumero">Numéro INSEE</param>
        /// <returns>Retourne la chaîne épurée</returns>
        public static string NettoyerNumero(string strNumero)
        {
            return NettoyerString(strNumero);
        }
        #endregion

        #region "Méthodes privées"
        /// <summary>
        /// Enlève les caractères ne pouvant faire partie du numéro
        /// A-Z0-9 uniquement
        /// </summary>
        /// <param name="strNumero">Numéro INSEE</param>
        /// <returns>Retourne la chaîne épurée</returns>
        private static string NettoyerString(string strNumero)
        {
            strNumero = strNumero.ToUpper();
            Regex regINSEE = new Regex("[^A-Z0-9_]");
            strNumero = regINSEE.Replace(strNumero, "");

            return strNumero;
        }

        /// <summary>
        /// Convertion du numéro (string) en entier
        /// </summary>
        /// <param name="strNumero">Numéro INSEE</param>
        /// <returns>Retourne le numéro INSEE sous forme d'un entier, 0 si numéro invalide</returns>
        private static Int64 NumeroEnInt(string strNumero)
        {
            // le numero apres convertion
            long numero = 0;

            // Pour les Corses !
            // Emplacement de la lettre pour les corses
            const Int16 INDICE_LETTRE_CORSE = 6;
            // Constante pour calcul Corse 2A
            const Int32 CORSEA = 1000000;
            // Constante pour calcul Corse 2B
            const Int32 CORSEB = 2000000;

            strNumero = NettoyerString(strNumero);

            // le numero doit faire NB_CARACTERES sinon c'est pas
            // la peine d'aller plus loin
            if (strNumero.Length != NB_CARACTERES)
                return numero;

            // convertion en entier, si la chaîne ne peut etre convertie
            // soit une erreur, soit un Corse...
            if (!long.TryParse(strNumero, out numero))
            {
                // verification du 7eme caractère
                if (strNumero[INDICE_LETTRE_CORSE] == 'A')
                {
                    // un Corse du Sud
                    strNumero = strNumero.Replace('A', '0');
                    if (long.TryParse(strNumero, out numero))
                    {
                        numero -= CORSEA;
                    }
                }
                else if (strNumero[INDICE_LETTRE_CORSE] == 'B')
                {
                    // Haute Corse
                    strNumero = strNumero.Replace('B', '0');
                    if (long.TryParse(strNumero, out numero))
                    {
                        numero -= CORSEB;
                    }
                }
            }

            return numero;
        }
        #endregion
    }
}
