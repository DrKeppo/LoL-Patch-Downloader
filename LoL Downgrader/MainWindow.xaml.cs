using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Threading;

namespace LoL_Downgrader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public string[] Labels = new string[] { "GamePath" };
        public string[] Vals = new string[1];
        FolderBrowserDialog fbd = new FolderBrowserDialog();
        public string Path;

        public MainWindow()
        {
            InitializeComponent();
            if (!File.Exists("settings.json"))
            {
                BrowseGame();
                CreateJson();
            }
            LoadJsonData();
            LoadServers();
            Startup();
        }

        public void Startup()
        {
            location_box.Text = Vals[0];
        }

        public void LoadServers()
        {
            string[] servers = new string[] { "EUNE", "EUW", "NA" };

            for (int i = 0; i < servers.Length; i++)
            {
                server_combobox.Items.Add(servers[i]);
            }
        }

        public void LoadVersions()
        {
            string url = "http://l3cdn.riotgames.com/releases/live/projects/lol_game_client/releases/releaselisting_" + server_combobox.SelectedItem.ToString();
            WebClient wc = new WebClient();
            string version = wc.DownloadString(url);
            string[] array = version.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in array)
            {
                version_combobox.Items.Add(item);
            }
        }

        private void server_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadVersions();
            version_combobox.IsEnabled = true;
        }

        private void go_button_Click(object sender, RoutedEventArgs e)
        {
            //string url = "http://l3cdn.riotgames.com/releases/live/projects/lol_game_client/releases/0.0.1.20/files/DATA/Characters/Chogath/Chogath_Dandy_TX_CM.dds.compressed";
            //WebClient wc = new WebClient();
            //wc.DownloadFile(url, "testfile.dds");
            RunRADSKernel();
        }

        private void version_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            go_button.IsEnabled = true;
        }

        public void RunRADSKernel()
        {
            string version = version_combobox.SelectedItem.ToString();
            string command = string.Format(@"/c rads_user_kernel.exe InstallProjectRelease lol_game_client {0} -debug", version);
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe")
            {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                WorkingDirectory = @"C:\Riot Games\League of Legends\RADS\system",
                Arguments = command
            };
            Process proc = new Process() { StartInfo = psi };

            proc.Start();
            proc.WaitForExit();
            proc.Close();
        }

        public void LoadJsonData()
        {
            var jReader = JsonManager.Load("settings.json");
            for (int i = 0; i < Labels.Length; i++)
            {
                if (!jReader.Dictionary.TryGetValue(Labels[i], out Vals[i]))
                    return;
            }
            return;
        }

        public void CreateJson()
        {
            if (!File.Exists("settings.json"))
            {
                JObject jObject = new JObject(
                    new JProperty("GamePath", Path)
                );
                File.Create("settings.json").Close();
                WriteJson();
            }
        }

        public void BrowseGame()
        {
            fbd.Description = @"Please browse Riot Games\League of Legends folder.";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!File.Exists(fbd.SelectedPath + @"\LeagueClient.exe"))
                {
                    System.Windows.MessageBox.Show("This folder does not contain LeagueClient.exe!");
                }
                else {
                    Path = fbd.SelectedPath;
                    WriteJson();
                    location_box.Text = Path;
                    return;
                }
            }
        }

        private void browse_button_Click(object sender, RoutedEventArgs e)
        {
            BrowseGame();
        }

        public void WriteJson()
        {
                JObject jObject = new JObject(
                    new JProperty("GamePath", Path)
                );
          using (StreamWriter sWriter = new StreamWriter("settings.json"))
          using (JsonTextWriter jWriter = new JsonTextWriter(sWriter))
          {
            jObject.WriteTo(jWriter);
          }
        }
    }
}
