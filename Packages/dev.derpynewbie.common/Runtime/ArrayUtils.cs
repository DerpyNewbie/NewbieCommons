using System;
using JetBrains.Annotations;

namespace DerpyNewbie.Common
{
    public static class ArrayUtils
    {
        [PublicAPI] [Pure]
        public static T[] AddAsList<T>(this T[] arr, T item)
        {
            var newArr = new T[arr.Length + 1];
            Array.Copy(arr, newArr, arr.Length);
            newArr[arr.Length] = item;
            return newArr;
        }

        [PublicAPI] [Pure]
        public static T[] AddAsSet<T>(this T[] arr, T item)
        {
            return arr.ContainsItem(item) ? arr : arr.AddAsList(item);
        }

        [PublicAPI] [Pure]
        public static T[] AddAsSet<T>(this T[] arr, T item, out bool result)
        {
            result = !arr.ContainsItem(item);
            return !result ? arr : arr.AddAsList(item);
        }

        [PublicAPI] [Pure]
        public static T[] InsertItemAtIndex<T>(this T[] arr, int index, T item)
        {
            bool _;
            return arr.InsertItemAtIndex(index, item, out _);
        }

        [PublicAPI] [Pure]
        public static T[] InsertItemAtIndex<T>(this T[] arr, int index, T item, out bool result)
        {
            if (index < 0 || index > arr.Length)
            {
                result = false;
                return arr;
            }

            var newArr = new T[arr.Length + 1];
            if (arr.Length != 0)
            {
                Array.Copy(arr, newArr, index);
                if (arr.Length - index != 0)
                    Array.ConstrainedCopy(arr, index, newArr, index + 1, arr.Length - index);
            }

            newArr[index] = item;

            result = true;
            return newArr;
        }

        [PublicAPI] [Pure]
        public static T[] RemoveItem<T>(this T[] arr, T item)
        {
            bool _;
            return arr.RemoveItem(item, out _);
        }

        [PublicAPI] [Pure]
        public static T[] RemoveItem<T>(this T[] arr, T item, out bool result)
        {
            var itemIndex = arr.FindItem(item);
            return arr.RemoveItemAtIndex(itemIndex, out result);
        }

        [PublicAPI] [Pure]
        public static T[] RemoveItemAtIndex<T>(this T[] arr, int index)
        {
            bool _;
            return arr.RemoveItemAtIndex(index, out _);
        }

        [PublicAPI] [Pure]
        public static T[] RemoveItemAtIndex<T>(this T[] arr, int index, out bool result)
        {
            if (index < 0 || index >= arr.Length)
            {
                result = false;
                return arr;
            }

            var isLastItem = index == arr.Length - 1;
            var newArr = new T[arr.Length - 1];

            Array.Copy(arr, newArr, newArr.Length);
            if (!isLastItem)
                Array.ConstrainedCopy(arr, index + 1, newArr, index, arr.Length - (index + 1));

            result = true;
            return newArr;
        }

        [PublicAPI] [Pure]
        public static bool ContainsItem<T>(this T[] arr, T item)
        {
            return arr.FindItem(item) != -1;
        }

        [PublicAPI] [Pure]
        public static bool ContainsString(this string[] arr, string item,
            StringComparison comparison = StringComparison.Ordinal)
        {
            return arr.FindString(item, comparison) != -1;
        }

        [PublicAPI] [Pure]
        public static int FindString(this string[] arr, string item,
            StringComparison comparison = StringComparison.Ordinal)
        {
            if (item == null)
            {
                for (var i = 0; i < arr.Length; i++)
                    if (arr[i] == null)
                        return i;
                return -1;
            }

            for (var i = 0; i < arr.Length; i++)
                if (arr[i] != null && arr[i].Equals(item, comparison))
                    return i;
            return -1;
        }

        [PublicAPI] [Pure]
        public static int FindItem<T>(this T[] arr, T item)
        {
            if (item == null)
            {
                for (var i = 0; i < arr.Length; i++)
                    if (arr[i] == null)
                        return i;
                return -1;
            }

            for (var i = 0; i < arr.Length; i++)
                if (arr[i] != null && arr[i].Equals(item))
                    return i;
            return -1;
        }

        [PublicAPI] [Pure]
        public static T[] AppendArray<T>(this T[] lhs, T[] rhs)
        {
            var r = new T[lhs.Length + rhs.Length];
            Array.ConstrainedCopy(lhs, 0, r, 0, lhs.Length);
            Array.ConstrainedCopy(rhs, 0, r, lhs.Length, rhs.Length);
            return r;
        }

        [PublicAPI] [Pure]
        public static T[] GetSpanOfArray<T>(this T[] arr, int index, int length)
        {
            if (arr == null || arr.Length < index + length) return null;

            var r = new T[length];
            Array.ConstrainedCopy(arr, index, r, 0, length);
            return r;
        }

        [PublicAPI] [Pure]
        public static T[] SwapItem<T>(this T[] arr, int src, int dst)
        {
            (arr[dst], arr[src]) = (arr[src], arr[dst]);
            return arr;
        }
    }
}