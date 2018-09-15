using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace LauncherMvvmLight.Domain.Utils.Helpers
{
    /// <summary>
    /// This a helper Extension Method that help us transforming a string like #ffffff to a Color instance
    /// </summary>
    public static class StylesHelpers
    {
        /// <summary>
        /// The styles helper5s
        /// </summary>
        /// <param name="colorString">The color string.</param>
        /// <returns></returns>
        /// Style defaultStyle = (Style)FindResource("MyTestStyle");
        //call sample: string name = ResourceHelper.FindNameFromResource(this.Resources, defaultStyle);

        public static class ResourceHelper
        {
            static public string FindNameFromResource(ResourceDictionary dictionary, object resourceItem)
            {
                foreach (object key in dictionary.Keys)
                {
                    if (dictionary[key] == resourceItem)
                    {
                        return key.ToString();
                    }
                }

                return null;
            }
        }

        //The default style of the document you could obtain through the Style property of RadDocument:
        //StyleDefinition defaultDocumentStyle = this.radRichTextBox.Document.Style;
        //You could check the style of the Span in which the caret is currently positioned:
        //StyleDefinition currentSpanStyle = this.radRichTextBox.Document.CaretPosition.GetCurrentSpanBox().AssociatedDocumentElement.Style;
        //All the styles that are used in the document could be accessed through the StyleRepository.The next snippet demonstrates how you could obtain the StyleDefinition for the Normal style:
        //StyleDefinition normalStyle = this.radRichTextBox.Document.StyleRepository[RadDocumentDefaultStyles.NormalStyleName];
    }
}
