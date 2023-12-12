using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace DemoMsUser.Common.Attributes
{
    public class AtLeastOneElementAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return value is IList { Count: > 0 };
        }
    }
}
