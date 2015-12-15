using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Valentines2014.Annotations;

namespace Valentines2015
{
    /// <summary>
    /// Interaction logic for OpenWindow.xaml
    /// </summary>
    public partial class OpenWindow : Window, INotifyPropertyChanged

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

        private List<MusicBoxScript> scriptList;

        public OpenWindow()
        {
            InitializeComponent();
            this.DataContext = this; 
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
            List<string> failingFiles = new List<string>();
            foreach (var file in txtFiles)
            {
                using (StreamReader rs = new StreamReader(file))
                {
                    string header = rs.ReadLine(); //Line 1, header
                    if (header.Substring(0, 10) != "#MusicBox:")
                    {
                        failingFiles.Add(file);
                        continue;
                    }
                    string name = header.Substring(10);
                    scriptList.Add(new MusicBoxScript { Name = name, Path = file });
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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
