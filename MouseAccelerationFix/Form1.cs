using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MouseAccelerationFix
{
    public partial class Form1 : Form
    {
        const string regPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers";
        const string regValue = "NoDTToDITMouseBatch";

        public Form1()
        {
            InitializeComponent();
            lbPrograms.DisplayMember = "fileName";
            lbPrograms.ValueMember = "filePath";
            getLoadedKeys();
        }

        private void btn_Browse_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Executables (*.exe)|*.exe|All Files (*.*)|*.*";
            fd.InitialDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86);
            fd.Multiselect = false;

            if(fd.ShowDialog() == DialogResult.OK)
            {
                tbPath.Text = fd.FileName;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string programPath = tbPath.Text;

            if (!File.Exists(programPath)) 
            {
                return;
            }

            string programName = Path.GetFileName(programPath);
            basicFile file = new basicFile(programName, programPath);

            foreach(basicFile item in lbPrograms.Items)
            {
                if (item.filePath == file.filePath)
                {
                    return;
                }
            }

            addRegistryEntry(file.filePath);
            lbPrograms.Items.Add(file);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            for(int i = lbPrograms.SelectedItems.Count - 1; i>=0; i--)
            {
                basicFile item = (basicFile)lbPrograms.SelectedItems[i];
                removeRegistryEntry(item.filePath);
                lbPrograms.Items.Remove(item);
            }
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            for (int i = lbPrograms.Items.Count - 1; i >= 0; i--)
            {
                basicFile item = (basicFile)lbPrograms.Items[i];
                removeRegistryEntry(item.filePath);
                lbPrograms.Items.Remove(item);
            }
        }

        private void getLoadedKeys()
        {
            RegistryKey regFolder = Registry.LocalMachine.OpenSubKey(regPath);
            foreach(string key in regFolder.GetValueNames())
            {
                if(regFolder.GetValue(key).ToString() == regValue)
                {
                    string fileName = Path.GetFileName(key);
                    lbPrograms.Items.Add(new basicFile(fileName, key));
                }
            }
        }

        private void addRegistryEntry(string filePath)
        {
            RegistryKey regFolder = Registry.LocalMachine.OpenSubKey(regPath, true);

            if (regFolder.GetValue(filePath) == null)
            {
                regFolder.SetValue(filePath, regValue, RegistryValueKind.String);
            }
        }

        private void removeRegistryEntry(string filePath)
        {
            RegistryKey regFolder = Registry.LocalMachine.OpenSubKey(regPath, true);
            regFolder.DeleteValue(filePath, false);
        }

        public class basicFile
        {
            public string fileName { get; set; }
            public string filePath { get; set; }

            public basicFile(string name, string path)
            {
                this.fileName = name;
                this.filePath = path;
            }
        }

        private void btnCmdAlert_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Please run a command prompt as administrator and run the following command to ensure changes are accepted by Windows:" + Environment.NewLine + "Rundll32 apphelp.dll,ShimFlushCache" + Environment.NewLine + "This has been copied onto your clipboard");
            Clipboard.SetText("Rundll32 apphelp.dll,ShimFlushCache");
        }
    }
}
