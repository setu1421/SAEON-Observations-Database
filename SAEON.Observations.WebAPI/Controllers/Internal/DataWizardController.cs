using SAEON.Logs;
using SAEON.Observations.Core;
using System;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    [RoutePrefix("Internal/DataWizard")]
    public class DataWizardController : ApiController
    {
        private static Random random = new Random(DateTime.Now.Millisecond);

        [HttpGet]
        [Route("Approximation")]
        public DataWizardApproximation Approximation([FromUri] DataWizardInput input)
        {
            using (Logging.MethodCall<DataWizardApproximation>(GetType()))
            {
                try
                {
                    Logging.Verbose("Uri: {Uri}", Request.RequestUri);
                    Logging.Verbose("Input: {@Input}", input);
                    var result = new DataWizardApproximation
                    {
                        RowCount = random.Next()
                    };
                    if (result.RowCount % 2 == 0)
                    {
                        result.Errors.Add($"Error {random.Next()}");
                    }
                    Logging.Verbose("RowCount: {RowCount}", result.RowCount);
                    Logging.Verbose("Errors: {Errors}", result.Errors);
                    return result;
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get approximation");
                    throw;
                }
            }
        }

    }
}
