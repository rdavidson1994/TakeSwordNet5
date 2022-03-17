using System.Collections.Generic;

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
    public record Location()
    {
        public static bool DoesMatch(Entity self, Entity other)
        {
            if (!self.IsMember(out Entity? ownLocation, out Location _))
                return false;

            if (!other.IsMember<Location>(ownLocation))
                return false;

            return true;
        }
    }
    public record ItemTraits(int Weight);
    public record Liquid();
    public record FoodTraits(int Amount, bool IsBeverage = false)
    {
        public static implicit operator int(FoodTraits n) => n.Amount;
        public static implicit operator FoodTraits(int n) => new FoodTraits(n);
    }

    public abstract record Countable(int Amount) : ICountable
    {
        public static implicit operator int(Countable n) => n.Amount;
        public T Add<T>(int delta) where T : Countable
        {
            if (this is T derivedInstance)
            {
                return derivedInstance with { Amount = Amount + delta };
            }
            throw new ComponentException("Invalid - failed to cast component to derived type.");
        }
        //public static implicit operator Amount(int n) => new Amount(n);
    }

    public interface ICountable
    {
        int Amount { get; init; }

    }


    public record Health(int HitPoints) : Countable(HitPoints);
}

