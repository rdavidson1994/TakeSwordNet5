﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("TakeSwordTests")]

namespace TakeSword
{
    public class OutputTemplate
    {
        private readonly List<object> entries = new();
        public void Add(string text)
        {
            entries.Add(text);
        }

        public OutputTemplate AddFormat(FormattableString formattableString, string end = "\n")
        {
            var pieces = Regex.Split(formattableString.Format, @"{\d+}");
            object?[] arguments = formattableString.GetArguments() ?? new object?[] { };

            var argumentCount = formattableString.ArgumentCount;
            for (int i = 0; i < argumentCount; i++)
            {
                Add(pieces[i]);
                object? argument = arguments[i];
                if (argument is not null)
                {
                    entries.Add(argument);
                }
            }
            Add(pieces[argumentCount]);
            Add(end);
            return this;
        }

        public void AddLine(string line)
        {
            Add(line + "\n");
        }

        public void AddLines(List<string> lines)
        {
            foreach (string line in lines)
            {
                Add(line + "\n");
            }
        }

        public OutputEntry Render(Func<object, InteractiveSpan> symbolResolver)
        {
            OutputEntry output = new();
            foreach (var entry in entries)
            {
                if (entry is string stringEntry)
                {
                    output.Add(new InteractiveSpan(stringEntry));
                }
                else if (entry is InteractiveSpan spanEntry)
                {
                    output.Add(spanEntry);
                }
                else
                {
                    output.Add(symbolResolver(entry));
                }
            }
            return output;
        }
    }

    public class OutputEntry
    {
        public List<InteractiveSpan> Content { get; }
        public OutputEntry()
        {
            Content = new();
        }
        public void Add(string text)
        {
            Content.Add(new InteractiveSpan(text));
        }
        public void Add(InteractiveSpan span)
        {
            Content.Add(span);
        }
        public OutputEntry this[string text]
        {
            get
            {
                Add(text);
                return this;
            }
        }

        public void AddLine(string line)
        {
            Add(line + "\n");
        }

        public void AddLine(OutputEntry output)
        {
            Content.AddRange(output.Content);
            Add("\n");
        }

        public void AddLines(List<string> lines)
        {
            foreach (string line in lines)
            {
                Add(line + "\n");
            }
        }

        public string AsPlainText()
        {
            GameOutputUpdate update = new();
            update.SceneUpdates.Add(this);
            return update.AsPlainText();
        }
    }

}

