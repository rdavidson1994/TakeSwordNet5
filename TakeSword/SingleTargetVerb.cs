using System;
using System.Collections.Generic;

namespace TakeSword
{
    public record SingleTargetVerb<T>(
        Func<T, string, IReadOnlyList<T>> LookupTable,
        Func<T, T, IGameAction> ActionFactory,
        params string[] Synonyms
    ) : IVerb<T>
    {
        public IEnumerable<IGameAction> GetMatches(T subject, string sentence)
        {
            foreach (string word in Synonyms)
            {
                // Try to remove each possible word from the start of the sentence.
                string? sentenceWithoutVerb = sentence.StripPrefix(word);

                // If not possible, skip that word
                if (sentenceWithoutVerb is null)
                    continue;

                string targetName = sentenceWithoutVerb.Trim();

                var candidates = LookupTable(subject, targetName);
                if (candidates.Count == 0)
                {
                    yield return ActionOutcome.Failure($"I don't know what \"{targetName}\" is.").AsAction();
                    yield break;
                }
                foreach (T candidate in candidates)
                {
                    yield return ActionFactory(subject, candidate);
                }
            }
        }
    }
}
