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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Collections;
using System.Threading;
using WpfApp2;
using Newtonsoft.Json;

namespace Stuffa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static bool isPlaying = false;
        Music[] songs = new Music[3];
        // Process all files in the directory passed in, recurse on any directories 
        // that are found, and process the files they contain.
        public static void ProcessDirectory(string targetDirectory, ListBox list)
        {
            // Process the list of files found in the directory.
            try { string[] fileEntries = Directory.GetFiles(targetDirectory);
                foreach (string fileName in fileEntries)
                    ProcessFile(fileName, list);

                // Recurse into subdirectories of this directory.
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                foreach (string subdirectory in subdirectoryEntries)
                    ProcessDirectory(subdirectory, list);
            } catch { }
        }

        // Insert logic for processing found files here.
        public static void ProcessFile(string path, ListBox list)
        {
            //gets the file name
            list.Items.Add(path);
            // Console.WriteLine("Processed file '{0}'.", path);
        }

        public  void pausePlay()
        {
            
                if (isPlaying)
                {

                    player.Pause();
                    isPlaying = false;
                }
                else
                {
                    player.Play();
                    isPlaying = true;
                }
            
        }

        public void pausePlayServer()
        {
            this.Dispatcher.Invoke(() => { pausePlay(); });
        }

        public void startServer()
        {
            Server server = new Server();
            while (true)
            {
                string message = server.startServer();
                if (message == "P")
                {

                    pausePlayServer();
                }
            }
        }
       



        public MainWindow()
        {
            Thread.CurrentThread.Name = "parent";
            InitializeComponent();

            ProcessDirectory("C:\\Users\\Fredrik\\source\\repos\\stuffa\\Musik\\", list);

            progresBar.Value = 0.5;
            Thread serverThread = new Thread(startServer);
            serverThread.IsBackground = true;
            serverThread.Start();

         
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string path = list.SelectedItem.ToString();
            // Denna funkar bara efter att man har lagt till  en ny låt - därför utkommenterad
            //textBox.Text = "Path: " + songs[0].path + " Title: " + songs[0].name;
            TagLib.File tagFile = TagLib.File.Create(path);
            string artist = tagFile.Tag.JoinedPerformers;
            string album = tagFile.Tag.JoinedAlbumArtists;
            string songName = tagFile.Tag.Title;
            var length = tagFile.Properties.Duration;

            label.Content = length;

            text.Text = path + "\n\nmusik grupp: " + artist + " Album: " + album + " Title: " + songName;

            player.Source = new Uri(path, UriKind.RelativeOrAbsolute);


            player.Play();
            isPlaying = true;


        }

       

        public void Button_Click(object sender, RoutedEventArgs e)
        {

            pausePlay();
            
            
             
            

            
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
 
        private void open_song_click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".mp3";
            dlg.Filter = "MP3 Files (*.mp3)|*.mp3|M4A Files (*.m4a)|*.m4a|FLAC Files (*.flac)|*.flac";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                list.Items.Add(filename);
                textBox1.Text = filename;

                songs[0] = new Music(filename, System.IO.Path.GetFileNameWithoutExtension(filename));
            }
        }

        private void progresBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            List<string> musicTracks = new List<string>();
            foreach(var i in list.Items)
            {
                musicTracks.Add(i.ToString());
            }
            // package Newtonsoft.Json (Json.Net) need to be installed. 
            // to install go to "project" > "manage NuGet packages..." > "Brows" > type "Newtonsoft.Json" / "Json.Net" > "install"
            string json = JsonConvert.SerializeObject(musicTracks.ToArray());
            System.IO.File.WriteAllText(@"D:\path8ihbjhgnbbv.txt", json);


        }

        private void load_Click(object sender, RoutedEventArgs e)
        {
            //List<string> musicTracks = new List<string>();

            using (StreamReader r = new StreamReader(@"D:\path8ihbjhgnbbv.txt"))
            {
                string json = r.ReadToEnd();
                List<string> musicTracks = JsonConvert.DeserializeObject<List<string>>(json);

                foreach (string i in musicTracks.ToArray())
                {
                    list.Items.Add(i);

                }
            }
            
        }
    }
}