﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace MahApps.Metro.Controls
{
    public abstract class FlyoutBase : ContentControl
    {
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(FlyoutBase), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, IsOpenedChanged));
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(Position), typeof(FlyoutBase), new PropertyMetadata(Position.Left, PositionChanged));

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public Position Position
        {
            get { return (Position)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        private static void IsOpenedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var flyout = (FlyoutBase)dependencyObject;
            flyout.IsOpenedChanged(e);
        }

        protected virtual void IsOpenedChanged(DependencyPropertyChangedEventArgs e)
        {
            VisualStateManager.GoToState(this, (bool)e.NewValue == false ? "Hide" : "Show", true);
        }

        private static void PositionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var flyout = (FlyoutBase)dependencyObject;
            flyout.PositionChanged(e);
            
        }

        protected virtual void PositionChanged(DependencyPropertyChangedEventArgs e)
        {
            
        }
    }

    [TemplatePart(Name = "PART_BackButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Header", Type = typeof(ContentPresenter))]
    public class Flyout : FlyoutBase
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(Flyout), new PropertyMetadata(default(string)));
        
        public static readonly DependencyProperty IsPinnableProperty = DependencyProperty.Register("IsPinnable", typeof(bool), typeof(Flyout), new PropertyMetadata(default(bool)));
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(Flyout));
        public static readonly DependencyProperty PeekProperty = DependencyProperty.Register("Peek", typeof (bool), typeof (Flyout), new FrameworkPropertyMetadata(default(bool)));
        public static readonly DependencyProperty PeekWidthProperty = DependencyProperty.Register("PeekWidth", typeof (int), typeof (Flyout), new PropertyMetadata(30, PeekWidthChanged));

        private static void PeekWidthChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            Flyout flyout = dependencyObject as Flyout;
            flyout.ApplyAnimation(flyout.Position);
        }

        public int PeekWidth
        {
            get { return (int) GetValue(PeekWidthProperty); }
            set { SetValue(PeekWidthProperty, value); }
        }

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public bool Peek
        {
            get { return (bool)GetValue(PeekProperty); }
            set { SetValue(PeekProperty, value); }
        }

        public bool IsPinnable
        {
            get { return (bool)GetValue(IsPinnableProperty); }
            set { SetValue(IsPinnableProperty, value); }
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        static Flyout()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Flyout), new FrameworkPropertyMetadata(typeof(Flyout)));
        }

        protected override void PositionChanged(DependencyPropertyChangedEventArgs e)
        {
            base.PositionChanged(e);
            ApplyAnimation((Position)e.NewValue);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Dispatcher.BeginInvoke(new Action<Position>(ApplyAnimation), DispatcherPriority.Loaded, Position);
            //ApplyAnimation(Position);
        }

        protected virtual void ApplyAnimation(Position position)
        {
            var root = (Grid)GetTemplateChild("root");
            if (root == null)
                return;

            var hideFrame = (EasingDoubleKeyFrame)GetTemplateChild("hideFrame");
            var showFrame = (EasingDoubleKeyFrame)GetTemplateChild("showFrame");

            if (hideFrame == null || showFrame == null)
                return;

            showFrame.Value = 0;
            root.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            if (position == Position.Right)
                HorizontalAlignment = HorizontalAlignment.Right;

            if (position == Position.Right)
            {
                hideFrame.Value = root.DesiredSize.Width - (Peek ? PeekWidth : 0);
                root.RenderTransform = new TranslateTransform(hideFrame.Value, 0);
            }
            else
            {
                hideFrame.Value = -root.DesiredSize.Width + (Peek ? PeekWidth : 0);
                root.RenderTransform = new TranslateTransform(hideFrame.Value, 0);
            }
        }
    }
}
