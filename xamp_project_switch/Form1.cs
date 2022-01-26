using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xamp_project_switch
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        file_read_write IO = new file_read_write();
        string xamp_folder = "";
        int current_project_id = -1;
        int next_project_id = 1;
        Dictionary<int, KeyValuePair<string, string>> projects = new Dictionary<int, KeyValuePair<string, string>>();

        protected void save()
        {
            IO.set_filename("xamp_projects.cfg");
            Dictionary<string, string> savefile = new Dictionary<string, string>();
            savefile.Add("current_project_id", current_project_id.ToString());
            savefile.Add("get_next_project_id", next_project_id.ToString());
            savefile.Add("xamp_folder", xamp_folder);
            IO.save_mapped_file(savefile);
            IO.set_filename("xamp_projects.list");
            savefile = new Dictionary<string, string>();
            foreach (KeyValuePair<int, KeyValuePair<string, string>> project in projects)
            {
                savefile.Add(project.Key.ToString(), "" + project.Value.Key + "§" + project.Value.Value);
            }
            IO.save_mapped_file(savefile);
        }
        public Form1()
        {
            InitializeComponent();
            IO.set_filename("xamp_projects.cfg");
            Dictionary<string, string> cfg = IO.read_mapped_file();

            if (cfg.ContainsKey("current_project_id") == true)
            {
                int pid = -1;
                if (int.TryParse(cfg["current_project_id"], out pid) == true)
                {
                    if (cfg.ContainsKey("get_next_project_id") == true)
                    {
                        int get_next_project_id = -1;
                        if (int.TryParse(cfg["get_next_project_id"], out get_next_project_id) == true)
                        {
                            if (cfg.ContainsKey("xamp_folder") == true)
                            {
                                xamp_folder = cfg["xamp_folder"];
                                button3.BackgroundImage = xamp_project_switch.Properties.Resources.xampp_ready;
                                current_project_id = pid;
                                next_project_id = get_next_project_id;
                            }
                        }
                    }
                }
            }
            
            pictureBox1.Visible = false;
            if (System.IO.File.Exists("" + xamp_folder + "/xampp-control.exe") == true)
            {
                IO.set_filename("xamp_projects.list");
                disable_check = true;
                foreach (KeyValuePair<string, string> X in IO.read_mapped_file())
                {
                    int pid = 0;
                    if (int.TryParse(X.Key, out pid) == true)
                    {
                        if (pid > 0)
                        {
                            bool ischecked = false;
                            if (pid == current_project_id) ischecked = true;
                            string[] cfg_local = X.Value.Split('§');
                            if (cfg_local.Length == 2)
                            {
                                checkedListBox1.Items.Add("" + pid.ToString() + " |#| " + cfg_local[0], ischecked);
                                KeyValuePair<string, string> V = new KeyValuePair<string, string>(cfg_local[0], cfg_local[1]);
                                projects.Add(pid, V);
                            }
                        }
                    }
                }
                pictureBox1.Visible = false;
                checkbox_lock_relock.Start();
            }
            else
            {
                //checkedListBox1.Enabled = false;
                pictureBox1.Visible = true;
                xamp_folder = "";
            }

        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void xamp_select_location(object sender, EventArgs e)
        {
            using (var fbd = new OpenFileDialog())
            {
                fbd.FileName = "xampp-control";
                fbd.DefaultExt = "exe";
                fbd.Filter = "Exe Files (.exe)|*.exe";
                fbd.InitialDirectory = "C:/xampp";
                DialogResult result = fbd.ShowDialog();
                pictureBox1.Visible = true;
                button3.BackgroundImage = xamp_project_switch.Properties.Resources.xampp_fucked;
                xamp_folder = "";
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.FileName))
                {
                    if (fbd.FileName.EndsWith("xampp-control.exe") == true)
                    {
                        xamp_folder = fbd.FileName.Replace("xampp-control.exe", "");
                        if (xamp_folder != "")
                        {
                            pictureBox1.Visible = false;
                            button3.BackgroundImage = xamp_project_switch.Properties.Resources.xampp_ready;
                        }
                        save();
                    }
                }
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (xamp_folder != "")
            {
                using (var fbd = new OpenFileDialog())
                {
                    fbd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    DialogResult result = fbd.ShowDialog();
                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.FileName))
                    {
                        string[] bits = fbd.FileName.Split(new string[] { "\\" }, StringSplitOptions.None);
                        List<string> abits = bits.ToList();
                        abits.RemoveAt(bits.Length - 1);
                        string Atag = String.Join("\\", abits);
                        projects.Add(next_project_id, new KeyValuePair<string, string>(Atag.Split(new string[] { "\\" }, StringSplitOptions.None).Last(), Atag));
                        checkedListBox1.Items.Add("" + next_project_id.ToString() + " |#| " + projects[next_project_id].Key, false);
                        next_project_id++;
                        save();
                    }
                }
            }
        }

        private void linknow()
        {
            int target_project_id = -1;
            string[] bits = checkedListBox1.Items[checkedListBox1.SelectedIndex].ToString().Split(new string[] { " |#| " }, StringSplitOptions.None);
            if (int.TryParse(bits[0], out target_project_id) == true)
            {
                var process = new Process();
                var psi = new ProcessStartInfo();
                psi.FileName = "cmd.exe";
                psi.RedirectStandardInput = true;
                psi.RedirectStandardOutput = false;
                psi.RedirectStandardError = false;
                psi.UseShellExecute = false;
                if (System.Environment.OSVersion.Version.Major >= 6)
                {
                    process.StartInfo.Verb = "runas";
                }
                psi.WorkingDirectory = xamp_folder + "\\";
                process.StartInfo = psi;
                process.Start();
                using (System.IO.StreamWriter sw = process.StandardInput)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        sw.WriteLine("rmdir htdocs");
                        sw.WriteLine("mklink /d \"htdocs\" \"" + projects[target_project_id].Value + "\"");
                    }
                }
                disable_check = true;
                foreach (int A in checkedListBox1.CheckedIndices)
                {
                    update_index = A;
                    checkedListBox1.SetItemChecked(A, false);
                }
                checkbox_lock_relock.Start();
                checkedListBox1.SetItemChecked(checkedListBox1.SelectedIndex, true);
                current_project_id = target_project_id;
                save();
                checkedListBox1.ClearSelected();
            }
        }


        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (xamp_folder != "")
            {
                if (checkedListBox1.Items.Count > 0)
                {
                    if (checkedListBox1.SelectedIndex > -1)
                    {
                        linknow();
                    }
                }
            }
        }
        bool disable_check = false;
        int update_index = -1;
        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (disable_check == false)
            {
                if (e.Index != update_index)
                {
                    e.NewValue = e.CurrentValue;
                }
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.SelectedIndex > -1)
            {
                int target_project_id = -1;
                string[] bits = checkedListBox1.Items[checkedListBox1.SelectedIndex].ToString().Split(new string[] { " |#| " }, StringSplitOptions.None);
                if (int.TryParse(bits[0], out target_project_id) == true)
                {

                    rename TextDialog = new rename(bits[1]);

                    // Show testDialog as a modal dialog and determine if DialogResult = OK.
                    if (TextDialog.ShowDialog(this) == DialogResult.OK)
                    {

                        projects[target_project_id] = new KeyValuePair<string, string>(TextDialog.read_value, projects[target_project_id].Value);
                        checkedListBox1.Items[checkedListBox1.SelectedIndex] = "" + target_project_id.ToString() + " |#| " + projects[target_project_id].Key;
                        save();
                    }
                    TextDialog.Dispose();
                }
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (xamp_folder == "")
            {
                addToolStripMenuItem.Visible = false;
                toolStripSeparator2.Visible = false;
                removeToolStripMenuItem.Visible = false;
                renameToolStripMenuItem1.Visible = false;
                selectToolStripMenuItem1.Visible = false;
                openFolderToolStripMenuItem.Visible = false;
            }
            else
            {
                addToolStripMenuItem.Visible = true;
                toolStripSeparator2.Visible = true;
                removeToolStripMenuItem.Visible = false;
                renameToolStripMenuItem1.Visible = true;
                selectToolStripMenuItem1.Visible = true;
                openFolderToolStripMenuItem.Visible = false;

                if (checkedListBox1.Items.Count == 0)
                {
                    selectToolStripMenuItem1.Visible = false;
                    renameToolStripMenuItem1.Visible = false;
                    toolStripSeparator2.Visible = false;
                }
                else
                {
                    int target_project_id = -1;
                    if (checkedListBox1.SelectedIndex != -1)
                    {
                        string[] bits = checkedListBox1.Items[checkedListBox1.SelectedIndex].ToString().Split(new string[] { " |#| " }, StringSplitOptions.None);
                        if (int.TryParse(bits[0], out target_project_id) == true)
                        {
                            if (target_project_id == current_project_id)
                            {
                                openFolderToolStripMenuItem.Visible = true;
                                toolStripSeparator2.Visible = false;
                                removeToolStripMenuItem.Visible = false;
                                renameToolStripMenuItem1.Visible = false;
                                selectToolStripMenuItem1.Visible = false;
                            }
                            else
                            {
                                removeToolStripMenuItem.Visible = true;
                            }
                        }
                        else e.Cancel = true;
                    }
                    else
                    {
                        if(current_project_id > -1) openFolderToolStripMenuItem.Visible = true;
                        toolStripSeparator2.Visible = false;
                        removeToolStripMenuItem.Visible = false;
                        renameToolStripMenuItem1.Visible = false;
                        selectToolStripMenuItem1.Visible = false;
                    }
                }
            }
        }

        private void checkbox_lock_relock_Tick(object sender, EventArgs e)
        {
            disable_check = false;
            update_index = -1;
            checkbox_lock_relock.Stop();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void removeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            int target_project_id = -1;
            if (checkedListBox1.SelectedIndex != -1)
            {
                string[] bits = checkedListBox1.Items[checkedListBox1.SelectedIndex].ToString().Split(new string[] { " |#| " }, StringSplitOptions.None);
                if (int.TryParse(bits[0], out target_project_id) == true)
                {
                    if (target_project_id == current_project_id)
                    {
                        MessageBox.Show("This should have been hidden im sorry you are seeing this now");
                    }
                    else
                    {
                        projects.Remove(target_project_id);
                        checkedListBox1.Items.RemoveAt(checkedListBox1.SelectedIndex);
                        save();
                    }
                }
            }
        }

        private void checkedListBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.checkedListBox1.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                if (xamp_folder != "")
                {
                    if (checkedListBox1.Items.Count > 0)
                    {
                        checkedListBox1.SetSelected(index, true);
                        linknow();
                    }
                }
            }
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(current_project_id != -1)
            {
                Process.Start(@""+projects[current_project_id].Value+"");
            }
        }
    }
}
