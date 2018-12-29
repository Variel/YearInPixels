using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YearInPixels.Models.Response
{
    public class ErrorResponseModel
    {
        public bool Error => true;
        public string Message { get; set; }

        public ErrorResponseModel() { }
        public ErrorResponseModel(string message) => Message = message;
    }
}
