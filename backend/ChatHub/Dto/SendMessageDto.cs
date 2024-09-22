namespace ChatHub.Dto
{
    public record SendMessageDto
    (
        Guid UserId,
        Guid ToUserId,
        string Message
    );
}
