using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChatClient
{
    static class ErrorControls
    {
        static public bool NotEmptyTextBox(TextBox tb)
        {
            bool check = true;
            if (tb.Text == "")
            {
                check = false;
                tb.Background = Brushes.Red;
            }
            else
            {
                tb.Background = (SolidColorBrush)tb.FindResource("LightBrush");
            }

            return check;
        }
    }
}
