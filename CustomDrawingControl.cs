using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace 简单关系图_测试_
{
    
    public class CustomDrawingControl : Control
    {
        static CustomDrawingControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomDrawingControl), new FrameworkPropertyMetadata(typeof(CustomDrawingControl)));
        }

        public string[] StringsToDraw { get; set; }

        // 按下鼠标拖动、滚轮上下滑动、shift+滚轮左右滑动；ctrl+滚轮缩放
        // 按下坐标，当前坐标
        private Point startPoint, nowPoint, nowWheelPoint;
        // 左键是否按下，Ctrl是否按下，Shift是否按下
        private bool isDragging = false, isCtrl = false, isShift = false;
        // x偏移，y偏移，缩放
        private double offsetX = 0, offsetY = 0, scale = 1;
        private double offsetNowX = 0, offsetNowY = 0, scaleD = 0;

        private const int GridSpacing = 20; // ***网格间距
        private const int TextMargin = 5;


        public CustomDrawingControl()
        {
            StringsToDraw = new string[] { }; // 初始化为空数组

            // 注册鼠标事件处理程序
            this.MouseDown += CustomDrawingControl_MouseDown;
            this.MouseMove += CustomDrawingControl_MouseMove;
            this.MouseUp += CustomDrawingControl_MouseUp;
            this.MouseWheel += CustomDrawingControl_MouseWheel; // 添加滚轮事件

            // 设置背景为透明，以便在空白区域也能接收鼠标事件
            this.Background = Brushes.Transparent;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            // 设置字体、画笔等
            var font = new Typeface("Arial");
            var brush = Brushes.Black; // 字体颜色
            var pen = new Pen(Brushes.Red, 2); // 矩形边框颜色和粗细

            var padding = 2;//间距

           

            // *变换规则
            drawingContext.PushTransform(new TranslateTransform(offsetX + offsetNowX, offsetY + offsetNowY));
            drawingContext.PushTransform(new ScaleTransform(scale, scale, 0, 0));
            
           
            // 设置画笔和字体
            var gridPen = new Pen(Brushes.LightGray, 1);
            var textBrush = Brushes.Black;
            var textFont = new Typeface("Arial");
            var fontSize = 10;

            // ***绘制滚轮滚动后鼠标在画布的实际坐标
            var dx = nowWheelPoint.X;
            var dy = nowWheelPoint.Y;
            dx = (dx - offsetX - offsetNowX) / scale;
            dy = (dy - offsetY - offsetNowY) / scale;
            //drawingContext.DrawEllipse(Brushes.Red, new Pen(Brushes.Red, 1), new Point(dx, dy), 2, 2);

            // ***绘制滚轮滚动前鼠标在画布的实际坐标
            var dx2 = nowWheelPoint.X;
            var dy2 = nowWheelPoint.Y;
            dx2 = (dx2 - offsetX - offsetNowX) / (scale - scaleD);
            dy2 = (dy2 - offsetY - offsetNowY) / (scale - scaleD);
            //drawingContext.DrawEllipse(Brushes.Blue, new Pen(Brushes.Blue, 1), new Point(dx2, dy2), 2, 2);

            var dx3 = dx - dx2;
            var dy3 = dy - dy2;

            drawingContext.PushTransform(new TranslateTransform(dx3, dy3));
            offsetX += dx3 * scale;
            offsetY += dy3 * scale;

            var formattedText2 = new FormattedText(
                    "dx=" + Math.Round(dx) + ",dy=" + Math.Round(dy) + "\ndx2=" + Math.Round(dx2) + ",dy2=" + Math.Round(dy2),
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    textFont,
                    fontSize,
                    textBrush,
                    1
                );
            var textPoint2 = new Point(dx, dy - 25);
            //drawingContext.DrawText(formattedText2, textPoint2);

            scaleD = 0;

            // *** 网格
            // 获取控件的宽高
            var width = this.ActualWidth;
            var height = this.ActualHeight;

            if (width == 0 || height == 0) return;

            // 绘制横向网格线和文本
            for (int y = 0; y <= (int)(height / GridSpacing); y++)
            {
                var yPos = y * GridSpacing;
                drawingContext.DrawLine(gridPen, new Point(0, yPos), new Point(width, yPos));

                // 绘制横向坐标文本
                var formattedText = new FormattedText(
                    yPos.ToString(),
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    textFont,
                    fontSize,
                    textBrush,
                    1
                );
                var textPoint = new Point(TextMargin, yPos - formattedText.Height / 2);
                drawingContext.DrawText(formattedText, textPoint);
            }

            // 绘制纵向网格线和文本
            for (int x = 0; x <= (int)(width / GridSpacing); x++)
            {
                var xPos = x * GridSpacing;
                drawingContext.DrawLine(gridPen, new Point(xPos, 0), new Point(xPos, height));

                // 绘制纵向坐标文本
                var formattedText = new FormattedText(
                   xPos.ToString(),
                   CultureInfo.CurrentCulture,
                   FlowDirection.LeftToRight,
                   textFont,
                   fontSize,
                   textBrush,
                   1
               );
                var textPoint = new Point(xPos - formattedText.Width / 2, height - formattedText.Height - TextMargin);
                drawingContext.DrawText(formattedText, textPoint);
            }


            for (int i = 0; i < StringsToDraw.Length; i++)
            {
                var str = StringsToDraw[i];
                // 创建格式化文本
                var formattedText = new FormattedText(
                    str,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    font,
                    16,
                    brush,
                    1.0
                );
                // 字符串宽高
                double textWidth = formattedText.Width;
                double textHeight = formattedText.Height;
                // 起点坐标
                double x = padding + padding;
                double y = padding + textHeight * i + padding * 4 * i;

                drawingContext.DrawText(formattedText, new Point(x + padding, y + padding)); // 绘制文本

                drawingContext.DrawRectangle(
                    null, // 矩形内部为空
                    pen, // 使用红色边框
                    new Rect(x, y, textWidth + padding * 2, textHeight + padding * 2) // 矩形的位置和大小
                );

            }
            // *移除变换
            // drawingContext.Pop();
            

            // ++获取主窗口，并转换为 MainWindow 类型
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                // 将鼠标坐标信息写入到主窗口的 OutputTextBox
                mainWindow.OutputTextBox.Text = $"Mouse X: {Math.Floor(nowPoint.X)}, Y: {Math.Floor(nowPoint.Y)}\n" +
                    $"Offset X: {Math.Floor(offsetX + offsetNowX)}, Y: {Math.Floor(offsetY + offsetNowY)}\n" +
                    $"Scale: {scale}";
            }
        }

        // 鼠标按下事件处理程序
        private void CustomDrawingControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // 捕获鼠标
                this.CaptureMouse();
                // 记录起始点
                startPoint = e.GetPosition(this);
                // 设置拖动状态为 true
                isDragging = true;
            }
        }

        // 鼠标移动事件处理程序
        private void CustomDrawingControl_MouseMove(object sender, MouseEventArgs e)
        {
            nowPoint = e.GetPosition(this);
            // ++获取主窗口，并转换为 MainWindow 类型
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                // 将鼠标坐标信息写入到主窗口的 OutputTextBox
                mainWindow.OutputTextBox.Text = $"Mouse X: {Math.Floor(nowPoint.X)}, Y: {Math.Floor(nowPoint.Y)}\n" +
                    $"Offset X: {Math.Floor(offsetX + offsetNowX)}, Y: {Math.Floor(offsetY + offsetNowY)}\n" +
                    $"Scale: {scale}";
            }
            if (isDragging)
            {
                // 计算偏移量
                offsetNowX = nowPoint.X - startPoint.X;
                offsetNowY = nowPoint.Y - startPoint.Y;
                InvalidateVisual(); // 强制重绘
            }
        }

        // 鼠标抬起事件处理程序
        private void CustomDrawingControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                // 释放鼠标捕获
                this.ReleaseMouseCapture();
                // 设置拖动状态为 false
                isDragging = false;

                offsetX = offsetX + offsetNowX;
                offsetY = offsetY + offsetNowY;
                offsetNowX = 0;
                offsetNowY = 0;
            }
        }

        // 鼠标滚轮事件处理程序
        private void CustomDrawingControl_MouseWheel(object sender, MouseWheelEventArgs e)
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


        // 重置偏移量
        public void ResetOffset()
        {
            offsetX = 0;
            offsetY = 0;
            scale = 1.0;
            InvalidateVisual();
        }
    }

}