namespace TakeSword
{
    public static class DowntimeUtilities
    {
        public static IGameAction GetDowntimeActivity(Entity Actor, double diceRoll)
        {
            WeightedValidatingSelector selector = new();
            selector.AddEntry(new SingAction(Actor), 10);
            selector.AddEntry(new DanceAction(Actor), 5);
            return selector.SelectActivity(Actor, diceRoll);
        }
    }
}

