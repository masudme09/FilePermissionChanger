using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace FilePermissionChanger
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();

        }
      
        
        private void button1_Click(object sender, EventArgs e)//Add user button
        {
            string dirName1= createFolder(textBox1.Text);
            string userName = System.Configuration.ConfigurationManager.AppSettings.Get("userTest");
            RemoveFileSecurity(dirName1, userName, FileSystemRights.FullControl, AccessControlType.Allow);
        }

        private string createFolder( string dirName)//create folder
        {
            // If directory does not exist, create it. 
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
            string folderName = Environment.UserName;
            string folderDir = @dirName + "\\" + folderName;
            if (!Directory.Exists(folderDir))
            {
                Directory.CreateDirectory(folderDir);
            }
            return folderDir;
        }


        // Removes an ACL entry on the specified file for the specified account.
        public static void RemoveFileSecurity(string fileName, string account,
            FileSystemRights rights, AccessControlType controlType)
        {
            List<AccessRule> modifiedRulesCol = new List<AccessRule>();
            // Get a FileSecurity object that represents the
            // current security settings.
            FileSecurity fSecurity = File.GetAccessControl(fileName);

            //Get all rules collection including inherited ones
            AuthorizationRuleCollection ruleCol = fSecurity.GetAccessRules(true, true, typeof(NTAccount));

            try
            {
                //Removing inherited rules
                fSecurity.SetAccessRuleProtection(true, false);


                //Creating a list of rules except that need to be removed
                foreach (AccessRule a in ruleCol)
                {
                    if (string.Compare(a.IdentityReference.ToString(), account, true) != 0)
                    {
                        modifiedRulesCol.Add(a);
                    }
                }

                // Remove the FileSystemAccessRule from the security settings.
                fSecurity.RemoveAccessRule(new FileSystemAccessRule(account,
                    rights, controlType));

                foreach (AccessRule a in modifiedRulesCol)
                {
                    fSecurity.AddAccessRule(new FileSystemAccessRule(a.IdentityReference,
                    FileSystemRights.FullControl, a.AccessControlType));
                }

                fSecurity.AddAccessRule(new FileSystemAccessRule(Environment.UserName,
               FileSystemRights.FullControl, AccessControlType.Allow));

                // Set the new access settings.
                File.SetAccessControl(fileName, fSecurity);
            }

            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }

        // Adds an ACL entry on the specified file for the specified account.
        public static void AddFileSecurity(string fileName, string account,
            FileSystemRights rights, AccessControlType controlType)
        {


            // Get a FileSecurity object that represents the
            // current security settings.
            FileSecurity fSecurity = File.GetAccessControl(fileName);

            // Add the FileSystemAccessRule to the security settings.
            fSecurity.AddAccessRule(new FileSystemAccessRule(account,
                rights, controlType));

            // Set the new access settings.
            File.SetAccessControl(fileName, fSecurity);

        }

    }
}
