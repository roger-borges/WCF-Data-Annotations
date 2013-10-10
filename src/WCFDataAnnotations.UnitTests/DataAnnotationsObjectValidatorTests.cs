using System.ComponentModel.DataAnnotations;
using System.Linq;
using NUnit.Framework;

namespace WCFDataAnnotations.UnitTests
{
    [TestFixture]
    public class DataAnnotationsObjectValidatorTests
    {
        private const string ErrorMessage1 = "property 1 is required";
        private const string ErrorMessage2 = "property 2 can only contains numbers";

        private DataAnnotationsObjectValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new DataAnnotationsObjectValidator();
        }

        [Test]
        public void Validate_Does_Not_Return_ValidationResult_When_Passed_Null()
        {
            var result = _validator.Validate(null);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Any(), Is.False);
        }

        [Test]
        public void Validate_Does_Not_Return_ValidationResult_When_Not_Passed_Object_With_DataAnnotations()
        {
            var result = _validator.Validate("blah");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Any(), Is.False);
        }

        [Test]
        public void Validate_Does_Not_Return_ValidationResult_When_Passed_Object_That_Is_Valid()
        {
            var result = _validator.Validate(new TestClass { Property1 = "hello", Property2 = "12345", Property3 = "test" });

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Any(), Is.False);
        }

        [Test]
        public void Validate_Returns_ValidationResult_When_Passed_Object_That_Has_One_Invalid_Property()
        {
            var result = _validator.Validate(new TestClass { Property1 = null, Property2 = "12345", Property3 = "test" });

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().ErrorMessage, Is.StringContaining(ErrorMessage1));
        }

        [Test]
        public void Validate_Returns_ValidationResult_When_Passed_Object_That_Has_Two_Invalid_Properties()
        {
            var result = _validator.Validate(new TestClass { Property1 = null, Property2 = "test", Property3 = "test" });

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().ErrorMessage, Is.StringContaining(ErrorMessage1));
            Assert.That(result.Skip(1).First().ErrorMessage, Is.StringContaining(ErrorMessage2));
        }

        [Test]
        public void Validate_PropertyWithNoErrorMessage_CreatesErrorMessage()
        {
            var result = _validator.Validate(new TestClass { Property1 = null, Property2 = "test", Property3 = "" });

            var validationResults = result.ToList();

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(3, validationResults.Count);
            Assert.That(validationResults[0].ErrorMessage, Is.StringContaining(ErrorMessage1));
            Assert.That(validationResults[1].ErrorMessage, Is.StringContaining(ErrorMessage2));
            Assert.That(validationResults[2].ErrorMessage, Is.StringContaining("The Property3 field is required."));
        }

        [Test]
        public void Validate_MetaData()
        {
            var result = _validator.Validate(new TestClassWithMeta { Property1 = null, Property2 = "test", Property3 = "" });

            var validationResults = result.ToList();

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(3, validationResults.Count);
            Assert.That(validationResults[0].ErrorMessage, Is.StringContaining(ErrorMessage1));
            Assert.That(validationResults[1].ErrorMessage, Is.StringContaining(ErrorMessage2));
            Assert.That(validationResults[2].ErrorMessage, Is.StringContaining("The Property3 field is required."));
        }


        private class TestClass
        {
            [Required(ErrorMessage = ErrorMessage1)]
            public string Property1 { get; set; }

            [RegularExpression(@"\d{1,10}", ErrorMessage = ErrorMessage2)]
            public string Property2 { get; set; }

            [Required]
            public string Property3 { get; set; }
        }


        [MetadataType(typeof(SomeMetaData))]
        public class TestClassWithMeta
        {
            public string Property1 { get; set; }

            public string Property2 { get; set; }

            public string Property3 { get; set; }


            //[Serializable]
            public class SomeMetaData
            {
                [Required(ErrorMessage = ErrorMessage1)]
                public object Property1 { get; set; }

                [RegularExpression(@"\d{1,10}", ErrorMessage = ErrorMessage2)]
                public object Property2 { get; set; }

                [Required]
                public object Property3 { get; set; }
            }
        }
    }
}
