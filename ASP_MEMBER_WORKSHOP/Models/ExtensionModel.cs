using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace ASP_MEMBER_WORKSHOP.Models
{
    public static class ExtensionModel
    {
        // ปรับแต่งค่า Error ของ ModelSate ใหม่
        public static string GetErrorModelState(this ModelStateDictionary modelState)
        {
            var modelValue = modelState.Values.Select(value => value.Errors).FirstOrDefault();
            if (modelValue == null) return null;
            return modelValue[0].ErrorMessage;
        }

        // ปรับแต่งค่า Error Exception แสดงค่า inner exception ในสุด
        public static Exception GetErrorException(this Exception exception)
        {
            if (exception.InnerException != null)
                return exception.InnerException.GetErrorException();
            return exception;
        }
    }
}