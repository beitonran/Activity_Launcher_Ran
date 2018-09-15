using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace LauncherMvvmLight.Domain.Utils.Convertors
{ 
  public class DeviceStatusToTextDecorationsConverter: IValueConverter {

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (value is bool) {
        if ((bool)value) {
                    //TextDecorationCollection redStrikthroughTextDecoration = TextDecorations.Strikethrough.CloneCurrentValue();
                    //redStrikthroughTextDecoration[0].Pen = new Pen {Brush=Brushes.Red, Thickness = 3 };
                    //return redStrikthroughTextDecoration; 
                    return Brushes.BlueViolet;
                }
                return Brushes.Blue;
            }
            //return new TextDecorationCollection(); 
            return Brushes.Black;
        }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }
  }
}