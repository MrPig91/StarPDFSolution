[Flags]
public enum SplitPDFOptions
{
    None = 1,
    RemoveComments = 2,
    RemoveBookmarks = 4
}

public static class SplitPDFOptionsExtensions
{
    public static SplitPDFOptions AddFlag(this SplitPDFOptions options, SplitPDFOptions flag)
    {
        if (options.HasFlag(flag) == false)
            options = options | flag;
        return options;
    }
}