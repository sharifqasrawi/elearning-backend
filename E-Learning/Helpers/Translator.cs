using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Helpers
{

    public interface ITranslator
    {
        string GetTranslation(string key, string lang);
    }
    public class Translator : ITranslator
    {

        Dictionary<string, Dictionary<string, string>> translations = new Dictionary<string, Dictionary<string, string>>
        {
            {
                "en",
                new Dictionary<string, string>
                {
                    { "ACCOUNT.INVALID_USERNAME_PASSWORD", "Invalid username or password"},
                    { "ACCOUNT.EMAIL_NOT_CONFIRMED", "Email not confirmed" },
                    { "ACCOUNT.DEACTIVATED", "Your account is deactivated. Please contact website administrator" },
                    { "ACCOUNT.REGISTER_EMAIL_SUBJECT", "Confirm your email address" },
                    { "ACCOUNT.REGISTER_EMAIL_MESSAGE", "Please click on the link below to confirm your email address" },
                    { "ACCOUNT.RESET_PASSWORD_EMAIL_SUBJECT", "Reset your password" },
                    { "ACCOUNT.RESET_PASSWORD_EMAIL_MESSAGE", "Please click on the link below to reset your password" },
                    { "ACCOUNT.CANNOT_RESET_PASSWORD", "Cannot reset password. please confirm your email address"},
                    { "ACCOUNT.CURRENT_PASSWORD_INCORRECT", "Current password is incorrect" },
                    { "ACCOUNT.USER_NOT_FOUND", "User cannot be found" },

                    { "CLASSES.COURSE_HAS_CLASS", "This course already has a class" },
                    { "CLASSES.USER_NOT_ENROLLED", "User is not enrolled in this class" },

                    { "DIRECTORIES.DIRECTORY_EXISTS", "Directory already exists" },

                     { "REPORTS.REPLY_EMAIL_SUBJECT", "Report follow up" },

                    { "VALIDATION.EMAIL_REQUIRED", "Email is required" },
                    { "VALIDATION.PASSWORD_REQUIRED", "Password is required" },
                    { "VALIDATION.FIRSTNAME_REQUIRED", "First name is required" },
                    { "VALIDATION.LASTNAME_REQUIRED", "Last name is required" },
                    { "VALIDATION.COUNTRY_REQUIRED", "Country is required" },
                    { "VALIDATION.GENDER_REQUIRED", "Gender is required" },
                    { "VALIDATION.PASSWORDS_MATCH", "Passwords do not match" },
                    { "VALIDATION.CATEGORY_TITLE_REQUIRED", "Category title is required" },
                    { "VALIDATION.CLASSNAME_COURSEID_REQUIRED", "Class name and course Id are required to create class" },

                    { "ERROR", "Oops. An error occured" },
                    { "NOT_FOUND", "Not Found" }
                }
            },
            {
                "fr",
                new Dictionary<string, string>
                {
                    { "ACCOUNT.INVALID_USERNAME_PASSWORD", "Nom d'utilisateur ou mot de passe invalide"},
                    { "ACCOUNT.EMAIL_NOT_CONFIRMED", "E-mail non confirmé" },
                    { "ACCOUNT.DEACTIVATED", "Votre compte est désactivé. Veuillez contacter l'administrateur du site" },
                    { "ACCOUNT.REGISTER_EMAIL_SUBJECT", "Confirmez votre adresse email" },
                    { "ACCOUNT.REGISTER_EMAIL_MESSAGE", "Veuillez cliquer sur le lien ci-dessous pour confirmer votre adresse e-mail" },
                    { "ACCOUNT.RESET_PASSWORD_EMAIL_SUBJECT", "Réinitialisez votre mot de passe" },
                    { "ACCOUNT.RESET_PASSWORD_EMAIL_MESSAGE", "Veuillez cliquer sur le lien ci-dessous pour réinitialiser votre mot de passe" },
                    { "ACCOUNT.CANNOT_RESET_PASSWORD", "Impossible de réinitialiser le mot de passe. Merci de confirmer votre adresse e-mail"},
                    { "ACCOUNT.CURRENT_PASSWORD_INCORRECT", "le mot de passe actuel est incorrect" },
                    { "ACCOUNT.USER_NOT_FOUND", "L'utilisateur est introuvable" },

                    { "CLASSES.COURSE_HAS_CLASS", "Ce cours a déjà une classe" },
                    { "CLASSES.USER_NOT_ENROLLED", "L'utilisateur n'est pas inscrit dans cette cours" },

                    { "DIRECTORIES.DIRECTORY_EXISTS", "Le dossier existe déjà" },

                    { "REPORTS.REPLY_EMAIL_SUBJECT", "Suivi du rapport" },

                    { "VALIDATION.EMAIL_REQUIRED", "Email est requis" },
                    { "VALIDATION.PASSWORD_REQUIRED", "Mot de passe est requis" },
                    { "VALIDATION.FIRSTNAME_REQUIRED", "Le prénom est requis" },
                    { "VALIDATION.LASTNAME_REQUIRED", "Le nom est requis" },
                    { "VALIDATION.COUNTRY_REQUIRED", "le pays est requis" },
                    { "VALIDATION.GENDER_REQUIRED", "le sexe est requis" },
                    { "VALIDATION.PASSWORDS_MATCH", "Les mots de passe ne correspondent pas" },
                    { "VALIDATION.CATEGORY_TITLE_REQUIRED", "Titre de catégorie est requis" },
                    { "VALIDATION.CLASSNAME_COURSEID_REQUIRED", "Le nom de la classe et l'ID du cours sont requis pour créer la classe" },

                    { "ERROR", "Oops. Une erreur est survenue" },
                    { "NOT_FOUND", "Introuvable" }
                }
            }
        };


        public string GetTranslation(string key, string lang)
        {
            return translations[lang][key];
        }
    }
}
