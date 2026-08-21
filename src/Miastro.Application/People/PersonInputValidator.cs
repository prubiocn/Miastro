using System.Net.Mail;
using Miastro.Domain.Geography;
using Miastro.Domain.People;

namespace Miastro.Application.People;

public static class PersonInputValidator
{
    public static void Validate(
        CreatePersonCommand command)
    {
        ValidateCommon(
            command.FirstName,
            command.LastName,
            command.Phone,
            command.Email,
            command.PrivateNote,
            command.BirthData,
            command.CurrentResidence);
    }

    public static void Validate(
        UpdatePersonCommand command)
    {
        if (command.Id == Guid.Empty)
        {
            throw new ArgumentException(
                "Person id is required.",
                nameof(command));
        }

        ValidateCommon(
            command.FirstName,
            command.LastName,
            command.Phone,
            command.Email,
            command.PrivateNote,
            command.BirthData,
            command.CurrentResidence);
    }

    private static void ValidateCommon(
        string firstName,
        string lastName,
        string? phone,
        string? email,
        string? privateNote,
        BirthDataWriteModel? birth,
        CurrentResidenceWriteModel? residence)
    {
        Required(firstName, nameof(firstName), 120);
        Required(lastName, nameof(lastName), 120);
        Optional(phone, nameof(phone), 64);
        Optional(privateNote, nameof(privateNote), 10000);

        if (!string.IsNullOrWhiteSpace(email))
        {
            if (email.Trim().Length > 254)
            {
                throw new ArgumentException(
                    "Email is too long.",
                    nameof(email));
            }

            try
            {
                var parsed = new MailAddress(email.Trim());
                if (!string.Equals(
                    parsed.Address,
                    email.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    throw new FormatException();
                }
            }
            catch (FormatException)
            {
                throw new ArgumentException(
                    "Email syntax is invalid.",
                    nameof(email));
            }
        }

        if (birth is not null)
        {
            ValidateBirth(birth);
        }

        if (residence is not null)
        {
            Required(residence.Locality, "Residence.Locality", 200);
            Required(residence.Region, "Residence.Region", 160);
            Required(residence.Country, "Residence.Country", 120);
            _ = new Latitude(residence.Latitude);
            _ = new Longitude(residence.Longitude);
            _ = new IanaTimeZoneId(residence.IanaTimeZoneId);
        }
    }

    private static void ValidateBirth(
        BirthDataWriteModel birth)
    {
        Required(birth.Locality, "Birth.Locality", 200);
        Required(birth.Region, "Birth.Region", 160);
        Required(birth.Country, "Birth.Country", 120);

        _ = new Latitude(birth.Latitude);
        _ = new Longitude(birth.Longitude);
        _ = new IanaTimeZoneId(birth.IanaTimeZoneId);

        switch (birth.TimePrecision)
        {
            case BirthTimePrecision.Exact:
            case BirthTimePrecision.Approximate:
                if (birth.LocalTime is null)
                {
                    throw new ArgumentException(
                        "Concrete birth time is required.");
                }

                if (birth.ResolutionState
                    == BirthTemporalResolutionState.Ambiguous
                    && birth.AmbiguousSelectedCandidate is null)
                {
                    throw new ArgumentException(
                        "Ambiguous birth time requires explicit selection before save.");
                }

                if (birth.ResolutionState
                    == BirthTemporalResolutionState.Skipped)
                {
                    throw new ArgumentException(
                        "Skipped birth time must be corrected before save.");
                }
                break;

            case BirthTimePrecision.Range:
                if (birth.RangeStart is null
                    || birth.RangeEnd is null
                    || birth.RangeStart >= birth.RangeEnd)
                {
                    throw new ArgumentException(
                        "Birth time range is invalid.");
                }

                if (birth.ResolvedInstantUtc is not null)
                {
                    throw new ArgumentException(
                        "Range cannot persist a single resolved instant.");
                }
                break;

            case BirthTimePrecision.DayPeriod:
                if (birth.DayPeriod is null
                    || birth.ResolvedInstantUtc is not null)
                {
                    throw new ArgumentException(
                        "Day period must not contain a resolved instant.");
                }
                break;

            case BirthTimePrecision.Unknown:
                if (birth.LocalTime is not null
                    || birth.ResolvedInstantUtc is not null)
                {
                    throw new ArgumentException(
                        "Unknown birth time must not contain a time or instant.");
                }
                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(birth.TimePrecision));
        }
    }

    private static void Required(
        string value,
        string parameter,
        int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value)
            || value.Trim().Length > maxLength)
        {
            throw new ArgumentException(
                "Required value is invalid.",
                parameter);
        }
    }

    private static void Optional(
        string? value,
        string parameter,
        int maxLength)
    {
        if (value is not null && value.Trim().Length > maxLength)
        {
            throw new ArgumentException(
                "Optional value is too long.",
                parameter);
        }
    }
}
