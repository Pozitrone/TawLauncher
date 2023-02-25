using System;
using System.Windows;
using System.Windows.Media.Imaging;
using TAWLauncher;

namespace TawLauncher
{
  public partial class Launcher
  {
    public Launcher()
    {
      
      try
      {
        InitializeComponent();
        UpdateCore.launcherInstance = this;
        Init();
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }

    private void Init()
    {
      Version newVersion = UpdateCore.newVersion;
      Version currentVersion = UpdateCore.currentVersion;
      LogoImage.Source = new BitmapImage(new Uri(Config.LoadingLogo));

      if (currentVersion != null)
      {
        CurrentVersionText.Text = currentVersion.ToString();
      }

      if (!UpdateCore.AutomaticallyUpdate)
      {
        if (UpdateCore.UpdateAvailable)
        {
          UpdateButton.IsEnabled = true;
          Log.Text = "Update available.";
          RunButton.Content = "Run without updating";

          NewVersionHeader.Visibility = Visibility.Visible;
          NewVersionText.Visibility = Visibility.Visible;
          NewVersionText.Text = (newVersion ?? currentVersion) == null
            ? "None"
            : (newVersion ?? currentVersion).ToString();
        }
        else
        {
          if (UpdateCore.RunAfterUpdate)
          {
            UpdateCore.Run();
          }
          
          UpdateButton.IsEnabled = false;
          NewVersionHeader.Visibility = Visibility.Hidden;
          NewVersionText.Visibility = Visibility.Hidden;
          Log.Text = "No update available.";
          RunButton.Visibility = Visibility.Visible;
          RunButton.Content = "Run";
          RunButton.IsEnabled = true;
        }
      }
      else
      {
        UpdateButton.IsEnabled = false;
        NewVersionHeader.Visibility = Visibility.Hidden;
        NewVersionText.Visibility = Visibility.Hidden;
        Log.Text = "No update available.";
        RunButton.Visibility = Visibility.Visible;
        RunButton.Content = "Run";
        RunButton.IsEnabled = true;
      }
    }

    private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
    {
      RunButton.IsEnabled = false;
      UpdateCore.Update();
      Init();
    }

    private void RunButton_OnClick(object sender, RoutedEventArgs e)
    {
      UpdateCore.Run();
    }
  }
}