namespace TakeSword
{
    using System;
    using static ActionOutcome;

    public record QuitGameCommand : IGameAction
    {
        public ActionOutcome Execute(bool dryRun = false)
        {
            Console.WriteLine("Thanks for playing!");
            Environment.Exit(0);
            return Failure("Your last action quit the game, so now it's your turn to act.");
        }
    }
}
