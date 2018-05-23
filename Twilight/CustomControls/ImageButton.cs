using System.Windows.Controls;

namespace Twilight.CustomControls
{
    public class ImageButton : Button
    {
        private readonly Image _image = null;
        private readonly TextBlock _textBlock = null;

        public ImageButton()
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new System.Windows.Thickness(10)
            };


            _image = new Image();
            _image.Margin = new System.Windows.Thickness(0, 0, 10, 0);
            panel.Children.Add(_image);

            _textBlock = new TextBlock();
            panel.Children.Add(_textBlock);

            this.Content = panel;
        }

        // Properties
    }
}
