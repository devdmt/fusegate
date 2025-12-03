using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ModelView
{
    public class ApiRequestsDTO
    {


       
        public string? RequestName { get; set; }
        public int? RequestType { get; set; }
        public string? ApiName { get; set; }
        public string? PayLoad { get; set; }
        public string? IP { get; set; }

    }
    public enum ApiRequestType 
    {
    Create,Update, Delete, Query
    }
    public class UpdateRequestsDTO
    {
        public string Id { get; set; }
        public string? Response { get; set; }

        public bool? Responded { get; set; }
        public bool? Failed { get; set; }
        public string? ErrorMsg { get; set; }

    }
}
