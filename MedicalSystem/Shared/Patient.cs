namespace Shared;

public class Patient
{
    public long Id { get; set; }
    public string CardNumber { get; set; } = null!;
    public string? OmsNumber { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string? Patronymic { get; set; }
    public DateTime BirthDate { get; set; }
    public string? Phone { get; set; }
    public string Gender { get; set; } = null!;
    public string? Address { get; set; }
    public string? Email { get; set; }
    public string? Allergies { get; set; }
    public string? ChronicDiseases { get; set; }
    public DateTime CreatedDate { get; set; }

}
