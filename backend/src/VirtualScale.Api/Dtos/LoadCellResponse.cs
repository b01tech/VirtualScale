using VirtualScale.Domain.Enums;

namespace VirtualScale.Api.Dtos;

public record LoadCellResponse(int Id, decimal RawValue, decimal Factor, CellStatus Status);
