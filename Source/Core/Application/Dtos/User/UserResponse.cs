using Application.Dtos.Order;

namespace Application.Dtos.User;

public record UserResponse(string Guid, DateTime RegistrationDate, long? VkUserId, string? Login, string? Password, string Name, string Contact, IEnumerable<Response> Orders);