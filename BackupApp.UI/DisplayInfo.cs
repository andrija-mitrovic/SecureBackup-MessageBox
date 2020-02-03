using BackupApp.Library;
using BackupApp.Library.Models;
using BackupApp.Library.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BackupApp.UI
{
    public partial class DisplayInfo : Form
    {
        private string _currentDirectory = Application.StartupPath;
        private readonly Dictionary<BackupIconType, string> _backupIcon;

        public DisplayInfo()
        {
            InitializeComponent();

            _backupIcon = new Dictionary<BackupIconType, string>();
            this._backupIcon.Add(BackupIconType.PROCESSING, $"{_currentDirectory}\\BackupBlue.ico");

            this.notifyIcon.Icon = new Icon(_backupIcon[BackupIconType.PROCESSING]);
            this.notifyIcon.Text = "Backup working...";

            this.notifyIcon.Visible = true;
            this.Load += DisplayInfo_Load;
        }

        private void DisplayInfo_Load(object sender, EventArgs e)
        {
            Thread backgroundThread = new Thread(
               new ThreadStart(() =>
               {
                   try
                   {
                       bool status = BackupStatus();

                       string messageContent = "";

                       messageContent = (status == true) ?
                           $"Backup successful - Date: {DateTime.Now}" :
                           $"Backup unsuccessful - Date: {DateTime.Now}";

                       SendNotifications(messageContent);
                   }
                   catch(Exception ex)
                   {
                       MessageBox.Show(ex.Message);
                   }

                   Thread.Sleep(60 * 1000);
               }));

            backgroundThread.Start();
        }

        private bool BackupStatus()
        {
            string filePath = $"{_currentDirectory}\\{GlobalConfig.BackupAppFileInfo}";
            BackupModel backupModel = filePath.LoadFile().ConvertToBackUpModel();

            Backup backup = new Backup(new SharpCompression());
            bool status = backup.Create(_currentDirectory, backupModel);

            return status;
        }

        private void SendNotifications(string messageContent)
        {
            string emailConfigFilePath = $"{_currentDirectory}\\{GlobalConfig.EmailConfig}";
            string notificationFilePath = $"{_currentDirectory}\\{GlobalConfig.BackupAppNotifFileInfo}";

            Notification notification = new Notification();

            if (File.Exists(emailConfigFilePath))
            {
                EmailModel emailModel = emailConfigFilePath.LoadFile().ConvertToEmailModel();
                emailModel.Body = messageContent;
                notification.Add(new Email(emailModel));
            }

            if (File.Exists(emailConfigFilePath))
            {
                notification.Add(new Text(messageContent, notificationFilePath));
            }

            notification.Send();
        }
    }
}
