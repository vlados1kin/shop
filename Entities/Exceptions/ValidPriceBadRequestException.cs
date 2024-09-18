namespace Entities.Exceptions;

public sealed class ValidPriceBadRequestException : BadRequestException
{
    public ValidPriceBadRequestException() : base("Max price can't be less than min price.")
    {
    }
}