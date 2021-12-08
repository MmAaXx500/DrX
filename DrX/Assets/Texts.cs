namespace DrX.Assets
{
    /// <summary>
    /// Static class of <c>LineBuffer</c> compatibile strings used by the graphics engine
    /// </summary>
    public static class Texts
    {
        public static string HowManyPlayers { get; } = "Hányan szeretnétek játszani? (%min%-%max%)";
        public static string AskName { get; } = "Adj a karakterednek egy nevet!";
        public static string StartPlace { get; } = "Melyik mezőről szeretél indulni %pname%? (%spos%)";
        public static string MovePlayer { get; } = "Melyik mezőre szeretél lépni %pname%? (%spos%, space = egyelyben maradás)";
        public static string AvailableSteps { get; } = "Válassz egy mezőt:\n%steps%";
        public static string RevealDrX { get; } = "°b04DrX itt tartózkodik: %drxpos%°b--";
        public static string StatusLine { get; } = "Kör: %round%.  %statustext%";
        public static string PreGameStatusLine { get; } = "%statustext%";
        public static string DrXWin { get; } = "°f12Ezt a játékot DrX nyerte %round% kör után (%drxpos%)°f--";
        public static string PlayersWin { get; } = "°f10Elkaptátok DrX-et %round% kör után (%drxpos%)°f--";
        public static string EnterExit { get; } = "\n\nNyomj entert a kilépéshez";
    }
}
