using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public LoginPage()
        {
            InitializeComponent();
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            int id;
            if (int.TryParse(IdTextBox.Text.Trim(), out id))
            {
                string password = PasswordBox.Password;
          
         

            try
            {
                // נניח שהפונקציה זמינה דרך מופע של BL
                BO.Role role = s_bl.Volunteer.EnteredSystem(id, password);

                switch (role)
                {
                    case BO.Role.volunteer:
                        new VolunteerWindow().Show();
                        break;

                    case BO.Role.manager:
                        MessageBoxResult result = MessageBox.Show(
                            "האם ברצונך להיכנס למסך ניהול ראשי?",
                            "כניסת מנהל",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question
                        );

                        if (result == MessageBoxResult.Yes)
                            new MainAdminWindow().Show();

                        else
                            new VolunteerWindow().Show();

                        break;

                    default:
                        MessageBox.Show("הרשאה לא מזוהה. פנה למנהל המערכת.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                }
                    this.Close();
                }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("סיסמה שגויה או תעודת זהות לא קיימת.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("אירעה שגיאה: " + ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            }
            else
            {
                MessageBox.Show("יש להזין מזהה מספרי תקין.");
                return;
            }
        }

    }
}
