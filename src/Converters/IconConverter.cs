using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using IgnoreFiles.Models;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

namespace IgnoreFiles.Converters
{
    public class IconConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 4 || !(values[0] is FileTreeModel) || !(values[1] is double) || !(values[2] is double) || !(values[3] is DependencyObject))
            {
                return null;
            }

            int x = (int)(double)values[1];
            int y = (int)(double)values[2];

            if (x < 1)
            {
                x = 1;
            }

            if (y < 1)
            {
                y = 1;
            }

            FileTreeModel model = (FileTreeModel)values[0];
            if (!model.IsFile)
            {
                bool isExpanded = model.IsExpanded;
                ImageMoniker moniker = isExpanded ? KnownMonikers.FolderOpened : KnownMonikers.FolderClosed;
                return WpfUtil.ThemeImage((DependencyObject)values[3], WpfUtil.GetIconForImageMoniker(moniker, x, y));
            }

            string name = model.Name;
            bool isThemeIcon;
            ImageSource source = WpfUtil.GetIconForFile((DependencyObject)values[3], name, out isThemeIcon);
            return source;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
