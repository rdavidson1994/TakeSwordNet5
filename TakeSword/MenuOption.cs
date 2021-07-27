using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TakeSwordTests")]

namespace TakeSword
{
    public record MenuOption(
        string Label,
        string Content
    );

}

