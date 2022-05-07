using System.Windows;
using Cerberus.view;

namespace Cerberus.controller
{
    public partial class AuthenticationWindow : Window
    {
        public AuthenticationWindow()
        {
            InitializeComponent();
            QuestionArea.Text = Session.AuthenticationRequest();
        }
        
        private void Accept(object sender, RoutedEventArgs e)
        {
            if (Session.AuthenticationResponse(AnswerArea.Text))
            {
                ((ControlWindow) Owner).ActionButton.Visibility = Visibility.Visible;
                if (Session.IsAdmin())
                {
                    ((ControlWindow) Owner).AdminButton.Visibility = Visibility.Visible;
                }
            }
            Close();
        }
    }
}