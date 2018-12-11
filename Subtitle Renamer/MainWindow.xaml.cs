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

namespace Subtitle_Renamer
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MediaOpen_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    MediaList.Items.Add(filename);
                }
            }
        }

        private void MediaClear_Click(object sender, RoutedEventArgs e)
        {
            MediaList.Items.Clear();
        }

        private void MediaList_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void MediaList_Drop(object sender, DragEventArgs e)
        {
            string[] FileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string filename in FileNames)
            {
                MediaList.Items.Add(filename);
            }
        }

        private void MediaList_Loaded(object sender, RoutedEventArgs e)
        {
            AllowDrop = true;
        }

        private void SubtitleOpen_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    SubtitleList.Items.Add(filename);
                }
            }
        }

        private void SubtitleClear_Click(object sender, RoutedEventArgs e)
        {
            SubtitleList.Items.Clear();
        }

        private void SubtitleList_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void SubtitleList_Drop(object sender, DragEventArgs e)
        {
            string[] FileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string filename in FileNames)
            {
                SubtitleList.Items.Add(filename);
            }
        }

        private void SubtitleList_Loaded(object sender, RoutedEventArgs e)
        {
            AllowDrop = true;
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            int MediaCount = MediaList.Items.Count;
            int SubtitleCount = SubtitleList.Items.Count;
            int Count = Math.Min(MediaCount, SubtitleCount);

            for (int i = 0; i < Count; i++)
            {
                var MediaString = MediaList.Items.GetItemAt(i).ToString();
                var SubtitleString = SubtitleList.Items.GetItemAt(i).ToString();

                var Directory = System.IO.Path.GetDirectoryName(SubtitleString);
                var FileName = System.IO.Path.GetFileNameWithoutExtension(MediaString);
                string Extension;

                if (Postfix.IsChecked ?? false)
                {
                    var SubtitleName = System.IO.Path.GetFileName(SubtitleString);
                    var Index = SubtitleName.IndexOf(".");
                    Extension = SubtitleName.Substring(Index);
                }
                else
                {
                    Extension = System.IO.Path.GetExtension(SubtitleString);
                }

                var NewPath = System.IO.Path.Combine(Directory, FileName + Extension);

                System.IO.File.Move(SubtitleString, NewPath);
            }
        }
    }

    public class Matcher
    {
        //public enum Type
        //{
        //    Media,
        //    Subtitle
        //}

        //public void AddFiles(Type type, string[] MediaNames)
        //{
        //    switch (type)
        //    {
        //        case Type.Media:
        //            break;
        //        case Type.Subtitle:
        //            break;
        //        default:
        //            break;
        //    }
        //}

        public void AddMediaFiles(string[] MediaNames)
        {

        }

        public void AddSubtitleFiles(string[] SubtitleNames)
        {

        }

        public void RemoveMediaFiles(string[] MediaNames)
        {

        }

        public void RemoveSubtitleFiles(string[] SubtitleNames)
        {

        }

        public void ClearMediaFiles()
        {

        }

        public void ClearSubtitleFiles()
        {

        }

        public string[] GetMediaFiles()
        {

        }

        public string[] GetSubtitleFiles()
        {

        }

        public string[] GetRenameFiles()
        {

        }

        public bool MediaUpdated()
        {
            return IsUpdated(ref _bMediaUpdated);
        }

        public bool SubtitleUpdated()
        {
            return IsUpdated(ref _bSubtitleUpdated);
        }

        public bool RenameUpdated()
        {
            return IsUpdated(ref _bRenameUpdated);
        }

        private bool _bMediaUpdated = false;
        private bool _bSubtitleUpdated = false;
        private bool _bRenameUpdated = false;

        private bool IsUpdated(ref bool Updated)
        {
            bool Current = Updated;
            Updated = false;

            return Current;
        }
    }
}
