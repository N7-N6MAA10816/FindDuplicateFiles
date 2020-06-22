using Microsoft.VisualBasic;
using Ookii.Dialogs.Wpf;
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
using System.IO;
using Path = System.IO.Path;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TreeView = System.Windows.Controls.TreeView;
using ListBox = System.Windows.Controls.ListBox;

namespace FindDuplicateFiles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> MyFolders { get; set; }
            = new ObservableCollection<string>();
        //public ObservableCollection<KeyValuePair<string, List<string>>>  BigDic { get; set; } 
        //    = new ObservableCollection<KeyValuePair<string, List<string>>>();
        public SortedDictionary<string,List<string>> BigDic { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
           
        }

        
        private void SelectFolderButton1_Click(object sender, RoutedEventArgs e)
        {

            var ookiiDialog = new VistaFolderBrowserDialog();
            
            if (ookiiDialog.ShowDialog() == true)
            {
                //MessageBox.Show(ookiiDialog.SelectedPath);
                MyFolders.Add(ookiiDialog.SelectedPath);

            }
        }

        private void DeleteFolder_Click(object sender, RoutedEventArgs e)
        {
            if ( FolderListbox.SelectedItems != null)
            {
                Debug.WriteLine("SelectedItems is NOT null!");
                
                var selectedFiles = FolderListbox.SelectedItems.Cast<string>().ToList();

                foreach (var item in selectedFiles)
                {
                    //printCollection(MyFolders);
                    MyFolders.Remove(item);
                    //Debug.WriteLine(item);
                    PrintCollection(MyFolders);
                }
                
            }
            else
            {
                Debug.WriteLine("SelectedItems is null!");
            }    
        }

        private void DeleteAllFolder_Click(object sender, RoutedEventArgs e)
        {
            MyFolders.Clear();
        }

        private static void PrintListbox(ListBox somelist)
        {
            Debug.WriteLine("printList:");
            foreach (var item in somelist.SelectedItems)
            {
                Debug.WriteLine("\t{0}\n",item);
            }
        }

        private static void PrintCollection(ObservableCollection<string> somelist)
        {
            Debug.WriteLine("printCollection:");
            foreach (var item in somelist)
            {
                Debug.WriteLine(item);
            }
        }

        private void File_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> searchPath = new List<string>();

            Debug.Print("ScanButton was clicked.");
            if( MyFolders.Count() > 0)
            {
                searchPath = GetUniquePaths();
                ScanFolders(searchPath);
            }
        }

        private void ScanFolders(List<string> pathsToSearch)
        {
            Debug.Print("ScanFolders():");
            string root = pathsToSearch[0];
            var searchPattern = new Regex(".*", RegexOptions.IgnoreCase);

            //List<string> allDirs = new List<string>();
            var allDirs = Directory.EnumerateFiles(root,"*",SearchOption.AllDirectories)
                .Where(f => searchPattern.IsMatch(f)).ToList();

            Dictionary<string, List<string>> myDic = new Dictionary<string, List<string>>();
            
            foreach (string fileName in allDirs)
            {
                Debug.Print(fileName);
                //Debug.Print(Path.GetFileName(fileName));
                //Debug.Print(Path.GetDirectoryName(fileName));
                string key = Path.GetFileName(fileName);
                //string value = Path.GetDirectoryName(fileName);
                if ( myDic.ContainsKey(key))
                {
                    myDic[key].Add(fileName);
                }
                else
                {
                    myDic.Add(key, new List<string>() { fileName });
                }
            }

            foreach(string key in myDic.Keys)
            {
                Debug.Print("{0}:", key);
                foreach(string item in myDic[key])
                {
                    Debug.Print("\t{0}",item);
                }
            }
            //BigDic = new ObservableCollection<KeyValuePair<string, List<string>>>(myDic);
            BigDic = new SortedDictionary<string, List<string>>(myDic);
            DuplicateTreeView.ItemsSource = BigDic;

            //var dictionary = new Dictionary<string, List<string>>
            //                     {
            //                         {"Item 1", new List<string> {"Value 1", "Value 2", "Value 3"}},
            //                         {"Item 2", new List<string> {"Value 1"}},
            //                         {"Item 3", new List<string> {"Value 1", "Value 2"}}
            //                     };
            //DuplicateTreeView.ItemsSource = dictionary;






            foreach (var pair in BigDic)
            {
                Debug.Print("key: {0} {1}", pair.Key, BigDic[pair.Key]);
            }


        }
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.Print("StopButton was clicked.");
        }

        private List<string> GetUniquePaths()
        {
            Debug.WriteLine("GetUniquePaths():");
            List<string> tempList = new List<string>();
            //List<string> searchList = new List<string>();
            var sortedList = MyFolders.OrderBy(x => x.Length).Distinct().ToList();
            //searchList = sortedList.ToList();
            if (sortedList.Count() == 1)
            {
                Debug.Print(sortedList[0]);
                return sortedList;
            }
            else
            {
                for (int i = 0; i < sortedList.Count() - 1; i++)
                {
                    for (int j = i + 1; j < sortedList.Count; j++)
                    {
                        Debug.Print("---");
                        Debug.WriteLine(sortedList[i]);
                        Debug.WriteLine(sortedList[j]);

                        if (sortedList[j].Contains(sortedList[i]))
                        {
                            tempList.Add(sortedList[j]);
                            Debug.Print(":");
                            Debug.Print(string.Join("\n", tempList));
                            Debug.Print(":");
                        }
                    }
                }

                Debug.Print("=== tempList ===");
                foreach(var item in tempList)
                {
                    Debug.Print(item);
                    sortedList.Remove(item);
                }
                Debug.Print("=== sortedList ===");
                Debug.Print(string.Join("\n", sortedList));

                return sortedList;
            }
        }

        //private void DuplicateTreeView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    var tree = sender as TreeView;
        //    if(tree.SelectedItem is TreeViewItem)
        //    {
        //        var item = tree.SelectedItem as TreeViewItem;
        //        this.Title = "Selected header: " + item.Header.ToString();
        //    }
        //    else if(tree.SelectedItem is string)
        //    {
        //        this.Title = "Selected: " + tree.SelectedItem.ToString();
        //    }
        //}

        //private void DuplicateTreeView_Loaded(object sender, RoutedEventArgs e)
        //{
        //    TreeViewItem item = new TreeViewItem();
        //    item.Header = "Filename";
        //    //item.ItemsSource = new string[] { "monitor", "cpu", "mouse" };
        //    TreeViewItem item2 = new TreeViewItem();
        //    item2.Header = "Outfit";
        //    //item2.ItemsSource = new string[] { "Pants", "Shirt", "Hat", "Socks" };

        //    var tree = sender as TreeView;
        //    //tree.Items.Add(item);
        //    //tree.Items.Add(item2);
        //}
    }
}
