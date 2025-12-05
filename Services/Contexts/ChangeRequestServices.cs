using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using QTPCR.Models;
using QTPCR.Services.Contracts;
using System.Data;
using System.Net.Http;
using System.Text;

namespace QTPCR.Services.Contexts
{
    public class ChangeRequestServices : IChangeRequestServices
    {

        private readonly IConfiguration _configuration;
        private readonly IRealisService _realisService;
        private readonly IQtpServices _qtpServices;
        private readonly ILogsServices _logsServices;


        public ChangeRequestServices(
            IConfiguration configuration, 
            IRealisService realisService, 
            IQtpServices qtpServices,
            ILogsServices logsServices)
        {
            _configuration = configuration;
            _realisService = realisService;
            _qtpServices = qtpServices;
            _logsServices = logsServices;
        }


        public async Task<ChangeRequestResponse> GetRealisAllTestState(string qtpNumber)
        {
            string serviceCalled = "";

            var functionName = "GetRealisAllTestState";
            var _responseText = "";
            var errorMessage = "";
            serviceCalled = "RealisService";
            List<int> lotsNAState = new List<int>();
            try
            {

                List<StressTestDetails> result = await _qtpServices.StressTableRealisTestAll(qtpNumber);
                List<int> lots = new List<int>();
                List<string> realisTestIDs = new List<string>();
                if (result.Count == 0)
                {
                    return new ChangeRequestResponse { Success = true, ResponseText = _responseText, ResponseStatePerLot = _responseText, ResponseHasOtherState = false, ResponseHasOtherStateAndNoCR = false, ResponseOnHoldCount = 0, ResponseError = errorMessage, ResponseNALots = lotsNAState };
                }
                foreach (var item in result)
                {
                    if (!realisTestIDs.Contains(item.RealisTestID))
                    {
                        realisTestIDs.Add(item.RealisTestID);
                    }
                }
                if (realisTestIDs.Count > 0)
                {
                    var jsonOutput = new { ids = realisTestIDs };

                    string jsonString = JsonConvert.SerializeObject(jsonOutput);

                    HttpResponseMessage message = await _realisService.GetRealisStressStatus(jsonString);
                    if (!message.IsSuccessStatusCode)
                    {
                        var errorMessageJson = await message.Content.ReadAsStringAsync();
                        var errorMessageObject = JsonConvert.DeserializeObject<ErrorMessage>(errorMessageJson);
                        errorMessage = errorMessageObject.Message;
                    }
                    var state = await message.Content.ReadAsStringAsync();
                    List<StateDataItem> data = JsonConvert.DeserializeObject<List<StateDataItem>>(state);
                    List<TestState> testStateList = await _qtpServices.GetAllowedTestStateForCR("Y,N/A");
                    var onHoldStateName = await _qtpServices.GetConfigByAttribute("ON HOLD");
                    var hasOtherStateAndNoCRVal = await _qtpServices.GetConfigByAttribute("HAS OTHER STATE BUT NO CR");
                    var testStates = new Dictionary<string, List<List<string>>>();
                    if (data != null)
                    {
                        foreach (var item in result)
                        {
                            foreach (var stateItem in data)
                            {
                                if (stateItem.Id == item.RealisTestID)
                                {
                                    var stateName = item.TestCodeDisplay.ToUpper();
                                    if (testStateList.Any(x => x.STATE_NAME?.ToUpper() == stateItem.State.Name?.ToUpper()))
                                    {
                                        if (testStateList.Any(x => x.STATE_NAME?.ToUpper() == stateItem.State.Name?.ToUpper() && x.ENABLE_CR != hasOtherStateAndNoCRVal))
                                        {
                                            if (!testStates.ContainsKey(stateName))
                                            {
                                                testStates[stateName] = new List<List<string>>();
                                            }

                                            testStates[stateName].Add(new List<string>
                                            {
                                                item.LotNumber,
                                                stateItem.State.Name?.ToUpper(),
                                                "CR",
                                                item.QtpTransID.ToString(),
                                                item.TestID.ToString()
                                            });
                                        }
                                        if (testStateList.Any(x => x.STATE_NAME?.ToUpper() == stateItem.State.Name?.ToUpper() && x.ENABLE_CR == hasOtherStateAndNoCRVal))
                                        {
                                            if (!testStates.ContainsKey(stateName))
                                            {
                                                testStates[stateName] = new List<List<string>>();
                                            }
                                            testStates[stateName].Add(new List<string>
                                            {
                                                item.LotNumber,
                                                stateItem.State.Name.ToUpper(),
                                                "NACR",
                                                item.QtpTransID.ToString(),
                                                item.TestID.ToString()
                                            });
                                        }
                                    }
                                    else
                                    {
                                        if (!testStates.ContainsKey(stateName))
                                        {
                                            testStates[stateName] = new List<List<string>>();
                                        }
                                        testStates[stateName].Add(new List<string>
                                        {
                                            item.LotNumber,
                                            stateItem.State.Name.ToUpper(),
                                            "NOCR",
                                            item.QtpTransID.ToString(),
                                            item.TestID.ToString()
                                        });
                                    }
                                }
                            }
                        }
                    }

                    //var outputs = testStates.Select(x => new TestState { stateName = x.Key, testStates = x.Value }).ToArray();
                    //var output = testStates.Select(x => new TestState
                    //{
                    //    stateName = x.Key,
                    //    qtpTransID = x.Value.First()[3].ToString(),
                    //    testID = x.Value.First()[4].ToString(),
                    //    testStates = x.Value.Select(y => new object[] { y[0].ToString(), y[1].ToString().ToString(), y[2].ToString() }).ToList()
                    //}).ToArray();
                    var output = testStates.SelectMany(x => x.Value.Select(y => new QTPTestState
                    {
                        StateName = x.Key,
                        QtpTransID = y[3].ToString(),
                        TestID = y[4].ToString(),
                        TestStates = new List<object[]> { new object[] { y[0].ToString(), y[1].ToString(), y[2].ToString() } }
                    }))
                    .GroupBy(x => new { x.StateName, x.QtpTransID, x.TestID })
                    .Select(g => new QTPTestState
                    {
                        StateName = g.Key.StateName,
                        QtpTransID = g.Key.QtpTransID,
                        TestID = g.Key.TestID,
                        TestStates = g.SelectMany(x => x.TestStates).ToList()
                    })
                    .ToArray();

                    return new ChangeRequestResponse { Success = true, ResponseText = output, ResponseError = errorMessage };
                }
                else
                {
                    var errorLog = new ErrorModel
                    {
                        FunctionName = functionName,
                        ExceptionStackTrace = "No data in realisTestIDs",
                        Message = "No data in realisTestIDs"
                    };
                    await _logsServices.ErrorLog(errorLog);
                    return new ChangeRequestResponse { Success = false, ResponseText = _responseText, ResponseError = errorMessage };
                }
            }
            catch (Exception ex)
            {
                var errorLog = new ErrorModel
                {
                    FunctionName = functionName,
                    ExceptionStackTrace = "No data in realisTestIDs",
                    Message = "No data in realisTestIDs"
                };
                await _logsServices.ErrorLog(errorLog);
                return new ChangeRequestResponse { Success = false, ResponseText = _responseText, ResponseError = errorMessage };
            }
        }
    }
}
