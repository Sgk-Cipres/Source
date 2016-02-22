namespace CarteTPService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstallerCarteTP = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstallerCarteTP = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstallerCarteTP
            // 
            this.serviceProcessInstallerCarteTP.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.serviceProcessInstallerCarteTP.Password = null;
            this.serviceProcessInstallerCarteTP.Username = null;
            // 
            // serviceInstallerCarteTP
            // 
            this.serviceInstallerCarteTP.Description = "Service de reception des envois de cartes tiers payants (almerys)";
            this.serviceInstallerCarteTP.ServiceName = "CarteTPService";
            this.serviceInstallerCarteTP.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstallerCarteTP,
            this.serviceInstallerCarteTP});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstallerCarteTP;
        private System.ServiceProcess.ServiceInstaller serviceInstallerCarteTP;
    }
}