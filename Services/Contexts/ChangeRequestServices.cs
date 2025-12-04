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

        public string connectionString;

        public ChangeRequestServices(
            IConfiguration configuration, 
            IRealisService realisService, 
            IQtpServices qtpServices)
        {
            _configuration = configuration;
            _realisService = realisService;
            _qtpServices = qtpServices;
        }


        public async Task<ChangeRequestResponse> GetRealisAllTestState()
        {
            string serviceCalled = "";

            var functionName = "GetRealisAllTestState";
            var _responseText = "";
            var errorMessage = "";
            serviceCalled = "RealisService";
            List<int> lotsNAState = new List<int>();
            try
            {

                List<StressTestDetails> result = await _qtpServices.StressTableRealisTestAll("adasd");
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

                    HttpResponseMessage message = await rs.GetRealisStressStatus(jsonString);
                    if (!message.IsSuccessStatusCode)
                    {
                        var errorMessageJson = await message.Content.ReadAsStringAsync();
                        var errorMessageObject = JsonConvert.DeserializeObject<ErrorMessage>(errorMessageJson);
                        errorMessage = errorMessageObject.Message;
                    }
                    var state = await message.Content.ReadAsStringAsync();
                    List<StateDataItem> data = JsonConvert.DeserializeObject<List<StateDataItem>>(state);
                    List<TestState> testStateList = _qtpServices.GetAllowedTestStateForCR("Y,N/A");
                    var onHoldStateName = QTPService.GetConfigByAttribute("ON HOLD");
                    var hasOtherStateAndNoCRVal = QTPService.GetConfigByAttribute("HAS OTHER STATE BUT NO CR");
                    var testStates = new Dictionary<string, List<List<string>>>();
                    if (data != null)
                    {
                        foreach (var item in result)
                        {
                            foreach (var stateItem in data)
                            {
                                if (stateItem.id == item.realisTestID)
                                {
                                    var stateName = item.testCodeDisplay.ToUpper();
                                    if (testStateList.Any(x => x.STATE_NAME.ToUpper() == stateItem.state.name.ToUpper()))
                                    {
                                        if (testStateList.Any(x => x.STATE_NAME.ToUpper() == stateItem.state.name.ToUpper() && x.ENABLE_CR != hasOtherStateAndNoCRVal))
                                        {
                                            if (!testStates.ContainsKey(stateName))
                                            {
                                                testStates[stateName] = new List<List<string>>();
                                            }

                                            testStates[stateName].Add(new List<string>
                                            {
                                                item.lotNumber,
                                                stateItem.state.name.ToUpper(),
                                                "CR",
                                                item.qtpTransID.ToString(),
                                                item.testID.ToString()
                                            });
                                        }
                                        if (testStateList.Any(x => x.STATE_NAME.ToUpper() == stateItem.state.name.ToUpper() && x.ENABLE_CR == hasOtherStateAndNoCRVal))
                                        {
                                            if (!testStates.ContainsKey(stateName))
                                            {
                                                testStates[stateName] = new List<List<string>>();
                                            }
                                            testStates[stateName].Add(new List<string>
                                            {
                                                item.lotNumber,
                                                stateItem.state.name.ToUpper(),
                                                "NACR",
                                                item.qtpTransID.ToString(),
                                                item.testID.ToString()
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
                                            item.lotNumber,
                                            stateItem.state.name.ToUpper(),
                                            "NOCR",
                                            item.qtpTransID.ToString(),
                                            item.testID.ToString()
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
                    var output = testStates.SelectMany(x => x.Value.Select(y => new TestState
                    {
                        stateName = x.Key,
                        qtpTransID = y[3].ToString(),
                        testID = y[4].ToString(),
                        testStates = new List<object[]> { new object[] { y[0].ToString(), y[1].ToString(), y[2].ToString() } }
                    }))
                    .GroupBy(x => new { x.stateName, x.qtpTransID, x.testID })
                    .Select(g => new TestState
                    {
                        stateName = g.Key.stateName,
                        qtpTransID = g.Key.qtpTransID,
                        testID = g.Key.testID,
                        testStates = g.SelectMany(x => x.testStates).ToList()
                    })
                    .ToArray();

                    return new ChangeRequestResponse { Success = true, ResponseText = output, ResponseError = errorMessage };
                }
                else
                {
                    errorLog(serviceCalled, "No data in realisTestIDs", functionName);
                    return new ChangeRequestResponse { Success = false, ResponseText = _responseText, ResponseError = errorMessage };
                }
            }
            catch (Exception ex)
            {
                errorLog(serviceCalled, ex.Message, functionName);
                return new ChangeRequestResponse { Success = false, ResponseText = _responseText, ResponseError = errorMessage };
            }
        }
    }
}
