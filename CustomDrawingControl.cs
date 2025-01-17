﻿using Microsoft.Win32;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace 简单关系图_测试_
{
    
    public class CustomDrawingControl : Control
    {
        // *自定义属性
        private double padding = 2;// 间距
        
        
        static CustomDrawingControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomDrawingControl), new FrameworkPropertyMetadata(typeof(CustomDrawingControl)));
        }
        public string[] StringsToDraw { get; set; }
        // 按下鼠标拖动、滚轮上下滑动、shift+滚轮左右滑动；ctrl+滚轮缩放
        private Point startPoint, nowPoint, nowWheelPoint;// 按下坐标，当前坐标
        private bool isDragging = false;// 左键是否按下
        private double offsetX = 0, offsetY = 0, scale = 1;// x偏移，y偏移，缩放
        private double offsetNowX = 0, offsetNowY = 0, scaleD = 0;
        // 将 DrawingVisual 声明为成员变量
        private DrawingVisual _drawingVisual = new DrawingVisual();
        public CustomDrawingControl()
        {
            StringsToDraw = new string[] { }; // 初始化为空数组
            // 注册鼠标事件处理程序
            this.MouseDown += CustomDrawingControl_MouseDown;
            this.MouseMove += CustomDrawingControl_MouseMove;
            this.MouseUp += CustomDrawingControl_MouseUp;
            this.MouseWheel += CustomDrawingControl_MouseWheel;
            // 设置背景，以便在空白区域也能接收鼠标事件
            this.Background = Brushes.White;
        }
        protected override void OnRender(DrawingContext drawingContext2)
        {
            base.OnRender(drawingContext2);
            using (var drawingContext = _drawingVisual.RenderOpen())
            {
                // 变换规则
                drawingContext.PushTransform(new TranslateTransform(offsetX + offsetNowX, offsetY + offsetNowY));
                drawingContext.PushTransform(new ScaleTransform(scale, scale, 0, 0));
                // 调整缩放中心
                var dx = nowWheelPoint.X - offsetX - offsetNowX;
                var dy = nowWheelPoint.Y - offsetY - offsetNowY;
                dx = dx / scale - dx / (scale - scaleD);
                dy = dy / scale - dy / (scale - scaleD);
                drawingContext.PushTransform(new TranslateTransform(dx, dy));
                offsetX += dx * scale;
                offsetY += dy * scale;
                scaleD = 0;

                // 设置字体、画笔等
                var font = new Typeface("Arial");
                var brush = Brushes.Black; // 字体颜色
                var pen = new Pen(Brushes.Red, 2); // 矩形边框颜色和粗细

                if(StringsToDraw!=null)
                    for (int i = 0; i < StringsToDraw.Length; i++)
                    {
                        var str = StringsToDraw[i];
                        // 创建格式化文本
                        var formattedText = new FormattedText(
                            str, System.Globalization.CultureInfo.CurrentCulture,
                            FlowDirection.LeftToRight, font, 16, brush, 1.0
                        );
                        // 字符串宽高
                        double textWidth = formattedText.Width;
                        double textHeight = formattedText.Height;
                        // 起点坐标
                        double x = padding + padding;
                        double y = padding + textHeight * i + padding * 4 * i;

                        drawingContext.DrawText(formattedText, new Point(x + padding, y + padding)); // 绘制文本、
                        drawingContext.DrawRectangle(null, pen, new Rect(x, y, textWidth + padding * 2, textHeight + padding * 2));// 绘制边框
                    }
            }
            // 这里直接画drawingvisual，不会重新绘制
            drawingContext2.DrawDrawing(_drawingVisual.Drawing);
        }
        private void CustomDrawingControl_MouseDown(object sender, MouseButtonEventArgs e)// 鼠标按下事件处理程序
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.CaptureMouse();// 捕获鼠标
                startPoint = e.GetPosition(this);// 记录起始点
                isDragging = true;// 设置拖动状态为 true
            }
        }
        private void CustomDrawingControl_MouseMove(object sender, MouseEventArgs e)// 鼠标移动事件处理程序
        {
            if (isDragging)
            {
                nowPoint = e.GetPosition(this);
                // 计算偏移量
                offsetNowX = nowPoint.X - startPoint.X;
                offsetNowY = nowPoint.Y - startPoint.Y;
                InvalidateVisual(); // 强制重绘
            }
        }
        private void CustomDrawingControl_MouseUp(object sender, MouseButtonEventArgs e)// 鼠标抬起事件处理程序
        {
            if (isDragging)
            {
                this.ReleaseMouseCapture();// 释放鼠标捕获
                isDragging = false;// 设置拖动状态为 false
                offsetX = offsetX + offsetNowX;
                offsetY = offsetY + offsetNowY;
                offsetNowX = 0;
                offsetNowY = 0;
            }
        }
        private void CustomDrawingControl_MouseWheel(object sender, MouseWheelEventArgs e) // 鼠标滚轮事件处理程序
        {
            if (Keyboard.Modifiers == ModifierKeys.Control) // Ctrl + 滚轮，缩放
            {
                nowWheelPoint = e.GetPosition(this);
                // 计算新的缩放比例
                scale += e.Delta * 0.0005;
                scaleD = e.Delta * 0.0005;
                if (scale < 0.0001) 
                {
                    scale -= e.Delta * 0.0005;
                    scaleD = 0;
                }
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift) // Shift + 滚轮，左右滚动
            {
                offsetX += e.Delta * 0.2;
            }
            else // 默认情况下，上下滚动
            {
                offsetY += e.Delta * 0.2;
            }
            InvalidateVisual(); // 强制重绘
        }
        // 重置偏移缩放
        public void ResetOffset()
        {
            offsetX = 0;
            offsetY = 0;
            scale = 1.0;
            InvalidateVisual();
        }

        // **图片保存相关
        private String keySavePath = "LastSavePath";
        private String keySaveName = "LastSaveName";
        public void SaveToPngWithDialog()
        {
            Dispatcher.Invoke(() =>
            {
                String name = "img_" + SomeDeal.GetCurrentDateTimeFormatted() + ".png";
                if (!MainWindow.isUpdateName)
                {
                    name = MainWindow._configManager.GetString(keySaveName, name);
                }
                var saveDialog = new SaveFileDialog
                {
                    InitialDirectory = MainWindow._configManager.GetString(keySavePath, MainWindow.defaultSaveDir),
                    Filter = "PNG Image (*.png)|*.png",
                    FileName = name,
                    AddExtension = true
                };
                if (saveDialog.ShowDialog() == true)
                {
                    var filePath = saveDialog.FileName;
                    SaveToPng(filePath);
                    MainWindow._configManager.SaveString(keySavePath, System.IO.Path.GetDirectoryName(filePath));
                    MainWindow._configManager.SaveString(keySaveName, System.IO.Path.GetFileName(filePath));
                }
            });
        }
        public void SaveToPng(string filePath)
        {
            // 3. 测量内容范围
            var bounds = _drawingVisual.ContentBounds;
            // 4. 创建 RenderTargetBitmap
            var renderTargetBitmap = new RenderTargetBitmap(
                (int)Math.Ceiling(bounds.Width),
                (int)Math.Ceiling(bounds.Height),
                96, 96, PixelFormats.Pbgra32);

            // 3. 创建一个新的 DrawingVisual，用于应用临时的平移变换
            var saveVisual = new DrawingVisual();
            using (var saveDc = saveVisual.RenderOpen())
            {
                // 4. 应用一个临时的平移变换，将内容平移回原点 (0, 0)
                saveDc.PushTransform(new TranslateTransform(-bounds.X, -bounds.Y));
                // 5. 将 _drawingVisual 绘制到 saveVisual 的 DrawingContext 上
                saveDc.DrawDrawing(_drawingVisual.Drawing);
                // 6. 弹出临时的平移变换
                saveDc.Pop();
            }
            // 7. 将 saveVisual 渲染到 RenderTargetBitmap
            renderTargetBitmap.Render(saveVisual);
            
            // 5. 编码为 PNG
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            // 6. 保存文件
            try
            {
                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    encoder.Save(fs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存图像失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}