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
        private double offsetNowX = 0, offsetNowY = 0, scaleNow = 1;

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

            // 应用缩放变换
            drawingContext.PushTransform(new ScaleTransform(scale, scale, nowWheelPoint.X, nowWheelPoint.Y));


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
                double x = padding + offsetX + offsetNowX + padding;
                double y = padding + offsetY + offsetNowY + textHeight * i + padding * 4 * i;

                drawingContext.DrawText(formattedText, new Point(x + padding, y + padding)); // 绘制文本

                drawingContext.DrawRectangle(
                    null, // 矩形内部为空
                    pen, // 使用红色边框
                    new Rect(x, y, textWidth + padding * 2, textHeight + padding * 2) // 矩形的位置和大小
                );

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
            if (isDragging)
            {
                nowPoint = e.GetPosition(this);
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
                scale += e.Delta * 0.001;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift) // Shift + 滚轮，左右滚动
            {
                offsetX += e.Delta * 0.1;
            }
            else // 默认情况下，上下滚动
            {
                offsetY += e.Delta * 0.1;
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