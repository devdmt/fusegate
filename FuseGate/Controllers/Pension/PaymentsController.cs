using API.Infrastructure.Interface;
using DAL.Model;
using EsbJson.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FuseGate.Controllers.Pension
{
    [ApiController]
    [Route("ProcessResult")]
    [AllowAnonymous]
    public class PaymentsController : VersionedApiController
    {
        public readonly Isettings _isettings;
        public readonly IPay _mpesa;
        public PaymentsController(Isettings isettings,  IPay mpesa)
        {
            _isettings = isettings;
            _mpesa = mpesa;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ProcessPensionSTKResult()
        {
            using var reader = new StreamReader(HttpContext.Request.Body);
             _isettings.LogRequests("test", "ProcessPensionSTKResult",RequestType.Info);



             var body = await reader.ReadToEndAsync();
            try
            {
                if (!string.IsNullOrEmpty(body))
                {
                   _isettings.LogRequests(body, "ProcessPensionSTKResult",RequestType.Info);
                    //var stkresult = await HttpContext.Request.ReadFromJsonAsync<MpesaSTKResult>();
                    var stkre = JsonSerializer.Deserialize<MpesaSTKResult>(body);
                    var res = await _mpesa.ProcessPensionSTKResult(body, stkre);

                }

            }
            catch (Exception ex) {

                _isettings.LogRequests(ex.Message, "ProcessPensionSTKResult", RequestType.Error);
            
            }

            return Ok();
        }
    

        [HttpPost("ProcessInsureSTKResult")]
        [AllowAnonymous]
        public async Task<IActionResult> ProcessInsureSTKResult()
        {
            using var reader = new StreamReader(HttpContext.Request.Body);
             _isettings.LogRequests("test", "ProcessPensionSTKResult",RequestType.Info);



             var body = await reader.ReadToEndAsync();
            try
            {
                if (!string.IsNullOrEmpty(body))
                {
                   _isettings.LogRequests(body, "ProcessInsureSTKResult",RequestType.Info);
                    //var stkresult = await HttpContext.Request.ReadFromJsonAsync<MpesaSTKResult>();
                    var stkre = JsonSerializer.Deserialize<MpesaSTKResult>(body);
                    var res = await _mpesa.ProcessInsureSTKResult(body, stkre);

                }

            }
            catch (Exception ex) {

                _isettings.LogRequests(ex.Message, "ProcessPensionSTKResult", RequestType.Error);
            
            }

            return Ok();
        }
    }
}
