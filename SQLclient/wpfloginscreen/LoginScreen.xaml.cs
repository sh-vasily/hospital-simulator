using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace wpfloginscreen
{
    /// <summary>
    /// Interaction logic for LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : Window
    {
        public LoginScreen()
        {
            InitializeComponent();
        }

        public static void Run()
        {
           
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            var username = txtUsername.Text;
            var passwd = txtPassword.Password;

             var sqlCon = new SqlConnection(@"Data Source=ВАСИЛИЙ-ПК\SQLEXPRESS; Initial Catalog=ProjectDB; Integrated Security=True;");
             try
             {
                 if (sqlCon.State == ConnectionState.Closed)
                     sqlCon.Open();
                var sqlExpression = @"sp_CheckPasswd";
                var sqlCmd = new SqlCommand(sqlExpression, sqlCon);
          
                sqlCmd.CommandType = CommandType.StoredProcedure;

                var nameParam = new SqlParameter
                {
                    ParameterName = "@username",
                    Value = username
                };
                sqlCmd.Parameters.Add(nameParam);

                var passwdParam = new SqlParameter
                {
                    ParameterName = "@passwd",
                    Value = passwd  
                };
                sqlCmd.Parameters.Add(passwdParam);

                var isCorrectParam = new SqlParameter
                {
                    ParameterName="@iscorrect",
                    SqlDbType = SqlDbType.Bit
                };

                isCorrectParam.Direction = ParameterDirection.Output;

                sqlCmd.Parameters.Add(isCorrectParam);

                sqlCmd.ExecuteNonQuery();

                var isCorrect = (bool)sqlCmd.Parameters["@iscorrect"].Value;

                 if (isCorrect)
                 {
                    var dashboard = new MainWindow();
                    dashboard.InitUser(username);
                    dashboard.Show();
                    this.Close();
                 }
                 else
                 {
                     MessageBox.Show("Username or password is incorrect.");
                 }
             }
             catch (Exception ex)
             {
                 MessageBox.Show(ex.Message);
             }
             finally
             {
                 sqlCon.Close();
             }  
        }
    }
}
