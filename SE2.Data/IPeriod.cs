namespace SE2.Data;

public interface IPeriod
{
    public string Period();
}

public class Winter : IPeriod
{
    public string Period() => "winter";
}

public class Summer : IPeriod
{
    public string Period() => "summer";
}