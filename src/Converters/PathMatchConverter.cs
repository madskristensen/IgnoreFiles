using System;
using System.Globalization;
using System.Windows.Data;
using IgnoreFiles.Models;

namespace IgnoreFiles.Converters
{
    public class PathMatchConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            FileTreeModel model = values[0] as FileTreeModel;
            Func<FileTreeModel, bool> predicate = values[1] as Func<FileTreeModel, bool>;
            return predicate?.Invoke(model) ?? true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
