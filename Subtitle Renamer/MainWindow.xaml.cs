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
        MediaDictionary Media = new MediaDictionary();
        SubtitleDictionary Subtitle = new SubtitleDictionary();

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
                foreach (var FileName in openFileDialog.FileNames)
                {
                    Media.AddFile(FileName);
                }

                MediaList.Items.Clear();
                foreach (var FilePath in Media.GetFilePaths())
                {
                    MediaList.Items.Add(FilePath);
                }
            }
        }

        private void MediaClear_Click(object sender, RoutedEventArgs e)
        {
            Media.Clear();
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
            foreach (var FileName in FileNames)
            {
                Media.AddFile(FileName);
            }

            MediaList.Items.Clear();
            foreach (var FilePath in Media.GetFilePaths())
            {
                MediaList.Items.Add(FilePath);
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
                foreach (var FileName in openFileDialog.FileNames)
                {
                    Subtitle.AddFile(FileName);
                }

                SubtitleList.Items.Clear();
                foreach (var FilePath in Subtitle.GetFilePaths())
                {
                    SubtitleList.Items.Add(FilePath);
                }
            }
        }

        private void SubtitleClear_Click(object sender, RoutedEventArgs e)
        {
            Subtitle.Clear();
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
            foreach (var FileName in FileNames)
            {
                Subtitle.AddFile(FileName);
            }

            SubtitleList.Items.Clear();
            foreach (var FilePath in Subtitle.GetFilePaths())
            {
                SubtitleList.Items.Add(FilePath);
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

    internal static class LevenshteinDistance
    {
        // Compute the distance between two strings.
        public static int Compute(string Source, string Target)
        {
            var SourceLength = Source.Length;
            var TargetLength = Target.Length;
            var Distance = new int[SourceLength + 1, TargetLength + 1];

            if (SourceLength == 0)
            {
                return TargetLength;
            }
            if (TargetLength == 0)
            {
                return SourceLength;
            }

            for (int SourceIndex = 0; SourceIndex <= SourceLength; SourceIndex++)
            {
                Distance[SourceIndex, 0] = SourceIndex;
            }
            for (int TargetIndex = 0; TargetIndex <= TargetLength; TargetIndex++)
            {
                Distance[0, TargetIndex] = TargetIndex;
            }

            for (int SourceIndex = 1; SourceIndex <= SourceLength; SourceIndex++)
            {
                for (int TargetIndex = 1; TargetIndex <= TargetLength; TargetIndex++)
                {
                    var Cost = Target[TargetIndex - 1] == Source[SourceIndex - 1] ? 0 : 1;
                    var Compare = new[]
                    {
                        Distance[SourceIndex - 1, TargetIndex    ] + 1,
                        Distance[SourceIndex    , TargetIndex - 1] + 1,
                        Distance[SourceIndex - 1, TargetIndex - 1] + Cost
                    };

                    Distance[SourceIndex, TargetIndex] = Compare.Min();
                }
            }

            return Distance[SourceLength, TargetLength];
        }
    }

    public abstract class FileDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        protected string GetFileNameWithoutExtension(string path)
        {
            var FileName = System.IO.Path.GetFileName(path);
            var DotIndex = FileName.IndexOf('.');
            var FileNameWithoutExtension = FileName.Substring(0, DotIndex);

            return FileNameWithoutExtension;
        }

        public List<TKey> GetFileNamesWithoutExtension()
        {
            return new List<TKey>(Keys);
        }

        public abstract bool AddFile(TKey FilePath);
        public abstract bool RemoveFile(TKey FilePath);

        public abstract List<TKey> GetFilePaths();
    }

    public class MediaDictionary : FileDictionary<string, string>
    {
        public override bool AddFile(string MediaPath)
        {
            var MediaNameWithoutExtension = GetFileNameWithoutExtension(MediaPath);
            TryGetValue(MediaNameWithoutExtension, out string value);

            if (MediaPath != value)
            {
                this[MediaNameWithoutExtension] = MediaPath;
                return true;
            }

            return false;
        }

        public override bool RemoveFile(string MediaPath)
        {
            var MediaNameWithoutExtension = GetFileNameWithoutExtension(MediaPath);
            TryGetValue(MediaNameWithoutExtension, out string value);

            if (MediaPath == value)
            {
                Remove(MediaNameWithoutExtension);
                return true;
            }

            return false;
        }

        public override List<string> GetFilePaths()
        {
            var FilePaths = new List<string>(Values);
            FilePaths.Sort();

            return FilePaths;
        }
    }

    public class SubtitleDictionary : FileDictionary<string, HashSet<string>>
    {
        public override bool AddFile(string SubtitlePath)
        {
            var SubtitleNameWithoutExtension = GetFileNameWithoutExtension(SubtitlePath);
            var IsContains = TryGetValue(SubtitleNameWithoutExtension, out HashSet<string> value);

            if (!IsContains)
            {
                this[SubtitleNameWithoutExtension] = new HashSet<string> { SubtitlePath };
                return true;
            }
            else
            {
                return value.Add(SubtitlePath);
            }
        }

        public override bool RemoveFile(string SubtitlePath)
        {
            bool Removed = false;

            var SubtitleNameWithoutExtension = GetFileNameWithoutExtension(SubtitlePath);
            var IsContains = TryGetValue(SubtitleNameWithoutExtension, out HashSet<string> value);

            if (IsContains)
            {
                Removed = value.Remove(SubtitlePath);
                if (Removed)
                {
                    if (value.Count == 0)
                    {
                        Remove(SubtitleNameWithoutExtension);
                    }
                }
            }

            return Removed;
        }

        public override List<string> GetFilePaths()
        {
            var FilePaths = new List<string>();

            foreach (var value in Values)
            {
                FilePaths.AddRange(value);
            }
            FilePaths.Sort();

            return FilePaths;
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

        //public string[] GetMediaFiles()
        //{

        //}

        //public string[] GetSubtitleFiles()
        //{

        //}

        //public string[] GetRenameFiles()
        //{

        //}

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
