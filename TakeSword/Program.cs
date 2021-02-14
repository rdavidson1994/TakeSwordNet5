using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using TakeSword;
using System.Runtime.CompilerServices;

namespace TakeSword
{
    public enum Instrument
    {
        SineWave,
        SquareWave,
        Violin,
        Guitar,
    }

    public static class Durations
    {
        public const int ThirtySecondNote = 125;
        public const int SixteenthNote = 250;
        public const int EighthNote = 500;
        public const int QuarterNote = 1000;
        public const int HalfNote = 2000;
        public const int WholeNote = 4000;
    }
    

    public record Position(
        int X,
        int Y,
        int Z
    );

    public record MusicalNote(
        int Pitch,
        int Duration = Durations.QuarterNote,
        Instrument Instrument = Instrument.SineWave,
        double Volume = 1.0
    );

    public class ValueArray<T>
    {
        private ImmutableArray<T> content;


        public ValueArray(ImmutableArray<T> content)
        {
            this.content = content;
        }

        public override bool Equals(object? obj)
        {
            return obj is ValueArray<T> array &&
                   content.SequenceEqual(array.content);
        }

        public override int GetHashCode()
        {
            HashCode hashCode = new();
            foreach (T item in content)
            {
                hashCode.Add(item);
            }
            return hashCode.ToHashCode();
        }
    }

    public class Mind
    {

    }

    public record Health(int HPMax, int HP);

    public record Name(string Data);

    public record Afraid(int Duration);

    public record FearImmune(int Duration);
    public record Lycanthropy();
    public record WerewolfState(int Duration);
    public static class Program
    {
        public static void Whatever(Action<ITuple> tupleAction)
        {

        }

        public static void Main(string[] args)
        {
            var myNote = new MusicalNote(
                Pitch: 400,
                Durations.HalfNote,
                Instrument.SineWave
            );
            var writableNote = new Edit<MusicalNote>(myNote);
            Console.WriteLine(writableNote.Value.Pitch);
            WriteableUtil.Destroy(ref writableNote);
            // Gives a warning, as desired: 
            // Console.WriteLine(writableNote.Value.Pitch);
            World world = new World();
            world.RegisterComponent<MusicalNote>();
            world.RegisterComponent<Position>();
            world.RegisterComponent<Health>();
            world.RegisterComponent<Name>();
            world.RegisterComponent<Mind>();
            world.RegisterComponent<Afraid>();
            world.RegisterComponent<FearImmune>();
            world.RegisterSparseComponent<Lycanthropy>();
            world.RegisterSparseComponent<WerewolfState>();
            EntityId aliceId = world.CreateEntity(
                new MusicalNote(440),
                new Position(10, 20, 30),
                new Lycanthropy(),
                new Name("Alice")
            );
            EntityId bobId = world.CreateEntity(
                new Health(100, 20),
                new Mind(),
                new Name("Bob"),
                new Lycanthropy()
            );
            world.InstallSystem<MusicalNote, Position>((id, note, p) =>
            {
                Console.WriteLine($"You hear a {note.Pitch} Hz note at ({p.X},{p.Y},{p.Z}), with volume={note.Volume}.");
                world.SetComponent<MusicalNote>(id, note with { Volume = note.Volume * 0.5 });
            });

            world.InstallSystem((EntityId id, Lycanthropy lycanthropy, Create<WerewolfState> werewolfState, Optional<Name> name) =>
            {
                bool isFullMoon = new Random().Next(0, 1) == 0;
                if (!isFullMoon || werewolfState.Value != null)
                {
                    return;
                }
                if (name.Value != null)
                {
                    Console.WriteLine($"{name.Value.Data} is becoming a werewolf!");
                }
                else
                {
                    Console.WriteLine("Becoming a werewolf!");
                }
                werewolfState.Write(new WerewolfState(10));
            });
            
            world.InstallSystem((EntityId id, Edit<WerewolfState> werewolfState) =>
            {
                int d = werewolfState.Value.Duration;
                if (d == 0)
                {
                    werewolfState.Destroy();
                }
                else
                {
                    werewolfState.Write(werewolfState.Value with { Duration = d - 1 });
                }
            });

            world.InstallSystem((EntityId id, Edit<Health> health, Name name, Create<Afraid> afraid) =>
            {
                if (afraid.Value is not null)
                {
                    return;
                }
                if (health.Value.HP < health.Value.HPMax / 2)
                {
                    Console.WriteLine($"{name.Data} becomes shaken!");
                    afraid.Write(new Afraid(3));
                }
            });

            world.InstallSystem((EntityId id, Edit<Afraid> afraid, Name name, Create<FearImmune> fearImmune) =>
            {
                if (afraid.Value.Duration == 0)
                {
                    afraid.Destroy();
                    fearImmune.Write(new FearImmune(3));
                }
                else
                {
                    Console.WriteLine($"{name.Data} cowers in fear!");
                    afraid.Write(afraid.Value with { Duration = afraid.Value.Duration - 1 });
                    //world.SetComponent(id, afraid with { Duration = afraid.Duration - 1 });
                }
            });
            world.Run();
            world.Run();
            world.SetComponent(aliceId, new MusicalNote(432));
            world.Run();
            world.Run();
            world.Run();
            world.Run();
            world.RemoveComponent<MusicalNote>(aliceId);
            world.Run();
            world.Run();
            world.Run();

            world.Run();
            world.Run();
            world.Run();
            Console.WriteLine("Killing bob...");
            world.DestroyEntity(bobId);
            EntityId charlie = world.CreateEntity();
        }
    }

}

