namespace FootballTvPlanner.Api.Data;

public class SeedSettings
{
    public static string SectionName { get; } = "Seed";

    public required List<SeedUser> Users { get; set; }

    public class SeedUser
    {
        public required string EmailAddress { get; set; }

        public string? Name { get; set; }

        public required string Password { get; set; }
    }
}
