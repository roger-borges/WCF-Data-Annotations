using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WCFDataAnnotations
{
    public interface IErrorMessageGenerator
    {
        string GenerateErrorMessage(string operationName, IEnumerable<ValidationResult> validationResults);
    }
}