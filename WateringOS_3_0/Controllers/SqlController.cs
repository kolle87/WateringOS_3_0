using System;
using System.Data;
using System.Data.SqlClient;
using WateringOS_3_0.Models;

/// SQL Class for logging on RaspberryPi 3
/// to be used with WateringOS >2.0
/// 
/// (C) Michael Kollmeyer, 27-Sept-2019
/// 

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
