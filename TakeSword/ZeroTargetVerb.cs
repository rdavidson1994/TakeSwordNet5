using System;
using System.Collections.Generic;
using System.Linq;

namespace TakeSword
{
    public record ZeroTargetVerb<T>(
        Func<T, IGameAction> ActionFactory,
        params string[] Synonyms
    ) : IVerb<T>
    {
        public IEnumerable<IGameAction> GetMatches(T subject, string sentence)
        {
            if (Synonyms.Any(x => x.Equals(sentence)))
            {
                yield return ActionFactory(subject);
            }
        }
    }
}
