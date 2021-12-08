namespace DrX.GraphHopper.Containers
{
    /// <summary>
    /// 0xLTRB<br/>
    /// L: left, T: top, R: right, B: bottom
    /// </summary>
    /// <remarks>
    /// <code>
    /// Left/Right:<br/>
    ///     0: Empty<br/>
    ///     1: Top<br/>
    ///     2: Center<br/>
    ///     3: Bottom<br/>
    ///     <br/>
    /// Top/Bottom:<br/>
    ///     0: Empty<br/>
    ///     1: Left<br/>
    ///     2: Center<br/>
    ///     3: Right<br/>
    /// </code>
    /// </remarks>
    public enum PathSegment
    {
        Empty = 0x0000,
        Horizontal = 0x2020,
        Vertical = 0x0202,
        BottomLeftToTopRight = 0x3311,
        TopLeftToBottomRight = 0x1133,
        BottomLeftToRight = 0x3021,
        TopLeftToRight = 0x1120,
        LeftToTopRight = 0x2310,
        LeftToBottomRight = 0x2033,
        BottomLeftToTop = 0x3201,
        TopLeftToBottom = 0x1102,
        TopToBottomRight = 0x0233,
        BottomToTopRight = 0x0312,
        LeftToTop = 0x2200,
        LeftToBottom = 0x2002,
        TopToRight = 0x0220,
        BottomToRight = 0x0022
    }
}
