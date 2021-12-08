using System;

namespace DrX.Utils.Generic
{
    public class List<T>
    {
        private T[] internalArray = Array.Empty<T>();

        public int Length => internalArray.Length;

        public T this[int i]
        {
            get { return internalArray[i]; }
            set { internalArray[i] = value; }
        }

        public List() { }

        /// <summary>
        /// Creates a copy of the items from the <paramref name="list"/>. <br/>
        /// Beware: The items are copied with shallow copy. Do not use with reference types.
        /// </summary>
        /// <param name="list"></param>
        public List(List<T> list)
        {
            CopyList(list);
        }

        /// <summary>
        /// Creates a copy of the items from the <paramref name="array"/>. <br/>
        /// Beware: The items are copied with shallow copy. Do not use with reference types.
        /// </summary>
        /// <param name="array"></param>
        public List(T[] array)
        {
            ArrayToList(array);
        }

        /// <summary>
        /// Add the <paramref name="item"/> to the list
        /// </summary>
        /// <param name="item">The item to add</param>
        public void Add(T item)
        {
            Append(item);
        }

        /// <summary>
        /// Add the <paramref name="item"/> to the list at the specified <paramref name="index"/>. The list is shifted, not overwritten.
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <param name="index"></param>
        public void AddAt(T item, int index)
        {
            if (index > Length && index >= 0) // index == Length is allowed because we can add to the end of the list
                throw new ArgumentOutOfRangeException(null);

            AddSpace(ref internalArray, index);
            internalArray[index] = item;
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
            if (index >= Length && index >= 0)
                throw new ArgumentOutOfRangeException(null);

            RemoveByIndex(index);
        }

        /// <summary>
        /// Appends the <paramref name="item"/> to the end of the list.
        /// </summary>
        /// <param name="item">The item to append</param>
        public void PushBack(T item)
        {
            Append(item);
        }

        /// <summary>
        /// Removes and return the last item from the list.
        /// </summary>
        /// <returns>The last item of the list</returns>
        public T PopBack()
        {
            if (internalArray.Length == 0)
                throw new InvalidOperationException("List length is zero. Cannot pop an item.");

            T ret = internalArray[Length - 1];
            RemoveSpace(ref internalArray, Length - 1);
            return ret;
        }

        /// <summary>
        /// Check if the list contains any item which equals to <paramref name="data"/>
        /// </summary>
        /// <param name="data"></param>
        /// <returns>True if the list contains such item. False otherwise.</returns>
        public bool Contains(T data)
        {
            return IndexOf(data) != -1;
        }

        /// <summary>
        /// Search for the index of <paramref name="data"/> in the list.
        /// </summary>
        /// <param name="data">The data to search for</param>
        /// <returns>The index of the <paramref name="data"/>. If not found returns -1.</returns>
        public int IndexOf(T data)
        {
            for (int i = 0; i < Length; i++)
            {
                if (data.Equals(internalArray[i]))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Clears the list
        /// </summary>
        public void Clear()
        {
            internalArray = Array.Empty<T>();
        }

        /// <summary>
        /// Converts this list to an equal array representation. Beware: the items copied by reference!
        /// </summary>
        /// <returns>The array representation of this list</returns>
        public T[] ToArray()
        {
            T[] arr = new T[Length];

            int idx = 0;
            for (int i = 0; i < Length; i++)
            {
                arr[idx] = internalArray[i];
                idx++;
            }
            return arr;
        }

        /// <summary>
        /// Copy the <paramref name="list"/> to this list
        /// </summary>
        /// <param name="list"></param>
        private void CopyList(List<T> list)
        {
            internalArray = new T[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                internalArray[i] = list[i];
            }
        }

        /// <summary>
        /// Copies an array to this list
        /// </summary>
        /// <param name="array"></param>
        private void ArrayToList(T[] array)
        {
            internalArray = new T[array.Length];
            ArrayCopy(array, 0, internalArray, 0);
        }

        /// <summary>
        /// Remove the first item which equals to the <paramref name="data"/>. (Checked by Equals())
        /// </summary>
        /// <param name="data">The item to remove</param>
        private void RemoveByData(T data)
        {
            int rmIdx = IndexOf(data);
            if (rmIdx != -1)
            {
                RemoveByIndex(rmIdx);
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
                RemoveSpace(ref internalArray, index);
            }
            else
                throw new IndexOutOfRangeException();
        }

        /// <summary>
        /// Add the <paramref name="item"/> to the list
        /// </summary>
        /// <param name="item">The item to add</param>
        private void Append(T item)
        {
            AddSpace(ref internalArray, Length + 1);
            internalArray[Length - 1] = item;
        }

        //private void ResizeInternalArray(int length)
        //{
        //    T[] newArr = new T[length];
        //    CopyItems(newArr, internalArray, length, skipIdx);
        //    internalArray = newArr;
        //}

        //private void CopyItems(T[] from, T[] to, int length, int skipIdx = -1)
        //{
        //    int fromIdx = 0;

        //    int minLen = to.Length < from.Length ? to.Length : from.Length;

        //    minLen = length < minLen ? length : minLen;

        //    for (int i = 0; i < minLen; i++)
        //    {
        //        if (skipIdx == fromIdx)
        //            fromIdx++;

        //        to[i] = from[fromIdx];
        //        fromIdx++;
        //    }
        //}

        /// <summary>
        /// And an empty space to the <paramref name="array"/> pointed by <paramref name="at"/>. (-1 equals to the array length)
        /// </summary>
        /// <param name="array">The array to modify</param>
        /// <param name="at">An index</param>
        private void AddSpace(ref T[] array, int at = -1)
        {
            T[] newArr = new T[array.Length + 1];
            if (at == array.Length || at == -1) // end of the array
            {
                ArrayCopy(array, 0, newArr, 0);
            }
            else if (at == 0) //beginning of the array
            {
                ArrayCopy(array, 0, newArr, 1);
            }
            else
            {
                ArrayCopy(array, 0, newArr, 0, at);
                ArrayCopy(array, at, newArr, at + 1, array.Length - at);
            }
            array = newArr;
        }

        /// <summary>
        /// Remove an item form the <paramref name="array"/> pointed by <paramref name="at"/>
        /// </summary>
        /// <param name="array">The array to modify</param>
        /// <param name="at">An index</param>
        private void RemoveSpace(ref T[] array, int at)
        {
            T[] newArr = new T[array.Length - 1];
            if (at == array.Length - 1 || at == -1) // end of the array
            {
                ArrayCopy(array, 0, newArr, 0);
            }
            else if (at == 0) //beginning of the array
            {
                ArrayCopy(array, 1, newArr, 0);
            }
            else
            {
                ArrayCopy(array, 0, newArr, 0, at);
                ArrayCopy(array, at + 1, newArr, at, array.Length - at);
            }
            array = newArr;
        }

        /// <summary>
        /// Copy the array items from <paramref name="source"/> to the <paramref name="dest"/> array.
        /// </summary>
        /// <param name="source">The array to copy from</param>
        /// <param name="sourceIdx">First index to copy</param>
        /// <param name="dest">Array to copy to</param>
        /// <param name="destIdx">First index to copy to</param>
        /// <param name="length">Length of the sequence to copy (-1 equals to the smaller array length)</param>
        private void ArrayCopy(T[] source, int sourceIdx, T[] dest, int destIdx, int length = -1)
        {
            if (length == -1)
                length = source.Length < dest.Length ? source.Length : dest.Length;

            int count = 0;
            while (count < length && source.Length > sourceIdx && dest.Length > destIdx)
            {
                dest[destIdx++] = source[sourceIdx++];
                count++;
            }
        }
    }
}
