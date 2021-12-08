using System;

namespace DrX.Utils.Generic
{
    public class LinkedList<T>
    {
        public int Length { get; private set; }
        public LinkedListNode<T> First { get; private set; }
        public LinkedListNode<T> Last { get; private set; }

        public T this[int i]
        {
            get { return Get(i); }
            set { SetAt(i, value); }
        }

        public LinkedList() { }

        /// <summary>
        /// Creates a copy of the items from the <paramref name="linkedList"/>. <br/>
        /// Beware: The items are copied with shallow copy. Do not use with reference types.
        /// </summary>
        /// <param name="linkedList"></param>
        public LinkedList(LinkedList<T> linkedList)
        {
            CopyLinkedList(linkedList);
        }

        /// <summary>
        /// Creates a copy of the items from the <paramref name="array"/>. <br/>
        /// Beware: The items are copied with shallow copy. Do not use with reference types.
        /// </summary>
        /// <param name="array"></param>
        public LinkedList(T[] array)
        {
            ArrayToLL(array);
        }

        /// <summary>
        /// Add the <paramref name="item"/> to the list
        /// </summary>
        /// <param name="item">The item to add</param>
        public void Add(T item)
        {
            LinkedListNode<T> newItem = new(item);
            AddToChain(newItem, Last, null);
        }

        /// <summary>
        /// Remove the specified <paramref name="node"/> from the list.<br/>
        /// The <paramref name="node"/> MUST be a reference to a node in this list
        /// </summary>
        /// <param name="node"></param>
        public void RemoveByNode(LinkedListNode<T> node)
        {
            RemoveFromChain(node);
        }

        /// <summary>
        /// Remove the first item which equals to the <paramref name="item"/>. (Checked by Equals())
        /// </summary>
        /// <param name="item">The item to remove</param>
        public void Remove(T item)
        {
            RemoveByData(item);
        }

        /// <summary>
        /// Remove the item pointed by the <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the item to be removed</param>
        public void RemoveAt(int index)
        {
            RemoveByIndex(index);
        }

        /// <summary>
        /// Appends the <paramref name="item"/> to the end of the list.
        /// </summary>
        /// <param name="item">The item to append</param>
        public void PushBack(T item)
        {
            Add(item);
        }

        /// <summary>
        /// Removes and return the last item from the list.
        /// </summary>
        /// <returns>The last item of the list</returns>
        public T PopBack()
        {
            if (Length == 0)
                throw new InvalidOperationException("List length is zero. Cannot pop an item.");

            T ret = Last.Data;
            RemoveFromChain(Last);
            return ret;
        }

        /// <summary>
        /// Get an item by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns>The data at <paramref name="index"/></returns>
        public T Get(int index)
        {
            return GetLLItemByIndex(index).Data;
        }

        /// <summary>
        /// Set an item value to <paramref name="data"/> at <paramref name="index"/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="data"></param>
        public void SetAt(int index, T data)
        {
            SetDataByIndex(index, data);
        }

        /// <summary>
        /// Check if the list contains any item which equals to <paramref name="data"/>
        /// </summary>
        /// <param name="data"></param>
        /// <returns>True if the list contains such item. False otherwise.</returns>
        public bool Contains(T data)
        {
            for (LinkedListNode<T> node = First; node != null; node = node.Next)
            {
                if (node.Data.Equals(data))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Find the <paramref name="data"/> in the list
        /// </summary>
        /// <param name="data">The data to search for</param>
        /// <returns>The first Node item which contains the <paramref name="data"/>. Null if not found</returns>
        public LinkedListNode<T> Find(T data)
        {
            return GetLLItemByData(data);
        }

        /// <summary>
        /// Clears the list
        /// </summary>
        public void Clear()
        {
            First = null;
            Last = null;
            Length = 0;
        }

        /// <summary>
        /// Converts this list to an equal array representation. Beware: the items copied by reference!
        /// </summary>
        /// <returns>The array representation of this list</returns>
        public T[] ToArray()
        {
            T[] arr = new T[Length];

            int idx = 0;
            for (LinkedListNode<T> node = First; node != null; node = node.Next)
            {
                arr[idx] = node.Data;
                idx++;
            }
            return arr;
        }

        /// <summary>
        /// Copy the <paramref name="linkedList"/> to this list
        /// </summary>
        /// <param name="linkedList"></param>
        private void CopyLinkedList(LinkedList<T> linkedList)
        {
            for (LinkedListNode<T> node = linkedList.First; node != null; node = node.Next)
            {
                LinkedListNode<T> newNode = new(node.Data);
                AddToChainBack(newNode);
            }
        }

        /// <summary>
        /// Copies an array to this list
        /// </summary>
        /// <param name="array"></param>
        private void ArrayToLL(T[] array)
        {
            if (array.Length > 0)
            {
                First = new LinkedListNode<T>(array[0]);
                AddToChain(First, null, null);

                LinkedListNode<T> PreviousItem = First;

                for (int i = 1; i < array.Length; i++)
                {
                    LinkedListNode<T> newItem = new(array[i]);
                    AddToChain(newItem, PreviousItem, null);
                    PreviousItem = newItem;
                }
            }
        }

        /// <summary>
        /// Remove the first item which equals to the <paramref name="data"/>. (Checked by Equals())
        /// </summary>
        /// <param name="data">The item to remove</param>
        private void RemoveByData(T data)
        {
            if (Length > 0)
            {
                LinkedListNode<T> toBeRemoved = GetLLItemByData(data);
                if (toBeRemoved != null)
                {
                    RemoveFromChain(toBeRemoved);
                }
            }
        }

        /// <summary>
        /// Remove the item pointed by the <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the item to be removed</param>
        private void RemoveByIndex(int index)
        {
            if (index < Length)
            {
                LinkedListNode<T> toBeRemoved = GetLLItemByIndex(index);
                if (toBeRemoved != null)
                {
                    RemoveFromChain(toBeRemoved);
                }
            }
        }

        /// <summary>
        /// Set an item value to <paramref name="data"/> at <paramref name="idx"/>
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="data"></param>
        private void SetDataByIndex(int idx, T data)
        {
            GetLLItemByIndex(idx).Data = data;
        }

        /// <summary>
        /// Find the <paramref name="data"/> in the list
        /// </summary>
        /// <param name="data">The data to search for</param>
        /// <returns>The first Node item which contains the <paramref name="data"/>. Null if not found</returns>
        private LinkedListNode<T> GetLLItemByData(T item)
        {
            LinkedListNode<T> llitem = First;
            while (llitem != null && !item.Equals(llitem.Data))
            {
                llitem = llitem.Next;
            }

            return llitem;
        }

        /// <summary>
        /// Get an Node item by index. The search is performed forwards in the first half and backwards in the second.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>The Node at <paramref name="index"/></returns>
        private LinkedListNode<T> GetLLItemByIndex(int index)
        {
            if (index < Length)
            {
                int firstHalf = index / 2;
                LinkedListNode<T> llitem;
                if (index <= firstHalf) // forward search
                {
                    llitem = SearchByIndexForward(index);
                }
                else // backward search
                {
                    llitem = SearchByIndexBackward(index);
                }
                return llitem;
            }
            throw new IndexOutOfRangeException();
        }

        /// <summary>
        /// Search forward for the <paramref name="index"/>
        /// </summary>
        /// <param name="index"></param>
        /// <returns>The Node at <paramref name="index"/></returns>
        private LinkedListNode<T> SearchByIndexForward(int index)
        {
            LinkedListNode<T> llitem = First;
            for (int i = 0; i < index; i++)
            {
                llitem = llitem.Next;
            }

            return llitem;
        }

        /// <summary>
        /// Search backward for the <paramref name="index"/>
        /// </summary>
        /// <param name="index"></param>
        /// <returns>The Node at <paramref name="index"/></returns>
        private LinkedListNode<T> SearchByIndexBackward(int index)
        {
            LinkedListNode<T> llitem = Last;
            for (int i = Length - 1; i > index; i--)
            {
                llitem = llitem.Prev;
            }

            return llitem;
        }

        /// <summary>
        /// Add a Node to the front of the chain
        /// </summary>
        /// <param name="node">The Node to add</param>
        private void AddToChainFront(LinkedListNode<T> node)
        {
            AddToChain(node, null, First);
        }

        /// <summary>
        /// Add a Node to the back of the chain
        /// </summary>
        /// <param name="node">The Node to add</param>
        private void AddToChainBack(LinkedListNode<T> node)
        {
            AddToChain(node, Last, null);
        }

        /// <summary>
        /// Add the <paramref name="node"/> between the <paramref name="previousNode"/> and the <paramref name="nextNode"/>
        /// </summary>
        /// <param name="node">The Node to add</param>
        /// <param name="previousNode">The previous Node</param>
        /// <param name="nextNode">The next Node</param>
        private void AddToChain(LinkedListNode<T> node, LinkedListNode<T> previousNode, LinkedListNode<T> nextNode)
        {
            if (previousNode != null)
                previousNode.Next = node;
            else
                First = node;

            if (nextNode != null)
                nextNode.Prev = node;
            else
                Last = node;

            node.Next = nextNode;
            node.Prev = previousNode;

            Length++;
        }

        /// <summary>
        /// Remove <paramref name="node"/> from the chain
        /// </summary>
        /// <param name="node">The Node to remove</param>
        private void RemoveFromChain(LinkedListNode<T> node)
        {
            if (node == null)
                throw new NullReferenceException();

            if (node.Prev != null)
                node.Prev.Next = node.Next;
            else
                First = node.Next;

            if (node.Next != null)
                node.Next.Prev = node.Prev;
            else
                Last = node.Prev;

            Length--;
        }
    }
}
