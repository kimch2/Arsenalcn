﻿using System.Configuration;
using System.Data.SqlClient;

namespace Arsenalcn.Common
{
    public static class SQLConn
    {
        public static SqlConnection GetConnection()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Arsenalcn.ConnectionString"].ConnectionString;

            return new SqlConnection(connectionString);
        }
    }
}