using API.Infrastructure.Interface;
using DAL.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Infrastructure.Application.Pay
{
    internal partial class Pay
    {
         public async Task<AcknowledgementDTO> RequestMpesaExpress(STkPushRequestDTO sTkPushRequestDTO, string token, string requesturl)
        {
            var res = new AcknowledgementDTO();


            var jsonrequest = JsonSerializer.Serialize(sTkPushRequestDTO);
            _setting.LogRequests(jsonrequest, "RequestMpesaExpress", RequestType.Info);
            var mpesaresponse = await ProcessJsonManager.ProcessJsonAsync(requesturl, token, jsonrequest,
                ProcessJsonManager.methodtype.Post, ProcessJsonManager.TokenType.Bearer);
            _setting.LogRequests(mpesaresponse, "RequestMpesaExpress", RequestType.Info);
            if (mpesaresponse.Contains("errorCode"))
            {
                var responseerror = JsonSerializer.Deserialize<STKResponseErrorDTO>(mpesaresponse);
                res.ResponseCode = responseerror.errorCode;
                res.ResponseDescription = responseerror.errorMessage;
                res.RequestId = responseerror.requestId;
                res.Success = false;
            }
            else
            {
                var response = JsonSerializer.Deserialize<STKResponseDTO>(mpesaresponse);

                res.Success = true;
                res.ErrorMessage = response.ResponseDescription;
                res.ResponseCode = response.ResponseCode;
                res.CustomerMessage = response.CustomerMessage;
                res.CheckoutRequestID = response.CheckoutRequestID;
                res.MerchantID = response.MerchantRequestID;
                res.ResponseDescription = response.ResponseDescription;

            }
            return res;
        }
    }
    }

