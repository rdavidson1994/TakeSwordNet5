using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TakeSwordTests")]
namespace TakeSword
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            World world = new();
            WorldSetup.Apply(world, out Entity playerEntity, out Entity startLocation);
            OutputEntry description = DescriptionUtilities.GetDescription(startLocation, playerEntity);
            Console.WriteLine(description.AsPlainText());

            while (true)
            {
                world.Run();
            }
        }
    }
}

