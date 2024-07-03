using BatchProcessing.Domain.Models;
using Bogus;
using Person = BatchProcessing.Domain.Models.Person;

namespace BatchProcessing.Grains.Services;

internal static class BogusService
{
    public static List<BatchProcessItem> Generate(Guid batchId, int numberOfRecords)
    {
        var batchProcessItemFaker = new Faker<BatchProcessItem>()
            .RuleFor(b => b.Id, f => f.Random.Guid())
            .RuleFor(b => b.BatchProcessId, f  => batchId)
            .RuleFor(b => b.Status, f => BatchProcessItemStatusEnum.Created)
            .RuleFor(b => b.CreatedOnUtc, f => DateTime.UtcNow)
            .RuleFor(b => b.Person, f => GeneratePerson());

        return batchProcessItemFaker.Generate(numberOfRecords);
    }

    public static Person GeneratePerson()
    {
        var personFaker = new Faker<Person>()
            .RuleFor(p => p.FirstName, f => f.Name.FirstName())
            .RuleFor(p => p.LastName, f => f.Name.LastName())
            .RuleFor(p => p.DateOfBirth, f => f.Date.Between(DateTime.Now.AddYears(-90), DateTime.Now))
            .RuleFor(p => p.Address, f => GenerateAddress());

        return personFaker.Generate();
    }

    public static Address GenerateAddress()
    {
        var addressFaker = new Faker<Address>()
            .RuleFor(a => a.StreetAddress, f => f.Address.StreetAddress())
            .RuleFor(a => a.City, f => f.Address.City())
            .RuleFor(a => a.State, f => f.Address.State())
            .RuleFor(a => a.PostalCode, f => f.Address.ZipCode())
            .RuleFor(a => a.Country, f => f.Address.Country());

        return addressFaker.Generate();
    }
}