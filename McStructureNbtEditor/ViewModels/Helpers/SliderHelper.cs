using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace McStructureNbtEditor.ViewModels.Helpers
{
    public static class SliderHelper
    {
        public static readonly DependencyProperty EnableMouseWheelProperty =
            DependencyProperty.RegisterAttached("EnableMouseWheel", typeof(bool), typeof(SliderHelper), new PropertyMetadata(false, OnEnableMouseWheelChanged));

        public static void SetEnableMouseWheel(DependencyObject element, bool value) => element.SetValue(EnableMouseWheelProperty, value);
        public static bool GetEnableMouseWheel(DependencyObject element) => (bool)element.GetValue(EnableMouseWheelProperty);

        private static void OnEnableMouseWheelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Slider slider)
            {
                if ((bool)e.NewValue) slider.MouseWheel += Slider_MouseWheel;
                else slider.MouseWheel -= Slider_MouseWheel;
            }
        }

        private static void Slider_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var slider = (Slider)sender;
            double step = slider.TickFrequency > 0 ? slider.TickFrequency : 1;
            slider.Value += (e.Delta > 0) ? step : -step;
            e.Handled = true; // 이벤트가 부모 컨테이너로 전파되어 화면 전체가 스크롤되는 것을 방지
        }
    }
}
