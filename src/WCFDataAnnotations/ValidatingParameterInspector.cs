using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;

namespace WCFDataAnnotations
{
    public class ValidatingParameterInspector : IParameterInspector
    {
        private readonly IEnumerable<IObjectValidator> _validators;

        public ValidatingParameterInspector(IEnumerable<IObjectValidator> validators)
        {
            ValidateArguments(validators);

            _validators = validators;
        }

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
        }

        public object BeforeCall(string operationName, object[] inputs)
        {
            var validationResults = new List<ValidationResult>();

            foreach (var input in inputs)
            {
                foreach (var validator in _validators)
                {
                    var results = validator.Validate(input);
                    validationResults.AddRange(results);
                }
            }

            if (validationResults.Count > 0)
            {
                var objectValidationException = new ObjectValidationException(validationResults);

                throw new FaultException<ObjectValidationException>(objectValidationException);
            }

            return null;
        }

        private static void ValidateArguments(IEnumerable<IObjectValidator> validators)
        {
            if (validators == null)
            {
                throw new ArgumentNullException("validators");
            }

            if (!validators.Any())
            {
                throw new ArgumentException("At least one validator is required.");
            }
        }
    }
}
