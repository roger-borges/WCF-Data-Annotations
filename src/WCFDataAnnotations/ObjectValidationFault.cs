namespace WCFDataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.Text;

    [DataContract]
    public class ObjectValidationFault
    {
        [DataMember]
        public List<ValidationError> ValidationErrors { get; set; }

        public ObjectValidationFault()
        {
        }

        public ObjectValidationFault(List<ValidationError> validationErrors)
        {
            ValidationErrors = validationErrors;
        }
    }
}
