using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

namespace FindDuplicateFiles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> MyFolders { get; set; }
            = new ObservableCollection<string>();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }



        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                SearchPathsTextBox.Text = openFileDlg.FileName;

            }
        }
        private void SelectFolderButton2_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg2 = new Microsoft.Win32.OpenFileDialog()
            {
                Multiselect = true
            };
            Nullable<bool> result = openFileDlg2.ShowDialog();
            if (result == true)
            {
                SearchPathsTextBox2.Text = string.Join("\n",openFileDlg2.FileNames);
                //SearchPathsTextBox2.Text = openFileDlg2.FileNames;
                foreach ( var name in openFileDlg2.FileNames)
                {
                    Debug.WriteLine(name);    
                    MyFolders.Add(name);
                }
                
            }
        }
        private void SelectFolderButton3_Click(object sender, RoutedEventArgs e)
        {

            var selectedFiles = FolderListbox.SelectedItems.Cast<string>().ToList();
            Debug.WriteLine("Index {0}", FolderListbox.SelectedItems.Count);
            //Debug.WriteLine(selectedFiles);

            if (selectedFiles == null)
            {
                foreach (var item in selectedFiles)
                {
                    MyFolders.Remove(item);
                    Debug.WriteLine("Removed {0}", item);
                }
            }
            else
            {
                Debug.WriteLine("Selected Files are empty!");
            }

            
        }
    }
}
