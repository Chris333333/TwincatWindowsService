namespace TwincatWindowsService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TwincatServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.TwincatServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // TwincatServiceProcessInstaller
            // 
            this.TwincatServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.TwincatServiceProcessInstaller.Password = null;
            this.TwincatServiceProcessInstaller.Username = null;
            // 
            // TwincatServiceInstaller
            // 
            this.TwincatServiceInstaller.DelayedAutoStart = true;
            this.TwincatServiceInstaller.Description = "Usługa do zapisu danych z PLC Beckhoff do mySQL. Nie kontroluje niczego.";
            this.TwincatServiceInstaller.DisplayName = "AUT_NatTwincatService";
            this.TwincatServiceInstaller.ServiceName = "NatTwincat";
            this.TwincatServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.TwincatServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstaller1_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.TwincatServiceProcessInstaller,
            this.TwincatServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller TwincatServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller TwincatServiceInstaller;
    }
}