using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WCFDataAnnotations
{
    public class DataAnnotationsObjectValidator : IObjectValidator
    {
        public IEnumerable<ValidationResult> Validate(object input)
        {
            if (input == null) return Enumerable.Empty<ValidationResult>();

            var inputMetadataAttributes = TypeDescriptor.GetAttributes(input.GetType()).OfType<MetadataTypeAttribute>();

            var metaResults = new List<ValidationResult>();

            foreach (var ima in inputMetadataAttributes)
            {
                var result = from property in TypeDescriptor.GetProperties(input).Cast<PropertyDescriptor>()
                             from metaProperty in TypeDescriptor.GetProperties(ima.MetadataClassType).Cast<PropertyDescriptor>()
                             from attribute in metaProperty.Attributes.OfType<ValidationAttribute>()
                             where property.Name == metaProperty.Name && !attribute.IsValid(property.GetValue(input))
                             select new ValidationResult
                             (
                                 attribute.FormatErrorMessage(property.Name),
                                 new[] { property.Name }
                             );

                if (result != null && result.Count() > 0)
                {
                    metaResults.AddRange(result);
                }
            }
            var directPropertyResults = (from property in TypeDescriptor.GetProperties(input).Cast<PropertyDescriptor>()
                                         from attribute in property.Attributes.OfType<ValidationAttribute>()
                                         where !attribute.IsValid(property.GetValue(input))
                                         select new ValidationResult
                                         (
                                             attribute.FormatErrorMessage(property.Name),
                                             new[] { property.Name }
                                         )).ToList();

            directPropertyResults.AddRange(metaResults);

            return directPropertyResults;
        }
    }
}
