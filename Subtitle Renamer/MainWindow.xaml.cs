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
        public class Filer : Dictionary<string, string>
        {
            string GetFileNameWithoutExtension(string path)
            {
                var fileName = System.IO.Path.GetFileName(path);
                var dotIndex = fileName.IndexOf('.');
                var fileNameWithoutExtension = fileName.Substring(0, dotIndex);

                return fileNameWithoutExtension;
            }

            public void Add(string path)
            {
                Add(path, GetFileNameWithoutExtension(path));
            }
        }

        public Filer MediaFiler = new Filer();
        public Filer SubtitleFiler = new Filer();

        public class Matcher
        {
            class LevenshteinDistance
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

            public List<(string, string)> Mapping(IEnumerable<string> mediaNames, IEnumerable<string> subtitleNames)
            {
                var mappingTable = new List<(string, string)>(subtitleNames.Count());

                foreach (var subtitle in subtitleNames)
                {
                    var media = mediaNames.Aggregate((min, next) =>
                        LevenshteinDistance.Compute(subtitle, min) <
                        LevenshteinDistance.Compute(subtitle, next) ? min : next);

                    mappingTable.Add((media, subtitle));
                }

                return mappingTable;
            }
        }

        Matcher matcher = new Matcher();

        public void AddFiles(ListBox list, Filer filer, string[] fileNames)
        {
            foreach (var item in fileNames)
            {
                try
                {
                    filer.Add(item);
                    list.Items.Add(item);
                }
                catch (ArgumentException)
                {

                }
            }

            list.Items.SortDescriptions.Add(
                new System.ComponentModel.SortDescription(
                    "", System.ComponentModel.ListSortDirection.Ascending));
        }

        public void ClearFiles(ListBox list, Filer filer)
        {
            filer.Clear();
            list.Items.Clear();
        }

        public List<(string, string)> Preview()
        {
            var previewList = new List<(string, string)>();
            var mediaNames = MediaFiler.Values.ToHashSet();
            var subtitleNames = SubtitleFiler.Values.ToHashSet();

            var mappingTable = matcher.Mapping(mediaNames, subtitleNames);
            foreach (var mappingTuple in mappingTable)
            {
                var mediaPaths = MediaFiler.Where(pair => pair.Value == mappingTuple.Item1).Select(pair => pair.Key);
                var subtitlePaths = SubtitleFiler.Where(pair => pair.Value == mappingTuple.Item2).Select(pair => pair.Key);

                foreach (var mediaPath in mediaPaths)
                {
                    foreach (var subtitlePath in subtitlePaths)
                    {
                        previewList.Add((mediaPath, subtitlePath));
                    }
                }
            }


            return previewList;
        }

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
                AddFiles(MediaList, MediaFiler, openFileDialog.FileNames);
            }
        }
        private void SubtitleOpen_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                AddFiles(SubtitleList, SubtitleFiler, openFileDialog.FileNames);
            }
        }

        private void MediaClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFiles(MediaList, MediaFiler);
        }
        private void SubtitleClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFiles(SubtitleList, SubtitleFiler);
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

        private void MediaList_Drop(object sender, DragEventArgs e)
        {
            AddFiles(MediaList, MediaFiler, (string[])e.Data.GetData(DataFormats.FileDrop));
        }
        private void SubtitleList_Drop(object sender, DragEventArgs e)
        {
            AddFiles(SubtitleList, SubtitleFiler, (string[])e.Data.GetData(DataFormats.FileDrop));
        }

        private void MediaList_Loaded(object sender, RoutedEventArgs e)
        {
            AllowDrop = true;
        }
        private void SubtitleList_Loaded(object sender, RoutedEventArgs e)
        {
            AllowDrop = true;
        }
        private void Preview_Click(object sender, RoutedEventArgs e)
        {
            Previex_View.Items.Clear();

            var previewList = Preview();
            foreach (var previewTuple in previewList)
            {
                Previex_View.Items.Add(new { MediaPath = previewTuple.Item1, SubtitlePath = previewTuple.Item2 });
            }
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
}
