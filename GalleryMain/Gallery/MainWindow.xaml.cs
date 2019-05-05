﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.MessageBox;
using System.ComponentModel;
using System.Threading;
using ProgressBar = System.Windows.Forms.ProgressBar;

namespace Gallery
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {

        IList<Image> images = new List<Image>();
        private List<string> filter = new List<string>() { @"bmp", @"jpg", @"gif", @"png" };
        Dictionary<Image, DateTime> photos = new Dictionary<Image, DateTime>();
        public MainWindow()
        {
            InitializeComponent();
        }
        bool Check;
        private void LoadButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            images.Clear();
            listbox.ItemsSource = null;
            FileInfo[] files;
            DirectoryInfo info = new DirectoryInfo(textBox.Text);
            try
            {

                      
                files = info.GetFiles().OrderBy(p => p.Name).ToArray();
                if(Check==true)
                switch ((ComboBoxShow.SelectedIndex))
                { 
                case 0:
                        files = info.GetFiles().OrderBy(p => p.Name).ToArray();
                        break;
                    case 1:
                        files = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();
                        break;
                    case 2:
                        files = info.GetFiles().OrderBy(p => p.LastWriteTime).ToArray();
                        break;
                    case 3:
                        files = info.GetFiles().OrderBy(p => p.Length).ToArray();
                        break;
                }
                else
                    switch ((ComboBoxShow.SelectedIndex))
                    { 
                case 0:
                        files = info.GetFiles().OrderByDescending(p => p.Name).ToArray();
                    break;
                    case 1:
                        files = info.GetFiles().OrderByDescending(p => p.CreationTime).ToArray();
                    break;
                    case 2:
                        files = info.GetFiles().OrderByDescending(p => p.LastWriteTime).ToArray();
                    break;
                    case 3:
                        files = info.GetFiles().OrderByDescending(p => p.Length).ToArray();
                    break;
                }
            }
            catch
            {
                MessageBox.Show("Folder path is not correct");
                throw new FileNotFoundException();
            }

            foreach (var file in files)
            {
                pbStatus.Visibility = Visibility.Visible;
                pbStatus.Maximum = files.Count();
                ++pbStatus.Value; 
                if (filter.Exists(n => n == file.Name.Split('.').Last().ToLower()))
                {
                    try
                    {
                        var bi = new BitmapImage(new Uri(file.FullName))
                        {
                            CacheOption = BitmapCacheOption.OnLoad
                        };

                        var img = new System.Windows.Controls.Image
                        {
                            Source = bi,
                            Width = 150,
                            Height = 100
                        };
                        images.Add(img);
                        photos.Add(img,file.CreationTimeUtc);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            listbox.ItemsSource = images;
            pbStatus.Visibility = Visibility.Hidden;
            
        }
        private void OnPhotoClick(object sender, MouseButtonEventArgs e)
        {

            frmViewer pvWindow = new frmViewer
            {
                SelectedPhoto = (Image)listbox.SelectedItem,
                Images = images,
                needAnimation = false,
                interval = 2
            };
            pvWindow.Show();
        }

        private void Open_Folder_Click(object sender, RoutedEventArgs e)
        {
            pbInterminate.Visibility = Visibility.Visible;
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                textBox.Text = dialog.SelectedPath;
            }
            pbInterminate.Visibility = Visibility.Hidden;
        }

        private void StartSlideShowButton_Click(object sender, RoutedEventArgs e)
        {

            frmViewer pvWindow = new frmViewer
            {
                SelectedPhoto = (Image)listbox.SelectedItem,
                Images = images,
                needAnimation = true,
                interval = Convert.ToInt32(textBox_interval.Text),
                Check = Check
            };
            pvWindow.Show();
        }

        private void Button_create_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBoxDescending_Unchecked(object sender, RoutedEventArgs e)
        {
            Check = false;
        }

        private void CheckBoxDescending_Checked(object sender, RoutedEventArgs e)
        {
            Check = true;
        }

        private void Listbox_GotFocus(object sender, RoutedEventArgs e)
        {
            foreach (var pair in photos)
            {
                if (listbox.SelectedItem == pair.Key)
                {
                    label_date.Content = pair.Value;
                    Console.WriteLine(pair.Value);
                }
            }
        }

        private void TextBoxSearch_MouseUp(object sender, RoutedEventArgs e)
        {
            textBoxSearch.Foreground = Brushes.Black;
            textBoxSearch.Text = "";
        }

        private void TextBoxSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            textBoxSearch.Foreground = Brushes.Silver;
            textBoxSearch.Text = "Enter value";
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            images.Clear();

            switch ((ComboBoxSearch.SelectedIndex))
            {
                case 0:
                    foreach (var pair in photos)
                    {
                        short b;
                        if (Int16.TryParse(textBoxSearch.Text, out b) == false)
                            MessageBox.Show("Error!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        short c = Convert.ToInt16(pair.Value.ToString("dd"));
                        if (b == c)
                        {
                            Image img = pair.Key;
                            images.Add(img);
                        }
                    }
                    break;
                case 1:
                    foreach (var pair in photos)
                    {
                        short b;
                        if (Int16.TryParse(textBoxSearch.Text, out b) == false)
                            MessageBox.Show("Error!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        short c = Convert.ToInt16(pair.Value.ToString("MM"));
                        if (b == c)
                        {
                            Image img = pair.Key;
                            images.Add(img);
                        }
                    }
                    break;
                case 2:
                    foreach (var pair in photos)
                    {
                        short b;
                        if (Int16.TryParse(textBoxSearch.Text, out b) == false)
                            MessageBox.Show("Error!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        short c = Convert.ToInt16(pair.Value.ToString("yyyy"));
                        if (b == c)
                        {
                            Image img = pair.Key;
                            images.Add(img);
                        }
                    }
                    break;
            }
            listbox.ItemsSource = images;
            images.Clear();
        }
    }
}