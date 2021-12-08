namespace DrX.Utils.Generic
{
    public class LinkedListNode<T>
    {
        public LinkedListNode<T> Prev { get; set; }
        public LinkedListNode<T> Next { get; set; }
        public T Data { get; set; }

        public LinkedListNode(T data)
        {
            Data = data;
        }
    }
}
