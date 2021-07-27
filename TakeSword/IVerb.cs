using System.Collections.Generic;

namespace TakeSword
{
    public interface IVerb<T>
    {
        IEnumerable<IGameAction> GetMatches(T subject, string sentence);
    }
}
