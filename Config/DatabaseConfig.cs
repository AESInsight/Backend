namespace Backend.Config;

public class DatabaseConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public int MaxPoolSize { get; set; } = 3;
    public int MinPoolSize { get; set; } = 1;
    public int ConnectionLifetime { get; set; } = 300; // 300 seconds
} 