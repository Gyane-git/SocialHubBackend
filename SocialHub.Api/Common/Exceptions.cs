namespace SocialHub.Api.Common;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}

public class BadRequestException : Exception
{
    public List<string> Errors { get; } = new();

    public BadRequestException(string message) : base(message)
    {
    }

    public BadRequestException(string message, IEnumerable<string> errors) : base(message)
    {
        Errors.AddRange(errors);
    }
}

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}
