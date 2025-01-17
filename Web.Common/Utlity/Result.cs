using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS_MVC.Utlity
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }

        public Result()
        {

        }
        public Result(bool isRes)
        {
            IsSuccess = isRes;
        }
    }
}
