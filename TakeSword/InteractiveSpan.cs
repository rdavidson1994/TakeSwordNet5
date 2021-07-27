using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TakeSwordTests")]

namespace TakeSword
{
    using System.Collections.Generic;

    public class InteractiveSpan
    {
        public InteractiveSpan(string text, List<MenuOption>? options = null)
        {
            Text = text;
            Options = options;
        }
        public string Text { get; set; }
        public List<MenuOption>? Options { get; set; }
    }
}

