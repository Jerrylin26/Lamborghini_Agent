using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System;

namespace Lamborghini
{
    /*
        此類別僅為後端驗證用途,前端驗證我搞不來
     */
    public class MyEmailAttribute : ValidationAttribute, IClientModelValidator
    {

        public override bool IsValid(object value)
        {
            string email = value.ToString();
            string pattern = @"^[\w\.-]+@[\w\.-]+\.\w{2,}$";
            bool isValid = Regex.IsMatch(email, pattern);
            if (isValid)
            {
                return true;
            }
            return false;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            //方式一
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-myemail", "這不是email格式!!!");

            //方式二
            //context.Attributes["data-val"] = "true";
            //context.Attributes["data-val-publishdate"] = "your publishdate should after 2020/12/30(前端驗證)";
        }

        private bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }
            attributes.Add(key, value);
            return true;
        }
    }
}
