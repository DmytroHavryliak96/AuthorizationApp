using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationApp.ViewModels
{
    public enum Status
    {
        Success = 1,
        Error = 2
    }
    public class ResultViewModel
    {
        public Status Status {get; set;}
        public string Message { get; set; }
        public Object Data { get; set; }
    }
}
