using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WCFDataAnnotations
{
    public interface IObjectValidator
    {
        IEnumerable<ValidationResult> Validate(object input);
    }
}