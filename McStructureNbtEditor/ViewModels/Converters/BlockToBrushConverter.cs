using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace McStructureNbtEditor.ViewModels.Converters
{
    public class BlockToBrushConverter : IValueConverter
    {
        public Brush AirBrush { get; set; } = Brushes.WhiteSmoke;
        public double Saturation { get; set; } = 0.3;
        public double Value { get; set; } = 0.90;

        private static readonly DrawingBrush CachedVoidBrush;

        private const string AIR_BLOCK = "minecraft:air";

        static BlockToBrushConverter()
        {
            DrawingGroup drawingGroup = new DrawingGroup();

            drawingGroup.Children.Add(new GeometryDrawing(
                Brushes.White,
                null,
                new RectangleGeometry(new Rect(0, 0, 32, 32))
            ));

            Pen redPen = new Pen(Brushes.Red, 1);

            drawingGroup.Children.Add(new GeometryDrawing(
                null,
                redPen,
                new LineGeometry(new Point(0, 0), new Point(32, 32))
            ));
            drawingGroup.Children.Add(new GeometryDrawing(
                null,
                redPen,
                new LineGeometry(new Point(32, 0), new Point(0, 32))
            ));

            CachedVoidBrush = new DrawingBrush(drawingGroup)
            {
                Stretch = Stretch.Fill
            };

            CachedVoidBrush.Freeze();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string blockName || string.IsNullOrWhiteSpace(blockName))
                return CachedVoidBrush;
            if (blockName == AIR_BLOCK)
                return AirBrush;

            var rawBlockName = GetRawBlockName(blockName);

            char c = GetFirstChar(rawBlockName);
            if (c == '\0')
                return CachedVoidBrush;
            c = char.ToUpper(c);
            if (c < 'A' || c > 'Z')
                return CachedVoidBrush;

            c = char.ToUpperInvariant(c);

            int index = c - 'A';

            double hue = 300.0 * index / 25.0;

            Color color = ColorFromHsv(hue, Saturation, Value);
            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static string GetRawBlockName(string blockName)
        {
            int colonIndex = blockName.LastIndexOf(':');
            string target = colonIndex >= 0 && colonIndex < blockName.Length - 1
                ? blockName[(colonIndex + 1)..]
                : blockName;
            return target;
        }

        private static char GetFirstChar(string rawBlockName)
        {
            foreach (char ch in rawBlockName)
            {
                if (char.IsLetter(ch))
                    return ch;
            }

            return '\0';
        }

        private static Color ColorFromHsv(double hue, double saturation, double value)
        {
            hue = hue % 360;
            if (hue < 0)
                hue += 360;

            double c = value * saturation;
            double x = c * (1 - Math.Abs((hue / 60.0) % 2 - 1));
            double m = value - c;

            double rPrime, gPrime, bPrime;

            if (hue < 60)
            {
                rPrime = c; gPrime = x; bPrime = 0;
            }
            else if (hue < 120)
            {
                rPrime = x; gPrime = c; bPrime = 0;
            }
            else if (hue < 180)
            {
                rPrime = 0; gPrime = c; bPrime = x;
            }
            else if (hue < 240)
            {
                rPrime = 0; gPrime = x; bPrime = c;
            }
            else if (hue < 300)
            {
                rPrime = x; gPrime = 0; bPrime = c;
            }
            else
            {
                rPrime = c; gPrime = 0; bPrime = x;
            }

            byte r = (byte)Math.Round((rPrime + m) * 255);
            byte g = (byte)Math.Round((gPrime + m) * 255);
            byte b = (byte)Math.Round((bPrime + m) * 255);

            return Color.FromRgb(r, g, b);
        }
    }
}