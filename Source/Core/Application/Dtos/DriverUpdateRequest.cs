using Domain.Entities;

namespace Application.Dtos;

public record DriverUpdateRequest(string DriverGuid, IEnumerable<Action<Driver>> Updater);