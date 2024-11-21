using Core.Entities;

namespace Core.Models;

public record AlertServiceStatusDto(bool IsRunning,
int PeriodInMinutes,
DateTime StartTime);
