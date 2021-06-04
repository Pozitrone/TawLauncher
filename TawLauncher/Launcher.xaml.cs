using System;
using System.Windows;

namespace TawLauncher
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class Launcher
  {
    public Launcher()
    {
      InitializeComponent();
    }

    private void B1_OnClick(object sender, RoutedEventArgs e)
    {
      try
      {
        Version newVersion = UpdateCore.CheckForUpdates();
        if (newVersion != null) B2.IsEnabled = true;
        B1.IsEnabled = false;
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }

    private void B2_OnClick(object sender, RoutedEventArgs e)
    {
      try
      {
        UpdateCore.Update();
        B2.IsEnabled = false;
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }

    private void B3_OnClick(object sender, RoutedEventArgs e)
    {
      try
      {
        UpdateCore.Run();
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }
  }
}