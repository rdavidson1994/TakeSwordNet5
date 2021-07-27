using System.Collections.Generic;

namespace TakeSword
{
    public record VerbSuite<T>(IReadOnlyList<IVerb<T>> Verbs)
    {
        public IEnumerable<IGameAction> GetMatches(T subject, string input)
        {
            foreach (var verb in Verbs)
            {
                foreach (var action in verb.GetMatches(subject, input))
                {
                    yield return action;
                }
            }
        }
    }
}
