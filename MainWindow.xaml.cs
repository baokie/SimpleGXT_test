using Microsoft.Win32;
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
        public static string defaultSaveDir_Text = KnownFolders.GetPath(KnownFolder.Documents);// *默认保存文件夹_文本
        public static bool isUpdateName = true;// *当前更新文件名
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
       
        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control)// ：提交
            {
                SubmitData();
                e.Handled = true; // 阻止事件继续传递
            }
            else if(e.Key == Key.R && Keyboard.Modifiers == ModifierKeys.Control)// ：重置
            {
                DrawingControl.ResetOffset();
                e.Handled = true; // 阻止事件继续传递
            }
            else if (e.Key == Key.U && Keyboard.Modifiers == ModifierKeys.Control)// ：更新文件名
            {
                updateName();
                e.Handled = true; // 阻止事件继续传递
            }
            else if(e.Key == Key.S && Keyboard.Modifiers ==( ModifierKeys.Control |  ModifierKeys.Shift))// ：保存图片
            {
                DrawingControl.SaveToPngWithDialog();
                e.Handled = true; // 阻止事件继续传递
            }
            else if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)// ：保存文本
            {
                saveText();
                e.Handled = true; // 阻止事件继续传递
            }
        }
        // ：更新文件名
        private void UpdateNameButton_Click(object sender, RoutedEventArgs e)
        {
            updateName();
        }
        private void updateName()
        {
            isUpdateName = !isUpdateName;
            if (isUpdateName)
            {
                UpdateNameButton.Content = "当前更新文件名（Ctrl+U）";
            }
            else
            {
                UpdateNameButton.Content = "当前不更新文件名（Ctrl+U）";
            }
        }
        // ：保存文本
        private void SaveTextButton_Click(object sender, RoutedEventArgs e)
        {
            saveText();
        }
        private String keySavePath = "LastSavePath_Text";
        private String keySaveName = "LastSaveName_Text";
        private void saveText()
        {
            Dispatcher.Invoke(() =>
            {
                String name = "txt_" + SomeDeal.GetCurrentDateTimeFormatted() + ".txt";
                if (!isUpdateName)
                {
                    name = MainWindow._configManager.GetString(keySaveName, name);
                }
                var saveDialog = new SaveFileDialog
                {
                    InitialDirectory = MainWindow._configManager.GetString(keySavePath, MainWindow.defaultSaveDir_Text),
                    Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                    FileName = name,
                    AddExtension = true
                };
                if (saveDialog.ShowDialog() == true)
                {
                    var filePath = saveDialog.FileName;
                    // 获取文本框内容
                    string textToSave = InputTextBox.Text;
                    try
                    {
                        File.WriteAllText(filePath, textToSave);// 将文本内容保存到文件
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error saving file: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);// 显示错误消息
                    }
                    MainWindow._configManager.SaveString(keySavePath, System.IO.Path.GetDirectoryName(filePath));
                    MainWindow._configManager.SaveString(keySaveName, System.IO.Path.GetFileName(filePath));
                }
            });
        }
        // ：保存图片
        private void SaveImageButton_Click(object sender, RoutedEventArgs e)
        {
            DrawingControl.SaveToPngWithDialog();
        }
        // ：重置
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            DrawingControl.ResetOffset();
        }
        // ：提交
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            SubmitData();
        }
        private void SubmitData()
        {
            // 获取 TextBox 中的文本，并按换行符分割成字符串数组
            string[] lines = InputTextBox.Text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            // 更新 CustomDrawingControl 的 StringsToDraw 属性
            DrawingControl.StringsToDraw = RuleWXT.getNameByLine(lines[0]);
            // 强制 CustomDrawingControl 重新绘制
            DrawingControl.InvalidateVisual();
        }
    }
}