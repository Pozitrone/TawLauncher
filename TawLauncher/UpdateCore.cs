#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace TawLauncher
{
  public static class UpdateCore
  {
    #region Variables

    public static Launcher launcherInstance;
    public static MainWindow mainWindowInstance;
    
    private static string _versionFileUrl;
    private static string _updateFileUrl;
    public static bool AutomaticallyUpdate { get; private set; } = true;
    private static string _exeToRun;
    public static bool RunAfterUpdate { get; private set; } = true;
    private static bool _keepZip;
    private static bool _keepLauncherOpen;

    private static bool HasMultipleFiles { get; set; }
    private static string[] _updateFileUrls;

    public static bool UpdateAvailable { get; private set; }
    
    private static ProgressBar _progressBar;

    public static Version newVersion;
    public static Version currentVersion;

    private class DownloadProgress
    {
      public long currentBytes;
      public long totalBytes;
    }
    
    private static readonly Dictionary<string, DownloadProgress> MultifileDownloadProgress = new Dictionary<string, DownloadProgress>();
    
    #endregion

    #region Functions

      #region CheckingVersions

      private static void CheckForUpdates()
      {
        currentVersion = CheckCurrentVersion();
        newVersion = CheckNewVersion();

        if (newVersion is null)
        {
          MessageBox.Show("Couldn't get updates. Please, check your connection.", "Taw Launcher");
          Application.Current.Shutdown();
        }

        if (currentVersion is null || currentVersion < newVersion)
        {
          UpdateAvailable = true;
        }
        else
        {
          Directory.Delete("temp", true);
        }
      }
      
      private static Version CheckCurrentVersion()
      {
        return !Directory.Exists("Data") ? null : ReadVersionFile();
      }

      private static Version CheckNewVersion()
      {
        if (_versionFileUrl == null) return null;

        DownloadVersionFile(_versionFileUrl);
        return ReadVersionFile("temp/version.txt");
      }

      #endregion

      #region Downloading

      public static void Update()
      {
        if (!UpdateAvailable)
        {
          if (!AutomaticallyUpdate) 
          {
            launcherInstance.RunButton.IsEnabled = true;
            return;
          }

          if (RunAfterUpdate) Run();
          else mainWindowInstance.RunLauncherAfterDelay();
          return;
        }
      
        if (AutomaticallyUpdate)
        {
          mainWindowInstance.MainProgress.Visibility = Visibility.Visible;
          mainWindowInstance.MainStatusText.Text = "Updating...";
        }

        if (Directory.Exists("Data")) Directory.Delete("Data", true);
        if (!HasMultipleFiles)
        {
          DownloadUpdateFile(_updateFileUrl);
        }
        else
        {
          int parts = 0;
          foreach (string updateFileUrl in _updateFileUrls)
          {
            DownloadUpdateFile(updateFileUrl, parts.ToString());
            parts++;
          }
        }
      
        File.Copy("temp/version.txt", "Data/version.txt");
        Directory.Delete("temp", true);

        UpdateAvailable = false;
      }
      
      private static void DownloadVersionFile(string url)
      {
        CreateTempDirectory();

        WebClient webClient = new WebClient();
        try
        {
          webClient.DownloadFile(url, "temp/version.txt");
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex + " ----- " + url);
        }
      }

      private static void DownloadUpdateFile(string url, string suffix = "")
      {
        CreateDirectoryTree();
        if (url == null) return;

        _progressBar =
          AutomaticallyUpdate ? mainWindowInstance.MainProgress : launcherInstance.LauncherProgress;
        _progressBar.Visibility = Visibility.Visible;
      
        if (!AutomaticallyUpdate)
        {
          launcherInstance.RunButton.Visibility = Visibility.Hidden;
          launcherInstance.RunButton.IsEnabled = false;
        }
      
        WebClient webClient = new WebClient();

        if (HasMultipleFiles)
        {
          webClient.DownloadProgressChanged += (obj, progress) =>
          {
            OnMultiDownloadProgressChanged(progress, suffix);
          };
        
          webClient.DownloadFileCompleted += (obj, progress) =>
          {
            OnMultiDownloadCompleted(suffix);
          };
        }
        else
        {
          webClient.DownloadProgressChanged += OnDownloadProgressChanged;
          webClient.DownloadFileCompleted += OnDownloadCompleted;
        }
      
      
        try
        {
          if (HasMultipleFiles) OnMultiDownloadStarted(suffix);
        
          webClient.DownloadFileAsync(new Uri(url), "Data/Application" + suffix + ".zip");
        }
        catch
        {
          MessageBox.Show("Couldn't download update.");
          Application.Current.Shutdown();
        }
      }

      #endregion

      #region Reading

      private static Version ReadVersionFile(string path = "Data/version.txt")
      {
        return !File.Exists(path) ? null : new Version(File.ReadLines(path).First());
      }

      public static void ReadConfigFile()
      {
        if (!File.Exists("taw.conf"))
        {
          MessageBox.Show("Config file not found. Generating blank config file.", "Taw Launcher");
          GenerateConfigFile();
          Application.Current.Shutdown();
        }
        else
        {
          string[] rawConfig = File.ReadAllLines("taw.conf");
          //MessageBox.Show(rawConfig.Where(line => line != "" && line[0] != '#').ToList().Aggregate((x , y) => x + "\n" + y).ToString());
          List<string> config = rawConfig.Where(line => line != "" && line[0] != '#').Select(x => x.Split(new[] { '=' }, 2)[1]).ToList();

          try
          {
            _versionFileUrl = config[0];
            AutomaticallyUpdate = bool.Parse(config[2]);
            _exeToRun = config[3].Contains(".exe") ? config[3] : config[3] + ".exe";
            RunAfterUpdate = bool.Parse(config[4]);
            _keepZip = bool.Parse(config[5]);
            _keepLauncherOpen = bool.Parse(config[6]);

            string[] urls = config[1].Split('|');
            if (urls.Length == 1) _updateFileUrl = urls[0];
            else
            {
              HasMultipleFiles = true;
              _updateFileUrls = urls;
            }
          }
          catch
          {
            MessageBox.Show("Config file invalid. Please, check the values.", "Taw Launcher");
            Application.Current.Shutdown();
          }
        }

        ValidateConfig();
      }

      #endregion

      #region FileHandling

      private static void CreateTempDirectory()
      {
        if (Directory.Exists("temp")) Directory.Delete("temp", true);
        Directory.CreateDirectory("temp");
      }

      private static void CreateDirectoryTree()
      {
        if (Directory.Exists("Data")) return;
        Directory.CreateDirectory("Data");
      }
      
      private static void GenerateConfigFile()
      {
        FileStream configFile = File.Create("taw.conf");
        configFile.Close();

        string[] config = new string[]
        {
          "# Url where the app downloads the version.txt from",
          "VERSION_FILE_URL=\n",

          "# Url to the zipped project",
          "APPLICATION_ZIP_URL=\n",

          "# Whether the app should automatically update upon startup (default FALSE)",
          "AUTOMATICALLY_UPDATE=FALSE\n",

          "# Name of the .exe file within the zip",
          "EXE_TO_RUN=\n",

          "# Automatically start the app after finishing the download (default FALSE)",
          "RUN_AFTER_UPDATE_FINISHED=FALSE\n",

          "# Whether the app should keep the zip file (default FALSE)",
          "KEEP_ZIP_FILE=FALSE\n",

          "# Whether the launcher should be kept open after starting the app (default FALSE)",
          "KEEP_LAUNCHER_OPEN=FALSE\n"
        };
        File.WriteAllLines("taw.conf", config);
      }

      #endregion

      #region EventHandlers

      private static void OnDownloadCompleted(object obj, AsyncCompletedEventArgs progress) 
    {
      {
        ZipFile.ExtractToDirectory("Data/Application.zip", "Data/Application");
        if (!_keepZip) File.Delete("Data/Application.zip");
        if (AutomaticallyUpdate) mainWindowInstance.RunLauncherAfterDelay(0);
        if (RunAfterUpdate) Run();
        else
        {
          launcherInstance.Log.Text = "Update complete";
          launcherInstance.RunButton.IsEnabled = true;
          launcherInstance.RunButton.Visibility = Visibility.Visible;
        }
      }
    }
    
    private static void OnMultiDownloadCompleted(string suffix) 
    {
      {
        ZipFile.ExtractToDirectory("Data/Application" + suffix + ".zip", "Data/Application");
        if (!_keepZip) File.Delete("Data/Application" + suffix + ".zip");

        foreach (KeyValuePair<string, DownloadProgress> keyValuePair in MultifileDownloadProgress)
        {
          if (keyValuePair.Value.currentBytes != keyValuePair.Value.totalBytes) return;
        }

        if (AutomaticallyUpdate) mainWindowInstance.RunLauncherAfterDelay(0);
        if (RunAfterUpdate) Run();
        else
        {
          launcherInstance.Log.Text = "Update complete";
          launcherInstance.RunButton.IsEnabled = true;
          launcherInstance.RunButton.Visibility = Visibility.Visible;
        }
      }
    }

    private static void OnDownloadProgressChanged(object obj, DownloadProgressChangedEventArgs progress)
    {
      _progressBar.Maximum = progress.TotalBytesToReceive;
      _progressBar.Value = progress.BytesReceived;
      if (AutomaticallyUpdate) return;
      launcherInstance.Log.Text = "Updating...";
      launcherInstance.RunButton.IsEnabled = false;
    }

    private static void OnMultiDownloadProgressChanged(DownloadProgressChangedEventArgs progress,
      string suffix)
    {
      MultifileDownloadProgress[suffix].currentBytes = progress.BytesReceived;
      MultifileDownloadProgress[suffix].totalBytes = progress.TotalBytesToReceive;

      _progressBar.Maximum = MultifileDownloadProgress.Sum(x => x.Value?.totalBytes ?? 0);
      _progressBar.Value = MultifileDownloadProgress.Sum(x => x.Value?.currentBytes ?? 0);
    }

    private static void OnMultiDownloadStarted(string suffix)
    {
      MultifileDownloadProgress.Add(suffix, new DownloadProgress());
      if (AutomaticallyUpdate) return;
      launcherInstance.Log.Text = "Updating...";
      launcherInstance.RunButton.IsEnabled = false;
    }

      #endregion

      #region Misc

      public static void Run()
      {
        if (!File.Exists(Path.GetFullPath("Data/Application/" + _exeToRun))) return;
        Process.Start(Path.GetFullPath("Data/Application/" + _exeToRun));
        if (!_keepLauncherOpen) Application.Current.Shutdown();
      }
      
      private static void ValidateConfig()
      {
        try
        {
          if (_versionFileUrl == String.Empty) throw new Exception("VERSION_FILE_URL");
          if (_updateFileUrl == String.Empty) throw new Exception("APPLICATION_ZIP_URL");
          //_automaticallyUpdate
          if (_exeToRun == ".exe") throw new Exception("EXE_TO_RUN");
          //_runAfterUpdate
          //_keepZip
          //_keepLauncherOpen
        }
        catch (Exception ex)
        {
          MessageBox.Show("Config file invalid. Please, check the values. \n Invalid value: " + ex.Message,
            "Taw Launcher");
          Application.Current.Shutdown();
        }

        CheckForUpdates();
      }

      #endregion
    #endregion
  }
}