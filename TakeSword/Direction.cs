using System;
using System.Collections.Generic;
using System.Linq;

namespace TakeSword
{
    public enum Direction
    {
        None = 0,
        North = 1,
        South = -1,
        East = 3,
        West = -3,
        Up = 9,
        Down = -9,
    }

    public static class DirectionExtensions
    {
        public static IEnumerable<Direction> EnumerateDirections()
        {
            yield return Direction.North;
            yield return Direction.South;
            yield return Direction.East;
            yield return Direction.West;
            yield return Direction.Up;
            yield return Direction.Down;
        }
        public static Direction Opposite(this Direction direction)
        {
            return (Direction)(-(int)direction);
        }

        public static string Name(this Direction direction)
        {
            return direction switch
            {
                Direction.North => "north",
                Direction.South => "south",
                Direction.East => "east",
                Direction.West => "west",
                Direction.Up => "up",
                Direction.Down => "down",
                _ => "invalid direction"
            };
        }
    }

    public static class DirectionConverter
    {
        static readonly Dictionary<string, Direction> letterToDirection = new Dictionary<string, Direction>
        {
            {"n", Direction.North },
            {"s", Direction.South },
            {"e", Direction.East },
            {"w", Direction.West },
            {"u", Direction.Up },
            {"d", Direction.Down },
        };

        static readonly string[] directionNames = { "north", "south", "east", "west", "up", "down" };
        public static Direction FromString(string str)
        {
            string key;
            if (str.Length != 1 && directionNames.Contains(str))
            {
                key = str[0].ToString();
            }
            else
            {
                key = str;
            }
            if (letterToDirection.TryGetValue(key, out Direction direction))
            {
                return direction;
            }
            return Direction.None;
        }
    }
}
