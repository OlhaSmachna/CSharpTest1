using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Wpf.DataTemplates.WithoutDataTemplates;

namespace CSharpTest1
{
    public partial class MainWindow : Window
    {
        Regex numRegex;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new DataSource();
            //numRegex = new Regex("^([^.0-][0-9]+|[0])$");
            numRegex = new Regex("^([^.0-][0-9]{0,2}|[0])$");
        }

        // Тут відбувається обробка подій textbox, щоб запобігти вводу невалідних даних
        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !numRegex.IsMatch((sender as TextBox).Text + e.Text);
        }
        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!numRegex.IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}