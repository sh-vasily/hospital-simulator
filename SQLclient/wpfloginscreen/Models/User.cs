using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfloginscreen.Models
{
    class User
    {
        private string name;
        public string type;
        public string Name
        {
            get => name;
            set => name = value;
        }

        public string Type
        {
            get => type;
            set => type = value;
        }

        public User(string _name)
        {
            Name = _name;
            type = GetType(_name);
        }


        static public string GetType(string name)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                if (sqlConnection.State == ConnectionState.Closed)
                    sqlConnection.Open();

                var sqlExpression = @"sp_GetAccessRightsUser";
                var sqlCmd = new SqlCommand(sqlExpression, sqlConnection);

                sqlCmd.CommandType = CommandType.StoredProcedure;

                var nameParam = new SqlParameter
                {
                    ParameterName = "@username",
                    Size = 50,
                    Value = name
                };
                sqlCmd.Parameters.Add(nameParam);

                var isCorrectParam = new SqlParameter
                {
                    ParameterName = "@usertype",
                    Size = 50,
                    SqlDbType = SqlDbType.VarChar
                };

                isCorrectParam.Direction = ParameterDirection.Output;

                sqlCmd.Parameters.Add(isCorrectParam);

                sqlCmd.ExecuteNonQuery();

                return sqlCmd.Parameters["@usertype"].Value as string;
            }
        }
    }
}
