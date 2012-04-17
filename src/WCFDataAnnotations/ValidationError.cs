namespace WCFDataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    [DataContract]
    public class ValidationError
    {
        [DataMember]
        public List<string> MemberNames { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
    }
}
