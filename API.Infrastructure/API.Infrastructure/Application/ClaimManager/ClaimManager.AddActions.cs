using API.Infrastructure.Interface;
using DAL.Model;
using Dapper;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Infrastructure.Application.ClaimManager
{
    internal partial class ClaimManager
    {
       public async Task AddActions(ActionsDTO action)
        {
            try
            {
                string proc = "AddPortalActions";
                var param = new DynamicParameters();
                param.Add("IncidenceDate",action.IncidenceDate);
                param.Add("ActionDescription",action.ActionDescription);
                param.Add("ActionName",action.ActionName);
                param.Add("ClaimType",(int)action.ClaimType);
                param.Add("ShopId",action.ShopId);
                param.Add("Reference",action.Reference);
                param.Add("RequestId",action.RequestId);
                param.Add("userId",action.userId);
              await _db.Connection.ExecuteAsync(proc, param, commandType:System.Data.CommandType.StoredProcedure);

            } catch (Exception ex)
            {
                _isettings.LogRequests(ex.Message,"AddActions",RequestType.Error);
            }
        }
         public async Task AddApprovalNotification(ActionsApprovalDTO action)
        {
            try
            {
                string proc = "AddApprovalNotification";
                var param = new DynamicParameters();
                param.Add("requestId", action.RequestId);
                param.Add("actiontype", action.actiontype);
                param.Add("actionstatus", action.actionstatus);
                param.Add("narration", action.narration);
                param.Add("dispatchcode", action.dispatchcode);
                param.Add("requestType", action.requestType);
                await _db.Connection.ExecuteAsync(proc, param, commandType: System.Data.CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {
                 _isettings.LogRequests(ex.Message,"AddApprovalNotification",RequestType.Error);
            }
        }

    }
}
