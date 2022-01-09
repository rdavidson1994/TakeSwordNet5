using System;
using System.Collections.Generic;

namespace TakeSword
{
    public record SingleTargetVerbWithTool<T>(
        Func<T, string, IReadOnlyList<T>> LookupTable,
        Func<T, T, T, IGameAction> ActionFactory,
        params string[] Synonyms
    ) : IVerb<T>
    {
        public IEnumerable<IGameAction> GetMatches(T subject, string sentence)
        {
            string[] sentenceHalves = sentence.Split(" with ", 2);
            if (sentenceHalves.Length != 2)
            {
                yield break;
            }

            string remainingSentence = sentenceHalves[0].Trim();
            string toolName = sentenceHalves[1].Trim();
            string? targetName = null;

            foreach (string word in Synonyms)
            {
                // Try to remove each possible word from the start of the sentence.
                targetName = remainingSentence.StripPrefix(word)?.Trim();

                // If not possible, skip that word
                if (targetName is not null)
                {
                    break;
                }
            }

            if (targetName is null)
            {
                yield break;
            }

            var toolCandidates = LookupTable(subject, toolName);
            var targetCandidates = LookupTable(subject, targetName);
            if (targetCandidates.Count == 0)
            {
                yield return ActionOutcome.Failure($"I don't know what \"{targetName}\" is.").AsAction();
                yield break;
            }
            if (toolCandidates.Count == 0)
            {
                yield return ActionOutcome.Failure($"I don't know what \"{toolName}\" is.").AsAction();
                yield break;
            }

            foreach (T tool in toolCandidates)
            {
                foreach (T target in targetCandidates)
                {
                    yield return ActionFactory(subject, target, tool);
                }
            }
        }
    }
}
