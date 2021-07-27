using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TakeSwordTests")]

namespace TakeSword
{
    public record Visibility();
    public record Name(string Value)
    {
        public static implicit operator string(Name n) => n.Value;
        public static implicit operator Name(string s) => new Name(s);
    }
    public record Senses();
    public record SceneDescription(List<string> Lines);
    public record Location();
    public record ItemTraits(int Weight);
    public record Liquid();
    public record FoodTraits(int Amount, bool IsBeverage = false)
    {
        public static implicit operator int(FoodTraits n) => n.Amount;
        public static implicit operator FoodTraits(int n) => new FoodTraits(n);
    }
}

