using Oracle.ManagedDataAccess.Client;
using QTPCR.Models;
using QTPCR.Services.Contracts;
using System;
using System.Data;
using System.Threading.Tasks;

namespace QTPCR.Services.Contexts
{
    public class LogsServices : ILogsServices
    {
        private readonly IConfiguration _configuration;
        private readonly IQtpServices _qtpServices;

        public LogsServices(IConfiguration configuration, IQtpServices qtpServices)
        {
            _configuration = configuration;
            _qtpServices = qtpServices;
        }


        public async Task<string> ErrorLog(ErrorModel errorModel)
        {
            string connString = Environment.GetEnvironmentVariable("QTPConnectionString") ?? _configuration["ConnectionStrings:QTP"];
            DateTime dateTime = DateTime.Now;
            using (OracleConnection conn = new OracleConnection(connString))
            {
                OracleTransaction transaction;
                conn.Open();

                transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    OracleCommand command = new OracleCommand();
                    command.Connection = conn;
                    command.Transaction = transaction;

                    command.CommandText = "QTP_ERRORLOGS_INSERTUPDATE";
                    command.BindByName = true;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("P_FUNCTION_USED", "varchar2").Value = errorModel.FunctionName;
                    command.Parameters.Add("P_PARAMETERS_USED", "varchar2").Value = $"Parameter(s): {errorModel.Parameters}";
                    command.Parameters.Add("P_EXCEPTION_STACKTRACE", "number").Value = $"Exception: {errorModel.ExceptionStackTrace}";
                    command.Parameters.Add("P_DATE_CREATED", "varchar2").Value = dateTime.ToString("yyyy/mm/dd/ HH:mm:ss tt");
                    command.Parameters.Add("P_STATUS", "varchar2").Value = "New";

                    command.ExecuteNonQuery();
                    command.Parameters.Clear();

                    transaction.Commit();
                    conn.Close();
                    return "Success";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return "Failed";
                }
            }
        }

    }
}
