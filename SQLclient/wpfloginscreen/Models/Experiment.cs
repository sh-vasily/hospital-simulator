using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace wpfloginscreen.Models
{
    [Serializable]
    public class Experiment
    {

       #region Скрытые поля

        private List<double> xEtalon = new List<double>();
        public List<double> yEtalon = new List<double>();

        private List<double> xPlan = new List<double>();
        public List<double> yPlan = new List<double>();
        
        private string textForm;

        private DateTime date;

        private string description;

        private string typePlan;

        private string typeFunction;

        private int etalonId;

        private int planId;
        #endregion

        #region Свойства

        public string Description
        {
            get => description;
            set => description = value;
        }

        public string TextForm
        {
            get => textForm;
            set => textForm = value;
        }

        public string TypeFunction
        {
            get => typeFunction;
            set => typeFunction = value;
        }

        public int EtalonID
        {
            get =>etalonId;
            set =>etalonId=value;
        }

        public int PlanID
        {
            get => planId;
            set => planId = value;
        }

        #endregion

        #region Статические методы
        public static Experiment Get(List<Experiment> exps, string func)
        {
            return exps.Select(p => p).Where(p => p.TextForm == func).First();
        }

        public static void FullTreeView(TreeView treeView , List<Experiment> exps, RoutedEventHandler routedEventHandler)
        {
            
            var mainItem = new TreeViewItem { Header = "Многочлены"};

            var sqlQuery = @"SELECT TextFormat , exper.TextDescription, etalon.ID, exper.PlansFunction_ID
                             FROM EtalonFunction etalon, Experiment exper
                             WHERE exper.EtalonFunction_ID = etalon.ID";

            using (var sqlConnection = new SqlConnection(@"Data Source =ВАСИЛИЙ-ПК\SQLEXPRESS; Initial Catalog =ProjectDB; Integrated Security = True;"))
            {
                sqlConnection.Open();

                var sqlDataAdapter = new SqlDataAdapter(sqlQuery, sqlConnection);

                var dataSet = new DataSet();

                sqlDataAdapter.Fill(dataSet);

                foreach (DataTable data in dataSet.Tables)
                {
                    foreach (DataRow row in data.Rows)
                    {
                        // получаем все ячейки строки
                        var cells = row.ItemArray;

                        var tempExp = new Experiment
                        {
                            textForm = cells[0] as string,
                            description = cells[1] as string,
                            etalonId = (int)cells[2],
                            planId = (int)cells[3]
                        };

                         exps.Add(tempExp);

                        mainItem.Items.Add(new TreeViewItem { Header = cells[0]  });
                     
                    }
                }

            }

            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            foreach (var exp in exps)
            {
                using (var sqlConnection = new SqlConnection(connectionString))
                {
                    if (sqlConnection.State == ConnectionState.Closed)
                        sqlConnection.Open();

                    var sqlExpression = @"sp_GetPoints";
                    var sqlCmd = new SqlCommand(sqlExpression, sqlConnection);

                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    var nameParam = new SqlParameter
                    {
                        ParameterName = "@idfunc",
                        SqlDbType = SqlDbType.Int,
                        Value = exp.etalonId
                    };

                    sqlCmd.Parameters.Add(nameParam);
                    sqlCmd.ExecuteNonQuery();


                    var sqlDataAdapter = new SqlDataAdapter(sqlCmd);

                    var dataSet = new DataSet();

                    sqlDataAdapter.Fill(dataSet);

                    foreach (DataTable data in dataSet.Tables)
                    {
                        foreach (DataRow row in data.Rows)
                        {
                            // получаем все ячейки строки
                            var cells = row.ItemArray;
                            exp.yEtalon.Add((double)cells[0]);
                        }
                    }




                    sqlConnection.Close();
                    sqlConnection.Open();

                    sqlExpression = @"sp_GetPlanPoints";
                    sqlCmd = new SqlCommand(sqlExpression, sqlConnection);

                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    nameParam = new SqlParameter
                    {
                        ParameterName = "@idfunc",
                        SqlDbType = SqlDbType.Int,
                        Value = exp.PlanID
                    };
                    sqlCmd.Parameters.Add(nameParam);
                    sqlCmd.ExecuteNonQuery();


                    sqlDataAdapter = new SqlDataAdapter(sqlCmd);

                    dataSet = new DataSet();

                    sqlDataAdapter.Fill(dataSet);

                    foreach (DataTable data in dataSet.Tables)
                    {
                        foreach (DataRow row in data.Rows)
                        {
                            // получаем все ячейки строки
                            var cells = row.ItemArray;
                            exp.yPlan.Add((double)cells[1]);
                        }
                    }


                }

                

            }

                
                
            

             var tempTree = new TreeViewItem { Header = "Функции" };

            tempTree.Items.Add(mainItem);

            tempTree.AddHandler(TreeView.MouseDoubleClickEvent, routedEventHandler);

            treeView.Items.Add(tempTree);
        }


        static public int IndexOf(List<Experiment> exps, string func)
        {
            for (int i = 0; i <exps.Count; i++)
            {
                if (exps[i].TextForm == func) return i;
            }
            return -1;
        }


         public void Update()
        {
            var list = new List<Experiment>();

            using (SqlConnection myConnection = new SqlConnection(@"Data Source =ВАСИЛИЙ-ПК\SQLEXPRESS; Initial Catalog =ProjectDB; Integrated Security = True;"))
            {
                string oString = "Select * from PointsEtalonFunction,EtalonFunction,PointPlansFunction";
                SqlCommand oCmd = new SqlCommand(oString, myConnection);
                myConnection.Open();

                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            xEtalon[i] = Convert.ToSingle(oReader[$"x{i + 1}"]);
                            yEtalon[i] = Convert.ToSingle(oReader[$"y{i + 1}"]);
                        }

                        text_form = oReader["TextFormat"].ToString();
                    }

                    myConnection.Close();
                }

            }
        }
    }
}
#endregion
