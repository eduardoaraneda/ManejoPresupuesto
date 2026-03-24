using Microsoft.AspNetCore.Identity;

namespace ManejoPresupuesto.Servicios
{
    public class MensajesDeErrorIdentity : IdentityErrorDescriber
    {
        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = $"La contraseña debe tener al menos {length} caracteres."
            };
        }
        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = "La contraseña debe contener al menos un carácter no alfanumérico."
            };
        }
        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = "La contraseña debe contener al menos un dígito ('0'-'9')."
            };
        }
        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUpper),
                Description = "La contraseña debe contener al menos una letra mayúscula ('A'-'Z')."
            };
        }
        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresLower),
                Description = "La contraseña debe contener al menos una letra minúscula ('a'-'z')."
            };


        }
    }
}
