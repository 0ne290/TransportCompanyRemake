namespace Application.Dtos;

public record UserResponse(string Guid, DateTime RegistrationDate, long? VkUserId, string? Login, string? Password, string Name, string Contact, IEnumerable<OrderResponse> Orders);