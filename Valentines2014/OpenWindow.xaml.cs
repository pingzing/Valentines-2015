using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Valentines2015.Annotations;
using Valentines2015.MVVM;

namespace Valentines2015
{
    /// <summary>
    /// Interaction logic for OpenWindow.xaml
    /// </summary>
    public partial class OpenWindow : BindableWindowBase

    {
        public MusicBoxScript ReturnSelection { get; set; }

        private bool _playEnabled = false;
        public bool PlayEnabled 
        {
            get { return _playEnabled; }
            set
            {
                _playEnabled = value;
                this.OnPropertyChanged();
            }
        }

        private ObservableCollection<MusicBoxScript> _scriptList = new ObservableCollection<MusicBoxScript>();
        public ObservableCollection<MusicBoxScript> ScriptList
        {
            get { return _scriptList; }
            set
            {
                if (value != _scriptList)
                {
                    _scriptList = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public OpenWindow()
        {
            InitializeComponent();
            this.DataContext = this; 
            PopulateMusicBoxList();               
        }

        private void PopulateMusicBoxList()
        {            
            string searchDir = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Scripts");
            IEnumerable<string> txtFiles = Directory.EnumerateFiles(searchDir, "*.txt", SearchOption.TopDirectoryOnly);
            AddScriptFilesToListBox(txtFiles);
        }

        private void AddScriptFilesToListBox(IEnumerable<string> txtFiles)
        {
            List<string> failingFiles = new List<string>();
            foreach (var file in txtFiles)
            {
                string newFilePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Scripts", Path.GetFileName(file));
                if (!File.Exists(newFilePath))
                {
                    File.Copy(file, newFilePath, false);
                }
                 using (StreamReader rs = new StreamReader(newFilePath))
                {
                    string header = rs.ReadLine(); //Line 1, header
                    if (header.Substring(0, 10) != "#MusicBox:")
                    {
                        failingFiles.Add(newFilePath);
                        continue;
                    }
                    string name = header.Substring(10);
                    ScriptList.Add(new MusicBoxScript { Name = name, Path = newFilePath });
                }
            }

            if (failingFiles.Count > 0)
            {
                StringBuilder failedString = new StringBuilder();
                foreach (string failed in failingFiles)
                {
                    failedString.AppendLine($"- {failed}");
                }
                MessageBox.Show($"Unable to open the following files:\n{failedString}", "Unable to open files", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog addDialog = new Microsoft.Win32.OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = ".txt",
                Filter = "*Text Files (.txt)|*.txt",
                Multiselect = true,
                Title = "Open Music Box script"
            };
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

        private void MusicBoxListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PlayEnabled = e.AddedItems.Count > 0;            
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
