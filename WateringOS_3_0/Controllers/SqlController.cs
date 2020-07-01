using System;
using System.Data;
using System.Data.SqlClient;
using WateringOS_3_0.Models;

/* DISCLAIMER

Watering OS - (C) Michael Kollmeyer 2020  
  
This file is part of WateringOS.

    WateringOS is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    WateringOS is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with WateringOS.  If not, see<https://www.gnu.org/licenses/>.

*/

namespace WateringOS_3_0
{
    public class SQLController
    {
        private SqlConnection SqlActiveConnection;

        public void InitSql()
        {
            SqlLog(LogType.Information, "Start initialisation", "The intialization of the SQL server class has started.");
            try
            {
                SqlActiveConnection = new SqlConnection("Data Source=wateringsystem.database.windows.net;Initial Catalog=WateringSystem;Persist Security Info=True;User ID=kollmeyer.michael;Password=00380465*Watering");
                SqlActiveConnection.Open();
            }
            catch (SqlException e)
            {
                SqlLog(LogType.Error, "Error in SQL Class: ", e.Message);
            }
        }
        public void WriteToSql()
        {            
                try
                {

                switch (SqlActiveConnection.State)
                {
                    case ConnectionState.Open:
                        if (!Globals.SqlIsBusy)
                        {
                            Globals.SqlIsBusy = true;
                            if (Globals.SqlDat_Data != "")
                            {
                                string vSQLcommand_Dat = Globals.SqlDat_Prefix + Globals.SqlDat_Data.Remove(Globals.SqlDat_Data.Length - 1) + ";";
                                using (SqlCommand command = new SqlCommand(vSQLcommand_Dat, SqlActiveConnection))
                                {
                                    try { command.ExecuteNonQuery(); } catch (Exception sqle) { SqlLog(LogType.Error, "Error while writing data string to SQL: ", sqle.Message); Globals.SqlIsBusy = false; break; }
                                }
                            }
                            if (Globals.SqlLog_Data != "")
                            {
                                string vSQLcommand_Log = Globals.SqlLog_Prefix + Globals.SqlLog_Data.Remove(Globals.SqlLog_Data.Length - 1) + ";";
                                using (SqlCommand command = new SqlCommand(vSQLcommand_Log, SqlActiveConnection))
                                {
                                    try { command.ExecuteNonQuery(); } catch (Exception sqle) { SqlLog(LogType.Error, "Error while writing logs string to SQL: ", sqle.Message); Globals.SqlIsBusy = false; break; }
                                }
                            }
                            Globals.SqlDat_Data = "";
                            Globals.SqlLog_Data = "";
                            Globals.SqlIsBusy = false;
                        }
                        break;
                    case ConnectionState.Closed:
                        SqlLog(LogType.Error, "SQL connection closed", "The SQL connection is closed.");
                        SqlLog(LogType.Error, "Restart SQL connection", "The SQL connection will be reinitialized.");
                        Globals.SqlIsBusy = true;
                        InitSql();
                        Globals.SqlIsBusy = false;
                        break;
                    case ConnectionState.Connecting:
                        SqlLog(LogType.Warning, "SQL connection is connecting", "The SQL connection object is connecting to the data source.");
                        break;
                    case ConnectionState.Fetching:
                        SqlLog(LogType.Warning, "SQL connection is fetching", "The SQL connection object is retrieving data.");
                        break;
                    case ConnectionState.Executing:
                        SqlLog(LogType.Warning, "SQL connection is executing", "The SQL connection object is executing a command.");
                        break;
                    case ConnectionState.Broken:
                        SqlLog(LogType.Error, "SQL connection broken", "The SQL connection to the data source is broken. This can occur only after the connection has been opened. A connection in this state may be closed and then re-opened.");
                        break;
                    }
                }
                catch (Exception e)
                {
                    SqlLog(LogType.Error, "Error while writing to SQL: ", e.Message + " -> " + Globals.SqlLog_Data);
                    Globals.SqlDat_Data = "";
                    Globals.SqlLog_Data = "";
                    Globals.SqlIsBusy = false;
                }
            }
        private void SqlLog(string vType, string vMessage, string vDetail)
        {
            LoggingEventArgs args = new LoggingEventArgs();
            args.TimeStamp = DateTime.Now;
            args.Instance = "[SQL]";
            args.Type = vType;
            args.Message = vMessage;
            args.Detail = vDetail;
            OnSqlLoggingEvent(args);
        }
        protected void OnSqlLoggingEvent(LoggingEventArgs e)
        {
            EventHandler<LoggingEventArgs> handler = SqlLogEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<LoggingEventArgs> SqlLogEvent;
    }
}
