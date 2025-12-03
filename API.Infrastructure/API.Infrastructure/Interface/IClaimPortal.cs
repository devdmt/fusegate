using DAL.ModelView.Safaricom;
using DAL.ModelView;
using DAL.Model.Safaricom;
using API.Infrastructure.Application.ClaimManager;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace API.Infrastructure.Interface
{
    public interface IClaimPortal : ITransientService
    {
        //Task<List<RepairShopDTO>> GetRepair();
        Task<ResponseDTO<UserDetails>> ClaimUserAuth(ClaimAuth username);
        Task<ResponseDTO<RepairShopDTO>> AuthVerifyOTP(VerifyOTP verifyOTP);
        Task<ResponseDTO> ResendOTP(string UserId, int shoptype);
        Task<ResponseDTO> ValidateDispatchCode(ValidateDispatchDTO Code);
        Task<ResponseDTO> MakeClaim(ClaimRequestDTOPortal claimRequest);
        Task<ResponseDTO> SaveForLaterClaim(ClaimRequestDTOSaveForLater claimRequest);
        Task<List<ClaimRequestDTOSaveForLater>> QuerySaveForLaterClaim(ClaimSearchSaveForLaterDTO request);
        Task<List<PartsCostDTO>> getPartsCost(PartsQuery parts);
        Task<ValidateClaimRespose> ValidateClaim(ValidateClaimDTO claimDTO);
        Task<List<CustomerSearchDTO>> QueryCustomer(string request);
        Task<List<ClaimsDTO>> QueryClaim(ClaimSearchDTO request);
        Task<ResponseDTO<UploadResponse>> UploadCreditLife(CreditLifeUpload upload, string userId, string browser, string Ip);
        Task<ResponseDTO> ApproveCreditLife(CreditLifeApproveUpload approveUpload);
        Task<ResponseDTO> AppproveDispatch(Approvedispatch upload);
        Task<ResponseDTO> ResendDispatchCode(Approvedispatch upload);
        Task<ResponseDTO> SendNotification(SendNotificationRequest notification);
        Task<ClaimRequestDTOSaveForLaterDetails> QuerySaveForLaterClaim(string Id);
        Task AddActions(ActionsDTO actionsDTO);
        Task AddApprovalNotification(ActionsApprovalDTO action);
    }
    public class ValidateDispatchDTO
    {
        public string ShopId { get; set; }
        public string ClaimId { get; set; }
        public string Code { get; set; }

    }

    public class VerifyOTP
    {
        public int UserId { get; set; }
        public string OTP { get; set; }
    }
    public class QueryImei
    {
        public string Imei { get; set; }
        public string Idnumber { get; set; }
    }

    public class UploadResponse
    {
        public Int64 Id { get; set; }
        public int FailureCount { get; set; }
        public string File { get; set; }
        public int SuccessCount { get; set; } = 0;
        public bool withFailed { get; set; } = false;
    }
    public enum actiontype
    {
        damageclaim, theftclaim, dispatch, collection, approveclaim
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ApprovalType
    {
        [EnumMember(Value = "approve")] approve, [EnumMember(Value = "reject")] reject, [EnumMember(Value = "replace")] replace
    }
    public class ActionsApprovalDTO
    {
        public ApprovalType? actiontype { get; set; }
        public string? actionstatus { get; set; }
        public long? RequestId { get; set; }
        public string? narration { get; set; }
        public string? dispatchcode { get; set; }
        public ApproveRequestType requestType { get; set; }
    }
    public enum ApproveRequestType
    {
        approval, dispatch, collection
    }
    public class ActionsDTO
    {
        public string ActionName { get; set; }
        public string ActionDescription { get; set; }
        public string ShopId { get; set; }
        public string ShopType { get; set; }
        public ClaimType ClaimType { get; set; }
        public string IncidenceDate { get; set; }
        public actiontype actiontype { get; set; }
        public string Reference { get; set; }
        public string RequestId { get; set; }
        public string userId { get; set; }
        public string userName { get; set; }

    }
}
