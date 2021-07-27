using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TakeSwordTests")]

namespace TakeSword
{
    using System.Collections.Generic;
    using System.Text;
    using System.Text.Json;

    public class GameOutputUpdate
    {
        public List<OutputEntry> Messages { get; } = new();
        public List<OutputEntry> MapUpdates { get; } = new();
        public List<OutputEntry> SceneUpdates { get; } = new();

        public string AsJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public string AsPlainText()
        {
            StringBuilder output = new();
            if (MapUpdates.Count != 0)
            {
                foreach (InteractiveSpan span in MapUpdates[^1].Content)
                {
                    output.Append(span.Text);
                }
                output.Append("\n");
            }
            if (SceneUpdates.Count != 0)
            {
                foreach (InteractiveSpan span in SceneUpdates[^1].Content)
                {
                    output.Append(span.Text);
                }
                output.Append("\n");
            }
            foreach (OutputEntry entry in Messages)
            {
                foreach (InteractiveSpan span in entry.Content)
                {
                    output.Append(span.Text);
                }
                output.Append("\n");
            }
            return output.ToString();
        }
    }

}

