#WCF Data Annotations

Validate your models over WCF!

The goal of this project is to allow you to have your validation run server (service) side, and have the validation results accessible through the thrown exception.

##Usage
Just place the `[ValidateDataAnnotationsBehavior]` attribute on your service class:

    [ValidateDataAnnotationsBehavior]
    public class PeopleService : IPeopleService
    {
        public void UpdatePerson(Person person)
        {
            // Your service code...
        }
    }

The service will now automatically validate the data annotations.

On the client-side you can catch the potential exception from the client:

    var person = new Person
    {
        Name = "John",
        Age = 10,
        SomethingElse = 12313123
    };
    
    var personServiceClient = new PeopleServiceClient();
    
    try
    {
        personServiceClient.UpdatePerson(person);
    }
    catch (FaultException<ObjectValidationFault> ex)
    {
        var objectValidationErrors = ex.Detail.ValidationErrors;
        
        // Display nice validation error messages to your users
    }
    catch (Exception ex)
    {
        // Catch other exceptions...
        // Don't swallow exceptions!
    }

## Credit

Original project was by DevTrends on [CodePlex](http://wcfdataannotations.codeplex.com)