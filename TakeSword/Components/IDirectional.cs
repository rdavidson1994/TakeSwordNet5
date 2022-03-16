namespace TakeSword
{
    public interface IDirectional<out T>
    {
        T North { get; }
        T South { get; }
        T East { get; }
        T West { get; }
        T Up { get; }
        T Down { get; }
    }

    public static class IDirectionalExtensions
    {
        public static T GetFacing<T>(this IDirectional<T> directional, Direction direction)
        {
            return direction switch
            {
                Direction.North => directional.North,
                Direction.South => directional.South,
                Direction.West => directional.West,
                Direction.East => directional.East,
                Direction.Up => directional.Up,
                Direction.Down => directional.Down,
                _ => throw new System.NotImplementedException()
            };
        }
    }
}
