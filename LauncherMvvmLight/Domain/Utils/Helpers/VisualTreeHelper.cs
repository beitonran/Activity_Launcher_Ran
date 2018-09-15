using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LauncherMvvmLight.Domain.Utils.Helpers
{
    public static class VisualTreeHelper
    {
        private static DependencyObject GetDependencyObjectFromVisualTree(DependencyObject startObject, Type type)

        {

            //Walk the visual tree to get the parent(ItemsControl)

            //of this control

            DependencyObject parent = startObject;

            while (parent != null)
            {
                if (type.IsInstanceOfType(parent))
                    break;
                else
                    ;//parent = VisualTreeHelper.GetParent(parent);

            }

            return parent;

        }
    }
}
