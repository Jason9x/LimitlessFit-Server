namespace LimitlessFit.Models
{
    public class DailyForecast(DateOnly date, int temperatureC, string? summary)
    {
        public DateOnly Date { get; set; } = date;
        private int TemperatureC { get; } = temperatureC;
        public string? Summary { get; set; } = summary;
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}