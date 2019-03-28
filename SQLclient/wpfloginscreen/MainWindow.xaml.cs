using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Configuration;
using wpfloginscreen.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Controls;

namespace wpfloginscreen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string connectionString;

        List<Experiment> experiments = new List<Experiment>();

        List<TextBox> yEtalonList = new List<TextBox>();

        List<TextBox> x = new List<TextBox>();

        List<TextBox> yPlanList = new List<TextBox>();

        Experiment currentExperiment;

        User currentUser;

        bool edit = false;

        public MainWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            yEtalonList.Add(y0);
            yEtalonList.Add(y1);
            yEtalonList.Add(y2);
            yEtalonList.Add(y3);
            yEtalonList.Add(y4);
            yEtalonList.Add(y5);
            yEtalonList.Add(y6);
            yEtalonList.Add(y7);
            yEtalonList.Add(y8);
            yEtalonList.Add(y9);


            x.Add(x0);
            x.Add(x1);
            x.Add(x2);
            x.Add(x3);
            x.Add(x4);
            x.Add(x5);
            x.Add(x6);
            x.Add(x7);
            x.Add(x8);
            x.Add(x9);

            yPlanList.Add(py0);
            yPlanList.Add(py1);
            yPlanList.Add(py2);
            yPlanList.Add(py3);
            yPlanList.Add(py4);
            yPlanList.Add(py5);
            yPlanList.Add(py6);
            yPlanList.Add(py7);
            yPlanList.Add(py8);
            yPlanList.Add(py9);

        }

        public void InitUser(string user)
        {
            currentUser = new User(user);
            UserNameTextBox.Text = currentUser.Name;
            UserTypeTextBox.Text = currentUser.Type;

            switch(currentUser.Type)
            {
               
                case "Администратор":
                    TextFormat.IsEnabled = true;
                    break;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Experiment.FullTreeView(experimentsTree,experiments, Select_Item_Click);
        }

        private void Select_Item_Click(object sender, RoutedEventArgs e)
        {
            edit = true;

            if (experimentsTree.SelectedItem is TreeViewItem)
            {

                var currentItemHeader = (experimentsTree.SelectedItem as TreeViewItem).Header as string;

                if (currentItemHeader != "Многочлены" && currentItemHeader != "Функции")
                {


                    TextFormat.Text = currentItemHeader;
                    currentExperiment = Experiment.Get(experiments, currentItemHeader);
                    ExperimentDescription.Text = currentExperiment.Description;
                    // MessageBox.Show(currentExperiment.EtalonID.ToString());

                    for (int i = 0; i < 10; i++)
                    {
                        yEtalonList[i].Text = currentExperiment.yEtalon[i].ToString();
                        x[i].Text = i.ToString();   
                        yPlanList[i].Text = currentExperiment.yPlan[i].ToString();                   
                    }

                    
            


            




                }
            }

            edit = false;
        }

       private void experimentsTree_MouseRightButtonDown(object sender, RoutedEventArgs e)
        {
           
        }

        private void TextFormat_TouchEnter(object sender, RoutedEventArgs e)
        {
            
        }

        private void TextFormat_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var currentItemHeader = (experimentsTree.SelectedItem as TreeViewItem).Header as string;

            string newformat = TextFormat.Text;

            if (currentItemHeader != "Многочлены" && currentItemHeader != "Функции")
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    

                    if (newformat != currentExperiment.TextForm)
                    {


                        using (var sqlConnection = new SqlConnection(connectionString))
                        {
                            if (sqlConnection.State == ConnectionState.Closed)
                                sqlConnection.Open();

                            var sqlExpression = @"sp_ChangeTextFormaFunction";
                            var sqlCmd = new SqlCommand(sqlExpression, sqlConnection);

                            sqlCmd.CommandType = CommandType.StoredProcedure;

                            var idParam = new SqlParameter
                            {
                                ParameterName = "@idfunc",
                                SqlDbType = SqlDbType.Int,
                                Value = currentExperiment.EtalonID
                            };

                            sqlCmd.Parameters.Add(idParam);

                            var textFormParam = new SqlParameter
                            {
                                ParameterName = "@textformat",
                                SqlDbType = SqlDbType.VarChar,
                                Size = 50,
                                Value = newformat
                            };

                            sqlCmd.Parameters.Add(textFormParam);

                            sqlCmd.ExecuteNonQuery();

                            experiments[Experiment.IndexOf(experiments, currentExperiment.TextForm)].TextForm = newformat;

                            currentExperiment.TextForm = newformat;


                            

                            ((experimentsTree.SelectedItem as TreeViewItem).Header) = newformat;
                           currentItemHeader = (experimentsTree.SelectedItem as TreeViewItem).Header as string;
                        }
                    }
                }
                }
                
            

        }

        private void  ChangeUser(object sender, RoutedEventArgs e)
        {
            var ls = new LoginScreen();
            ls.InitializeComponent();
            ls.Show();

            Close();
        }





      
        private void Help(object sender, RoutedEventArgs e)
        {
            var info = @"Программа предназначена для демонстрации результатов дипломной работы по теме:
Статистическое моделирование и анализ результатов реализации эксперимента при ортогональном планировании второго порядка";

            MessageBox.Show(info, "О программе", MessageBoxButton.OKCancel);
        }
        private void AboutProgram_Click(object sender, RoutedEventArgs e)
        {
            var info = @"Данная программа предназначена исключительно для просмтора и сохранения пользователями результатов экспериментов из базы данных
Её функционал следующий :
 •Вывод эталонной функции
 •Вывод типа эталонной функции
 •Вывод значений эталонной функции в точках, на которых проводится эксперимент
 •Вывод функции, построенной по плану в точках, на которых проводится эксперимент
 •Иерархическое представление данных об экспериментах [будет добавлено]
 •Вывод графика , на котором видна эталонная функция и полученные экспериментом точки";

            MessageBox.Show(info, "О программе", MessageBoxButton.OKCancel);
        }

        private void AboutProject_Click(object sender, RoutedEventArgs e)
        {
            var info = @"Данный проект представляет собой комплекс программ и связывающих их модулей , предназначенных для моделирования численных экспериментов, реализующих ортогональные планы второго порядка  на ЭВМ. Проект можно подразделить на две условные части :
•	Приложения , предназначенные для эксплуатации обычными пользователями.
•	Приложение , предназначенное для администрирования базы данных проекта .
В первый блок входит программное обеспечение , которым может воспользоваться любой  желающий. С помощью них можно посмотреть сохраненные на сервере планы экспериментов , результаты  их реализации , а также сравнить эти результаты с эталонным значением функции двумя способами : 
•	Формальным перечислением модулей разности точек соответственно эталонной функции и модели , построенной экспериментом ( разность от функций в соответствующих точках).
•	Графически : в специально отведенной для этой цели области окна приложения графически отображается эталонная функция (сечение при размерности n>2) и результат реализации эксперимента в области, содержащей точки, полученные моделированием эксперимента.
Во второй блок входит приложение , предназначенное для пользования администратором ( студентом или его научным руководителем). В функционал данного модуля входит следующее:
•	Возможность добавлять новые планы, редактировать и удалять имеющиеся.
•	Возможность редактирования структуры базы данных.
•	Возможность предоставления право на модификацию планов другим пользователям.
Также проект включает в себя серверную часть : приложение , написанное на c# , которое взаимодействует с приложениями типа “client” по сетевому протоколу  транспортного уровня TCP.
";
            MessageBox.Show(info, "О проекте", MessageBoxButton.OKCancel);
        }

        private void AboutRazrab_Click(object sender, RoutedEventArgs e)
        {
            var info = @"НИУ МАИ Факультет 8 Кафедра 813
Студент : Щербаков Василий Сергеевич
Группа : 8-3ПМИ-4ДБ-033-15
Научный руководитель : к.х.н. Любецкая Светлана Николаевна";

            MessageBox.Show(info, "Разработчики", MessageBoxButton.OKCancel);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
           // var exp = new Experiment();

//            exp.Update();

            var saveFileDialog = new System.Windows.Forms.SaveFileDialog
            {
                Filter = "Xml files(*.xml)|*.xml"
            };
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                MessageBox.Show("You should choose file", "Error");
                return;
            }

            XmlSerializer formatter;

            using (var fs = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate))
            {
                try
                {
                    formatter = new XmlSerializer(typeof(Experiment));
                    formatter.Serialize(fs, currentExperiment);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


            }
            }

        

        }
       
    }
    
