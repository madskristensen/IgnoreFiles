using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using IgnoreFiles.Models;

namespace IgnoreFiles.Converters
{
    public class ItemVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            FileTreeModel model = values[0] as FileTreeModel;
            Func<FileTreeModel, bool> predicate = values[1] as Func<FileTreeModel, bool>;
            bool result = predicate?.Invoke(model) ?? true;

            for (int i = 2; !result && i < values.Length; ++i)
            {
                predicate = values[i] as Func<FileTreeModel, bool>;

                if (predicate != null)
                {
                    result |= predicate(model);
                }
                else if (values[i] is bool)
                {
                    result |= (bool) values[i];
                }
                else
                {
                    break;
                }
            }

            return result ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}