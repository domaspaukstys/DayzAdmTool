using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace DayzServerManager
{
    public class ScrollViewerMouseHandled : ScrollViewer
    {
        public ScrollViewerMouseHandled()
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            DataContext = this;

            Loaded += (sender, args) => BindScaleToChild();
        }

        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register("Zoom", typeof(double),
            typeof(ScrollViewerMouseHandled), new PropertyMetadata(1.0));

        private Point? _lastDragPoint;
        private Point? _lastMousePositionOnTarget;
        private Point? _lastCenterPositionOnTarget;

        private FrameworkElement Child
        {
            get { return Content as FrameworkElement; }
        }

        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set
            {
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(ZoomProperty, Zoom, value));
                SetValue(ZoomProperty, value);
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (Child == null)
                return;

            _lastMousePositionOnTarget = Mouse.GetPosition(Child);

            if (e.Delta > 0 && Zoom <= 1.9)
            {
                Zoom += 0.1;
            }
            if (e.Delta < 0 && Zoom >= 0.2)
            {
                Zoom -= 0.1;
            }

            e.Handled = true;
        }

        private void BindScaleToChild()
        {
            if (Child == null)
                return;

            ScaleTransform scale = Child.LayoutTransform as ScaleTransform;
            if (scale == null)
            {
                scale = new ScaleTransform();
                Binding binding = new Binding();
                binding.Path = new PropertyPath("Zoom");
                BindingOperations.SetBinding(scale, ScaleTransform.ScaleXProperty, binding);
                BindingOperations.SetBinding(scale, ScaleTransform.ScaleYProperty, binding);
                Child.LayoutTransform = scale;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_lastDragPoint.HasValue)
            {
                Point posNow = e.GetPosition(this);

                double dX = posNow.X - _lastDragPoint.Value.X;
                double dY = posNow.Y - _lastDragPoint.Value.Y;

                _lastDragPoint = posNow;

                ScrollToHorizontalOffset(HorizontalOffset - dX);
                ScrollToVerticalOffset(VerticalOffset - dY);
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _lastDragPoint = e.GetPosition(this);
            Mouse.Capture(this);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
            _lastDragPoint = null;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (Child == null)
                return;

            _lastMousePositionOnTarget = null;
            Point centerOfViewport = new Point(ViewportWidth / 2,
                                         ViewportHeight / 2);
            _lastCenterPositionOnTarget = TranslatePoint(centerOfViewport, Child);
            base.OnMouseLeave(e);
        }

        protected override void OnScrollChanged(ScrollChangedEventArgs e)
        {
            if (Child == null)
                return;

            if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0)
            {
                Point? targetBefore = null;
                Point? targetNow = null;

                if (!_lastMousePositionOnTarget.HasValue)
                {
                    if (_lastCenterPositionOnTarget.HasValue)
                    {
                        var centerOfViewport = new Point(ViewportWidth / 2,
                                                         ViewportHeight / 2);
                        Point centerOfTargetNow =
                              TranslatePoint(centerOfViewport, Child);

                        targetBefore = _lastCenterPositionOnTarget;
                        targetNow = centerOfTargetNow;
                    }
                }
                else
                {
                    targetBefore = _lastMousePositionOnTarget;
                    targetNow = Mouse.GetPosition(Child);

                    _lastMousePositionOnTarget = null;
                }

                if (targetBefore.HasValue)
                {
                    double dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
                    double dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;

                    double multiplicatorX = e.ExtentWidth / Child.Width;
                    double multiplicatorY = e.ExtentHeight / Child.Height;

                    double newOffsetX = HorizontalOffset -
                                        dXInTargetPixels * multiplicatorX;
                    double newOffsetY = VerticalOffset -
                                        dYInTargetPixels * multiplicatorY;

                    if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
                    {
                        return;
                    }

                    ScrollToHorizontalOffset(newOffsetX);
                    ScrollToVerticalOffset(newOffsetY);
                }
            }
        }
    }
}
