using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel;
using Moq;
using NUnit.Framework;

namespace WCFDataAnnotations.UnitTests
{
    [TestFixture]
    public class ValidatingParameterInspectorTests
    {
        private const string OperationName = "DoSomething";

        private Mock<IObjectValidator> _singleValidatorMock;
        private Mock<IObjectValidator> _secondValidatorMock;
        private ValidatingParameterInspector _singleValidatorParameterInspector;
        private ValidatingParameterInspector _multipleValidatorsParameterInspector;

        [SetUp]
        public void Setup()
        {
            _singleValidatorMock = new Mock<IObjectValidator>();
            _singleValidatorMock.Setup(x => x.Validate(It.IsAny<object>())).Returns(Enumerable.Empty<ValidationResult>());

            _secondValidatorMock = new Mock<IObjectValidator>();
            _secondValidatorMock.Setup(x => x.Validate(It.IsAny<object>())).Returns(Enumerable.Empty<ValidationResult>());

            _singleValidatorParameterInspector = new ValidatingParameterInspector(new[] { _singleValidatorMock.Object });
            _multipleValidatorsParameterInspector = new ValidatingParameterInspector(new[] { _singleValidatorMock.Object, _secondValidatorMock.Object });
        }

        [Test]
        public void Must_Be_Supplied_With_Validators()
        {
            Assert.Throws<ArgumentNullException>(() => new ValidatingParameterInspector(null));
        }

        [Test]
        public void Validators_Cannot_Be_Empty()
        {
            Assert.Throws<ArgumentException>(() => new ValidatingParameterInspector(Enumerable.Empty<IObjectValidator>()));
        }

        [Test]
        public void BeforeCall_Calls_Validator_For_Single_Input()
        {
            var input = new object();
            _singleValidatorParameterInspector.BeforeCall(OperationName, new[] { input });
            _singleValidatorMock.Verify(x => x.Validate(input), Times.Once());
        }

        [Test]
        public void BeforeCall_Calls_Validator_For_Multiple_Inputs()
        {
            const string input1 = "hello";
            const string input2 = "goodbye";
            _singleValidatorParameterInspector.BeforeCall(OperationName, new[] { input1, input2 });
            _singleValidatorMock.Verify(x => x.Validate(input1), Times.Once());
            _singleValidatorMock.Verify(x => x.Validate(input2), Times.Once());
        }

        [Test]
        public void BeforeCall_Calls_Multiple_Validators_For_Single_Input()
        {
            var input = new object();
            _multipleValidatorsParameterInspector.BeforeCall(OperationName, new[] { input });
            _singleValidatorMock.Verify(x => x.Validate(input), Times.Once());
            _secondValidatorMock.Verify(x => x.Validate(input), Times.Once());
        }

        [Test]
        public void BeforeCall_Calls_Multiple_Validators_For_Multiple_Inputs()
        {
            const string input1 = "hello";
            const string input2 = "goodbye";
            _multipleValidatorsParameterInspector.BeforeCall(OperationName, new[] { input1, input2 });
            _singleValidatorMock.Verify(x => x.Validate(input1), Times.Once());
            _singleValidatorMock.Verify(x => x.Validate(input2), Times.Once());
            _secondValidatorMock.Verify(x => x.Validate(input1), Times.Once());
            _secondValidatorMock.Verify(x => x.Validate(input2), Times.Once());
        }

        [Test]
        public void BeforeCall_Throw_Exception_When_Validator_Returns_ValidationResult()
        {
            var validationResults = new List<ValidationResult> { new ValidationResult("something bad") };
            _singleValidatorMock.Setup(x => x.Validate(It.IsAny<object>())).Returns(validationResults);

            var faultException = Assert.Throws<FaultException<ObjectValidationFault>>(
                () => _singleValidatorParameterInspector.BeforeCall(OperationName, new[] { new object() }));
        }

        [Test]
        public void BeforeCall_Returns_Null_When_Valid()
        {
            var result = _singleValidatorParameterInspector.BeforeCall(OperationName, new[] { new object() });

            Assert.That(result, Is.Null);
        }
    }
}
