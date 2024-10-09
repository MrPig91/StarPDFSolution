[Flags]
public enum CombinePDFOptions
{
    None = 1,
    RemoveComments = 2,
    RemoveBookmarks = 4
}

public static class CombinePDFOptionsExtensions
{
    public static CombinePDFOptions AddFlag(this CombinePDFOptions options, CombinePDFOptions flag)
    {
        if (options.HasFlag(flag) == false)
            options = options | flag;
        return options;
    }
}