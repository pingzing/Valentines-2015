using System;
using System.Collections.Generic;
using System.IO;
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

namespace Valentines2015
{
    /// <summary>
    /// Interaction logic for OpenWindow.xaml
    /// </summary>
    public partial class OpenWindow : Window
    {
        public MusicBoxScript ReturnSelection { get; set; }

        private List<MusicBoxScript> scriptList;

        public OpenWindow()
        {
            InitializeComponent();            
            PopulateMusicBoxList();
            MusicBoxListBox.ItemsSource = scriptList;
            MusicBoxListBox.Items.Refresh();            
        }

        private void PopulateMusicBoxList()
        {
            scriptList = new List<MusicBoxScript>();
            string searchDir = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Scripts");
            IEnumerable<string> txtFiles = Directory.EnumerateFiles(searchDir, "*.txt", SearchOption.TopDirectoryOnly);
            AddScriptFilesToListBox(txtFiles);
        }

        private void AddScriptFilesToListBox(IEnumerable<string> txtFiles)
        {
            foreach (var file in txtFiles)
            {
                using (StreamReader rs = new StreamReader(file))
                {
                    string header = rs.ReadLine(); //Line 1, header
                    if (header.Substring(0, 10) != "#MusicBox:")
                    {
                        continue;
                    }
                    string name = header.Substring(10);
                    scriptList.Add(new MusicBoxScript { Name = name, Path = file });
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog addDialog = new Microsoft.Win32.OpenFileDialog();
            addDialog.CheckFileExists = true;
            addDialog.CheckPathExists = true;
            addDialog.DefaultExt = ".txt";
            addDialog.Filter = "*Text Files (.txt)|*.txt";
            addDialog.Multiselect = true;
            addDialog.Title = "Open Music Box script";
            if(addDialog.ShowDialog() == true)
            {                
                IEnumerable<string> fileNames = addDialog.FileNames;
                AddScriptFilesToListBox(fileNames);
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            ReturnSelection = (MusicBoxScript)MusicBoxListBox.SelectedItem;
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class MusicBoxScript
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
