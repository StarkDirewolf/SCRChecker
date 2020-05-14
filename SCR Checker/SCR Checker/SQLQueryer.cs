﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR_Checker
{
    class SQLQueryer
    {
        private const string CONNECTION_STRING = @"Data Source=10.11.117.174\PHARMACYE14;Initial Catalog=ProScriptConnect;Integrated Security=true;";

        public static Dictionary<string, string> GetDeliveryNHSNumbers(DateTime fromDay, DateTime toDay, bool onlyNomads)
        {
            QueryConstructor query = new QueryConstructor(QueryConstructor.QueryType.PATIENTS_WITH_NOTES_DISPENSED_TO);

            Dictionary<string, string> nhsNumNameLookup = new Dictionary<string, string>();

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                if (fromDay.Date == toDay.Date)
                {
                    query.SpecificDay(fromDay);
                } else
                {
                    query.BetweenDays(fromDay, toDay);
                }
                
                query.SortBy();
                SqlCommand command = new SqlCommand(query.ToString(), connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        // NHS number first
                        // Patient first name second
                        // Patient second name third
                        // Patient notes fourth
                        string nhsNum = reader[0].ToString();
                        if (!nhsNumNameLookup.ContainsKey(nhsNum))
                        {
                            if (reader[3].ToString().ToLower().Contains("deliver"))
                            {
                                if (onlyNomads)
                                {
                                    if (reader[3].ToString().ToLower().Contains("nomad") || reader[3].ToString().ToLower().Contains("dosset")
                                        || reader[3].ToString().ToLower().Contains("doset"))
                                    {
                                        nhsNumNameLookup.Add(nhsNum, reader[2] + ", " + reader[1]);
                                    }
                                } else
                                {
                                    nhsNumNameLookup.Add(nhsNum, reader[2] + ", " + reader[1]);
                                }
                                
                            }
                        }
                    }
                }
                
            }

            return nhsNumNameLookup;
        }
    }
}
