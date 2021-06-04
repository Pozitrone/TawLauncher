using System.Windows;

namespace TawLauncher
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow
  {
    public MainWindow()
    {
      InitializeComponent();
      UpdateCore.ReadConfigFile();
    }

    private void StartLauncher(object sender, RoutedEventArgs e)
    {
      Launcher launcher = new Launcher();
      launcher.Show();
      this.Close();
    }
  }
}