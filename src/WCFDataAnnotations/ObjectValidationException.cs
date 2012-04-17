namespace WCFDataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel;
    using System.Text;

    public class ObjectValidationException : FaultException
    {
        private readonly List<ValidationResult> _validationResults;

        public List<ValidationResult> ValidationResults
        {
            get
            {
                return _validationResults;
            }
        }

        public ObjectValidationException(List<ValidationResult> validationResults)
        {
            _validationResults = validationResults;
        }
    }
}
