using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace 简单关系图_测试_
{
    public partial class MainWindow : Window
    {
        private string myDir = Path.Combine(KnownFolders.GetPath(KnownFolder.Documents), "demoTool", "simpleGXT");// *程序文件夹
        public static string defaultSaveDir = KnownFolders.GetPath(KnownFolder.Pictures);// *默认保存文件夹
        public static string ConfigFilePath { get; set; }// *配置文件路径
        public static AppConfigManager _configManager; // 配置管理
        public MainWindow()
        {
            InitializeComponent();
            InputTextBox.Focus(); // 设置焦点到 InputTextBox
            // 创建程序文件夹*
            Directory.CreateDirectory(myDir);
            // 获取配置路径
            ConfigFilePath = Path.Combine(myDir, "config.xml");
            _configManager = new AppConfigManager(ConfigFilePath); // 初始化配置管理器
        }
        private void SaveImageButton_Click(object sender, RoutedEventArgs e)
        {
            DrawingControl.SaveToPngWithDialog();
        }
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            DrawingControl.ResetOffset();
        }
        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control)
            {
                SubmitData();
                e.Handled = true; // 阻止事件继续传递
            }
        }
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            SubmitData();
        }
        private void SubmitData()
        {
            // 获取 TextBox 中的文本，并按换行符分割成字符串数组
            string[] lines = InputTextBox.Text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            // 更新 CustomDrawingControl 的 StringsToDraw 属性
            DrawingControl.StringsToDraw = lines;
            // 强制 CustomDrawingControl 重新绘制
            DrawingControl.InvalidateVisual();
        }
    }
}