namespace DAL.ModelView
{
    public class ResponseDTO<T> where T : class
    {
        public string ErrorMsg { get; set; }
        public bool Success { get; set; }
        public T Result { get; set; }
    }
    public class ResponseDTO    
    {
        public string ErrorMsg { get; set; }
        public bool Success { get; set; }
        public string TransactionId { get; set; }
        public string ResponseId { get; set; }
        
    }
  
    public class OnboardingResponseDTO
    {
        public string ErrorMsg { get; set; }
        public bool Success { get; set; }
        public string TransactionId { get; set; }
        public string ResponseId { get; set; }
        public string CustomerId { get; set; }
    }

 
}
