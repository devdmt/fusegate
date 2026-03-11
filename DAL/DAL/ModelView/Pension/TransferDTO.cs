using System.Runtime.Serialization;
using System.Text.Json.Serialization;
namespace DAL.ModelView.Pension
{
    public class TransferRequestDTO
    {
        public string MemberNo { get; set; }
        public  string AgentCode { get; set; }
        public List<TransferDTO> TransferDetails { get; set; }
    }
    public class TransferDTO
    {
        
        public string? EmployerName { get; set; }
        public string? HrEmail { get; set; }
        public string? HrPhone { get; set; }
        public bool CurrentlyFunding { get; set; } = false;
        public string? PensionProviderName { get; set; }
        public string? AdditionalInformation { get; set; }
        public string? EmploymentYear { get; set; }

    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransferStatus
    {

        [EnumMember(Value = "Initiated")] Initiated,
        [EnumMember(Value = "Submitted")] Submitted, [EnumMember(Value = "Received")] Received,
        [EnumMember(Value = "Processed")] Processed, [EnumMember(Value = "Declined")] Declined

    }
}