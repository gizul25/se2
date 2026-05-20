namespace SE2.Data;

public class Grid
{
    public required string Image { get; set; }
    public required string City { get; set; }
    public int Size { get; set; }
    public required string Architecture { get; set; }

    public override string? ToString()
    {
        return $"{City},{Size},{Architecture}";
    }
}