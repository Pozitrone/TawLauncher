using System;
using System.Windows;

namespace TawLauncher
{
  /// <summary>
  /// Interaction logic for Launcher.xaml
  /// </summary>
  public partial class Launcher
  {
    public Launcher()
    {
      
      try
      {
        InitializeComponent();
        Init();
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }

    private void Init()
    {
      Version newVersion = UpdateCore.CheckForUpdates();
      Version currentVersion = UpdateCore.CheckCurrentVersion();

      if (currentVersion != null)
      {
        CurrentVersionText.Text = currentVersion.ToString();
      }

      if (!UpdateCore._automaticallyUpdate)
      {
        if (currentVersion != null)
        {
          RunButton.Visibility = Visibility.Visible;
          RunButton.IsEnabled = true;
        }
        else
        {
          RunButton.Visibility = Visibility.Hidden;
          RunButton.IsEnabled = false;
        }

        if (UpdateCore._updateAvailable)
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
          UpdateButton.IsEnabled = false;
          NewVersionHeader.Visibility = Visibility.Hidden;
          NewVersionText.Visibility = Visibility.Hidden;
          Log.Text = "No update available.";
          RunButton.Content = "Run";

          if (UpdateCore._runAfterUpdate)
          {
            UpdateCore.Run();
          }
        }
      }
      else
      {
        if (UpdateCore._updateAvailable) UpdateCore.Update();
        else UpdateCore.Run();
      }
    }

    private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
    {
      UpdateCore.Update();
      Init();
    }

    private void RunButton_OnClick(object sender, RoutedEventArgs e)
    {
      UpdateCore.Run();
    }
  }
}