using DAL.ModelView.Safaricom;
using Dapper;


namespace API.Infrastructure.Application.ClaimManager
{
    internal partial class ClaimManager
    {
        public void AddNotification(Notificationadd notificationadd)
        {

            try
            {
                string proc = "AddNotification";
                var param = new DynamicParameters();
                param.Add("@requestId",notificationadd.RequestId);
                param.Add("@userId",notificationadd.UserId); 
                param.Add("@shopId",notificationadd.ShopId);
                param.Add("@notificationType",(int)notificationadd.notificationType);
                 param.Add("@code",notificationadd.Code);
                _db.Connection.Execute(proc, param,commandType:System.Data.CommandType.StoredProcedure);


            } catch (Exception ex)
            {
                _isettings.LogRequests(ex.Message, "AddNotification", requestType: Interface.RequestType.Error);
            }
        }
    }
}
