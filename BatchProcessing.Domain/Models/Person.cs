namespace BatchProcessing.Domain.Models;

/// <summary>
/// Represents a person with a first name, last name, date of birth, and address.
/// </summary>
public class Person
{
    /// <summary>
    /// Gets or sets the first name of the person.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name of the person.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the date of birth of the person.
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets the address of the person.
    /// </summary>
    public Address Address { get; set; }
}
