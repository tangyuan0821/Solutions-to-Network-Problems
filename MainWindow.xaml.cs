using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows;

namespace NetworkTool
{
    public partial class MainWindow : Window
    {
        private const string AdapterName = "以太网"; // 根据实际适配器名称修改

        public MainWindow()
        {
            InitializeComponent();
            RefreshStatus(null, null);
        }

        private void RefreshStatus(object sender, RoutedEventArgs e)
        {
            bool isConnected = NetworkInterface.GetIsNetworkAvailable();
            StatusText.Text = isConnected ? "网络状态：已连接" : "网络状态：未连接";
        }

        private void RestartAdapter(object sender, RoutedEventArgs e)
        {
            if (!IsAdministrator())
            {
                MessageBox.Show("需要管理员权限！");
                RestartAsAdmin();
                return;
            }

            ExecuteCommand($"netsh interface set interface \"{AdapterName}\" disable");
            ExecuteCommand($"netsh interface set interface \"{AdapterName}\" enable");
            MessageBox.Show("网络适配器已重启！");
        }

        private bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void RestartAsAdmin()
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = Process.GetCurrentProcess().MainModule.FileName,
                UseShellExecute = true,
                Verb = "runas"
            };

            try
            {
                Process.Start(processInfo);
                Application.Current.Shutdown();
            }
            catch
            {
                MessageBox.Show("操作被用户取消");
            }
        }

        private void ExecuteCommand(string command)
        {
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C {command}",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                process.Start();
                process.WaitForExit();
            }
        }
    }
}