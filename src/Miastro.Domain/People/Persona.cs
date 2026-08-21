using System.Net.Mail;

namespace Miastro.Domain.People;

public sealed class Persona
{
    private readonly List<PersonHistoryEntry> _history = [];

    public Guid Id { get; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? PrivateNote { get; private set; }
    public bool IsFavorite { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; }
    public DateTimeOffset ModifiedAtUtc { get; private set; }
    public DateTimeOffset? LastConsultationAtUtc { get; private set; }

    public BirthData? BirthData { get; private set; }
    public CurrentResidence? CurrentResidence { get; private set; }

    public IReadOnlyList<PersonHistoryEntry> History => _history;

    private Persona(
        Guid id,
        string firstName,
        string lastName,
        DateTimeOffset nowUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "Person id cannot be empty.",
                nameof(id));
        }

        Id = id;
        FirstName = ValidateName(firstName, nameof(firstName));
        LastName = ValidateName(lastName, nameof(lastName));
        CreatedAtUtc = nowUtc.ToUniversalTime();
        ModifiedAtUtc = CreatedAtUtc;

        _history.Add(
            new PersonHistoryEntry(
                PersonHistoryEventType.Created,
                CreatedAtUtc,
                "Persona creada"));
    }

    public static Persona Create(
        string firstName,
        string lastName,
        DateTimeOffset nowUtc)
        => new(
            Guid.NewGuid(),
            firstName,
            lastName,
            nowUtc);

    public void UpdateIdentity(
        string firstName,
        string lastName,
        DateTimeOffset nowUtc)
    {
        FirstName = ValidateName(firstName, nameof(firstName));
        LastName = ValidateName(lastName, nameof(lastName));
        MarkRelevantEdit(nowUtc, "Identidad actualizada");
    }

    public void UpdateContact(
        string? phone,
        string? email,
        DateTimeOffset nowUtc)
    {
        Phone = ValidatePhone(phone);
        Email = ValidateEmail(email);
        MarkRelevantEdit(nowUtc, "Contacto actualizado");
    }

    public void UpdatePrivateNote(
        string? privateNote,
        DateTimeOffset nowUtc)
    {
        PrivateNote = ValidateOptional(privateNote, 10000);
        MarkRelevantEdit(nowUtc, "Nota privada actualizada");
    }

    public void SetFavorite(
        bool value,
        DateTimeOffset nowUtc)
    {
        if (IsFavorite == value)
        {
            return;
        }

        IsFavorite = value;
        MarkRelevantEdit(nowUtc, "Favorito actualizado");
    }

    public void SetBirthData(
        BirthData birthData,
        DateTimeOffset nowUtc)
    {
        BirthData = birthData
            ?? throw new ArgumentNullException(nameof(birthData));

        MarkRelevantEdit(nowUtc, "Datos de nacimiento actualizados");
    }

    public void SetCurrentResidence(
        CurrentResidence? residence,
        DateTimeOffset nowUtc)
    {
        CurrentResidence = residence;
        MarkRelevantEdit(nowUtc, "Residencia actualizada");
    }

    public void RecordConsultation(DateTimeOffset nowUtc)
    {
        LastConsultationAtUtc = nowUtc.ToUniversalTime();
        ModifiedAtUtc = LastConsultationAtUtc.Value;
    }

    private void MarkRelevantEdit(
        DateTimeOffset nowUtc,
        string summary)
    {
        ModifiedAtUtc = nowUtc.ToUniversalTime();

        _history.Add(
            new PersonHistoryEntry(
                PersonHistoryEventType.RelevantEdit,
                ModifiedAtUtc,
                summary));
    }

    private static string ValidateName(
        string value,
        string parameter)
    {
        var normalized = value?.Trim()
            ?? throw new ArgumentNullException(parameter);

        if (normalized.Length == 0)
        {
            throw new ArgumentException(
                "Name is required.",
                parameter);
        }

        if (normalized.Length > 120)
        {
            throw new ArgumentException(
                "Name is too long.",
                parameter);
        }

        return normalized;
    }

    private static string? ValidatePhone(string? value)
    {
        var normalized = ValidateOptional(value, 64);

        if (normalized is null)
        {
            return null;
        }

        if (normalized.Count(char.IsDigit) < 3)
        {
            throw new ArgumentException(
                "Phone value is not plausible.",
                nameof(value));
        }

        return normalized;
    }

    private static string? ValidateEmail(string? value)
    {
        var normalized = ValidateOptional(value, 254);

        if (normalized is null)
        {
            return null;
        }

        try
        {
            var parsed = new MailAddress(normalized);

            if (!string.Equals(
                parsed.Address,
                normalized,
                StringComparison.OrdinalIgnoreCase))
            {
                throw new FormatException();
            }
        }
        catch (FormatException)
        {
            throw new ArgumentException(
                "Email syntax is invalid.",
                nameof(value));
        }

        return normalized;
    }

    private static string? ValidateOptional(
        string? value,
        int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
        {
            throw new ArgumentException(
                $"Value exceeds {maxLength} characters.");
        }

        return normalized;
    }
}
