using System.Net.Mail;
using Miastro.Domain.People;

namespace Miastro.UI.Avalonia.ViewModels;

public sealed partial class MainWindowViewModel
{
    private string _firstNameError = string.Empty;
    private string _lastNameError = string.Empty;
    private string _emailError = string.Empty;
    private string _birthDateError = string.Empty;
    private string _birthTimeError = string.Empty;
    private string _birthLocationError = string.Empty;
    private string _residenceLocationError = string.Empty;

    public string FirstNameError
    {
        get => _firstNameError;
        private set
        {
            if (SetField(ref _firstNameError, value))
            {
                OnPropertyChanged(nameof(HasFirstNameError));
            }
        }
    }

    public bool HasFirstNameError
        => !string.IsNullOrWhiteSpace(FirstNameError);

    public string LastNameError
    {
        get => _lastNameError;
        private set
        {
            if (SetField(ref _lastNameError, value))
            {
                OnPropertyChanged(nameof(HasLastNameError));
            }
        }
    }

    public bool HasLastNameError
        => !string.IsNullOrWhiteSpace(LastNameError);

    public string EmailError
    {
        get => _emailError;
        private set
        {
            if (SetField(ref _emailError, value))
            {
                OnPropertyChanged(nameof(HasEmailError));
            }
        }
    }

    public bool HasEmailError
        => !string.IsNullOrWhiteSpace(EmailError);

    public string BirthDateError
    {
        get => _birthDateError;
        private set
        {
            if (SetField(ref _birthDateError, value))
            {
                OnPropertyChanged(nameof(HasBirthDateError));
            }
        }
    }

    public bool HasBirthDateError
        => !string.IsNullOrWhiteSpace(BirthDateError);

    public string BirthTimeError
    {
        get => _birthTimeError;
        private set
        {
            if (SetField(ref _birthTimeError, value))
            {
                OnPropertyChanged(nameof(HasBirthTimeError));
            }
        }
    }

    public bool HasBirthTimeError
        => !string.IsNullOrWhiteSpace(BirthTimeError);

    public string BirthLocationError
    {
        get => _birthLocationError;
        private set
        {
            if (SetField(ref _birthLocationError, value))
            {
                OnPropertyChanged(nameof(HasBirthLocationError));
            }
        }
    }

    public bool HasBirthLocationError
        => !string.IsNullOrWhiteSpace(BirthLocationError);

    public string ResidenceLocationError
    {
        get => _residenceLocationError;
        private set
        {
            if (SetField(ref _residenceLocationError, value))
            {
                OnPropertyChanged(nameof(HasResidenceLocationError));
            }
        }
    }

    public bool HasResidenceLocationError
        => !string.IsNullOrWhiteSpace(ResidenceLocationError);

    public bool ValidateEditor()
    {
        ClearValidationErrors();

        var valid = true;

        if (string.IsNullOrWhiteSpace(FirstName))
        {
            FirstNameError =
                "El nombre es obligatorio.";
            valid = false;
        }
        else if (FirstName.Trim().Length > 120)
        {
            FirstNameError =
                "El nombre es demasiado largo.";
            valid = false;
        }

        if (string.IsNullOrWhiteSpace(LastName))
        {
            LastNameError =
                "Los apellidos son obligatorios.";
            valid = false;
        }
        else if (LastName.Trim().Length > 120)
        {
            LastNameError =
                "Los apellidos son demasiado largos.";
            valid = false;
        }

        if (!string.IsNullOrWhiteSpace(Email))
        {
            try
            {
                var value = Email.Trim();

                if (value.Length > 254)
                {
                    throw new FormatException();
                }

                var parsed =
                    new MailAddress(value);

                if (!string.Equals(
                    parsed.Address,
                    value,
                    StringComparison.OrdinalIgnoreCase))
                {
                    throw new FormatException();
                }
            }
            catch (FormatException)
            {
                EmailError =
                    "Introduce un email válido.";
                valid = false;
            }
        }

        if (HasBirthData)
        {
            if (BirthDate is null)
            {
                BirthDateError =
                    "Indica la fecha de nacimiento.";
                valid = false;
            }

            if (_birthGeoNameId is null)
            {
                BirthLocationError =
                    "Selecciona una localidad de nacimiento.";
                valid = false;
            }

            switch (SelectedBirthPrecision.Value)
            {
                case BirthTimePrecision.Exact:
                case BirthTimePrecision.Approximate:
                    if (!TimeOnly.TryParse(
                        BirthTimeText,
                        out _))
                    {
                        BirthTimeError =
                            "Introduce la hora en formato HH:mm.";
                        valid = false;
                    }
                    else if (_resolvedBirthSnapshot is null)
                    {
                        BirthTimeError =
                            BirthSkipped
                                ? "Corrige la hora: esa hora local no existió."
                                : BirthAmbiguous
                                    ? "Elige una de las dos posibilidades."
                                    : "Resuelve la hora histórica antes de guardar.";

                        valid = false;
                    }
                    break;

                case BirthTimePrecision.Range:
                    if (!TimeOnly.TryParse(
                            BirthRangeStartText,
                            out var start)
                        || !TimeOnly.TryParse(
                            BirthRangeEndText,
                            out var end)
                        || start >= end)
                    {
                        BirthTimeError =
                            "El rango horario no es válido.";
                        valid = false;
                    }
                    break;

                case BirthTimePrecision.DayPeriod:
                case BirthTimePrecision.Unknown:
                    break;

                default:
                    BirthTimeError =
                        "Selecciona una precisión horaria válida.";
                    valid = false;
                    break;
            }
        }

        if (HasResidence
            && _residenceGeoNameId is null)
        {
            ResidenceLocationError =
                "Selecciona una localidad de residencia.";
            valid = false;
        }

        if (!valid)
        {
            ErrorMessage =
                "Revisa los campos marcados.";

            OnPropertyChanged(
                nameof(HasError));

            StatusMessage =
                "No guardado";
        }

        return valid;
    }

    public void ClearValidationErrors()
    {
        FirstNameError = string.Empty;
        LastNameError = string.Empty;
        EmailError = string.Empty;
        BirthDateError = string.Empty;
        BirthTimeError = string.Empty;
        BirthLocationError = string.Empty;
        ResidenceLocationError = string.Empty;
    }
}
