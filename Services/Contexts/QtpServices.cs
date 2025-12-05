using Oracle.ManagedDataAccess.Client;
using QTPCR.Models;
using QTPCR.Services.Contracts;
using System.Data;

namespace QTPCR.Services.Contexts
{
    public class QtpServices : IQtpServices
    {
        private readonly IConfiguration _configuration;
        private readonly IRealisService _realisService;


        public QtpServices(IConfiguration configuration, IRealisService realisService)
        {
            _configuration = configuration;
            _realisService = realisService;
        }


        public async Task<List<StressTestDetails>> StressTableRealisTestAll(string qtpNumber)
        {
            List<StressTestDetails> StressTableRealisList = new List<StressTestDetails>();

            using (OracleConnection conn = new OracleConnection(_configuration["ConnectionStrings:QTP"]))
            {
                conn.Open();

                OracleCommand command = new OracleCommand("QTP_STRESS_TABLE_REALIS_TEST_ALL", conn);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("p_qtp_number", OracleDbType.Varchar2, ParameterDirection.Input).Value = qtpNumber;

                OracleParameter resultParameter = new OracleParameter("p_result", OracleDbType.RefCursor, ParameterDirection.Output);
                command.Parameters.Add(resultParameter);

                //command.ExecuteNonQuery();

                OracleDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var rl = new StressTestDetails();

                    rl.QtpNumber = reader.IsDBNull(reader.GetOrdinal("QTP_NUMBER")) ? "" : reader.GetString(reader.GetOrdinal("QTP_NUMBER"));
                    rl.QtpTransID = reader.IsDBNull(reader.GetOrdinal("QTP_TRANS_ID")) ? "" : reader.GetString(reader.GetOrdinal("QTP_TRANS_ID"));
                    rl.TestID = reader.IsDBNull(reader.GetOrdinal("TEST_ID")) ? "" : reader.GetString(reader.GetOrdinal("TEST_ID"));
                    rl.LotNumber = reader.IsDBNull(reader.GetOrdinal("LOT_NUMBER")) ? "" : reader.GetString(reader.GetOrdinal("LOT_NUMBER"));
                    rl.DefinedLotSequence = reader.IsDBNull(reader.GetOrdinal("DEFINED_LOT_SEQUENCE")) ? "" : reader.GetString(reader.GetOrdinal("DEFINED_LOT_SEQUENCE"));
                    rl.RealisTestID = reader.IsDBNull(reader.GetOrdinal("REALIS_TEST_ID")) ? "" : reader.GetString(reader.GetOrdinal("REALIS_TEST_ID"));
                    rl.TestCodeDisplay = reader.IsDBNull(reader.GetOrdinal("TEST_CODE_DISPLAY")) ? "" : reader.GetString(reader.GetOrdinal("TEST_CODE_DISPLAY"));
                    rl.ProjectID = reader.IsDBNull(reader.GetOrdinal("PROJECT_ID")) ? "" : reader.GetString(reader.GetOrdinal("PROJECT_ID"));

                    StressTableRealisList.Add(rl);
                }
                conn.Close();
            }
            return StressTableRealisList;
        }

        public async Task<List<TestState>> GetAllowedTestStateForCR(string enable_cr)
        {
            try
            {
                List<TestState> testStateList = new List<TestState>();
                using (OracleConnection conn = new OracleConnection(_configuration["ConnectionString:QTP"]))
                {
                    conn.Open();


                    OracleCommand command = new OracleCommand("QTP_GET_TEST_STATE_REF", conn);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("p_enable_cr", OracleDbType.Varchar2, ParameterDirection.Input).Value = enable_cr;

                    OracleParameter resultParameter = new OracleParameter("p_result", OracleDbType.RefCursor, ParameterDirection.Output);
                    command.Parameters.Add(resultParameter);

                    //command.ExecuteNonQuery();

                    OracleDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var testState = new TestState();
                        testState.ID = reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID"));
                        testState.STATE_NAME = reader.IsDBNull(reader.GetOrdinal("STATE_NAME")) ? "" : reader.GetString(reader.GetOrdinal("STATE_NAME"));
                        testState.CODE = reader.IsDBNull(reader.GetOrdinal("CODE")) ? 0 : reader.GetInt32(reader.GetOrdinal("CODE"));
                        testState.ENABLE_CR = reader.IsDBNull(reader.GetOrdinal("ENABLE_CR")) ? "" : reader.GetString(reader.GetOrdinal("ENABLE_CR"));
                        testStateList.Add(testState);
                    }
                    conn.Close();
                }
                return testStateList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> GetConfigByAttribute(string attribute)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(_configuration["ConnectionString:QTP"]))
                {
                    string testprogramvalue = "";
                    conn.Open();
                    var cmd = "SELECT VALUE FROM QTP_CONFIG where IS_USED = 1 AND ATTRIBUTE = '" + attribute + "'";
                    OracleCommand command = new OracleCommand(cmd, conn);
                    OracleDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        testprogramvalue = reader.IsDBNull(0) ? "" : reader.GetString(0);
                    }
                    conn.Close();
                    return testprogramvalue;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}
