namespace BarberManagementSystem.DTOs.WorkingHours;

public class AvailableTimesResponseDto
{
    public bool isWorking { get; set; }
    public List<WorkingHourRangeDto> workingHours { get; set; } = new();
    public List<TimeRangeDto> breaks { get; set; } = new();
    public List<string> daysOff { get; set; } = new();
    public List<string> availableTimes { get; set; } = new();
}

public class WorkingHourRangeDto
{
    public string start { get; set; } = string.Empty;
    public string end { get; set; } = string.Empty;
}

public class TimeRangeDto
{
    public string start { get; set; } = string.Empty;
    public string end { get; set; } = string.Empty;
}

