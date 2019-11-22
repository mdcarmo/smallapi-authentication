using FluentValidation;

namespace SmallApiAuthentication.Dto.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        /// <summary>
        /// 
        /// </summary>
        public UserDtoValidator()
        {
            RuleFor(o => o.UserName).NotEmpty().WithMessage("UserName is required");
            RuleFor(o => o.Password).NotEmpty().WithMessage("Password is required");
        }
    }
}
