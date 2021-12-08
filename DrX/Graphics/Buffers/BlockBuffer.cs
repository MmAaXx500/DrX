using DrX.Utils;

namespace DrX.Graphics.Buffers
{
    /// <summary>
    /// Represents a BlockBuffer. <br/>
    /// BlockBuffers are contains a matrix of Blocks.<br/>
    /// These buffers have a block size which determines the dimensions of the Blocks contained in it.<br/>
    /// They also have a buffer size which determines how many rows and cols of Blocks are contained in it.
    /// </summary>
    public class BlockBuffer
    {
        public Dimension BufferSize { get; }
        public Dimension BlockSize { get; }
        public Block[,] Blocks { get; set; }

        public BlockBuffer(Dimension bufferSize, Dimension blockSize)
        {
            BufferSize = bufferSize;
            BlockSize = blockSize;
            Blocks = new Block[bufferSize.Rows, bufferSize.Cols];
            for (int i = 0; i < Blocks.GetLength(0); i++)
            {
                for (int j = 0; j < Blocks.GetLength(1); j++)
                {
                    Blocks[i, j] = new Block(blockSize);
                }
            }
        }

        public BlockBuffer(BlockBuffer bf)
        {
            BufferSize = bf.BufferSize;
            BlockSize = bf.BlockSize;
            Blocks = new Block[BufferSize.Rows, BufferSize.Cols];
            for (int row = 0; row < BufferSize.Rows; row++)
            {
                for (int col = 0; col < BufferSize.Cols; col++)
                {
                    Blocks[row, col] = new Block(bf.Blocks[row, col]);
                }
            }
        }
    }
}
