using System;
using System.Collections.Generic;
using System.Linq;

namespace LLParser
{
    public static class CollectionExtensions
    {
        public static char PopFromBack(this List<char> collection)
        {
            var returnValue = collection[collection.Count - 1];

            collection.RemoveAt(collection.Count - 1);

            return returnValue;
        }
        public static string PopFromBack(this List<char> collection, int count)
        {
            count = Math.Min(collection.Count, count);

            if (count == 0) return "";

            var start = collection.Count - count;

            var returnValue = new string(collection.Skip(start).ToArray());

            collection.RemoveRange(start, count);

            return returnValue;
        }
        

        public static char PullFromStart(this List<char> collection)
        {
            var returnValue = collection[0];

            collection.RemoveAt(0);

            return returnValue;
        }
        public static string PullFromStart(this List<char> collection, int count)
        {
            count = Math.Min(collection.Count, count);

            if (count == 0) return "";

            var returnValue = new string(collection.Take(count).ToArray());

            collection.RemoveRange(0, count);

            return returnValue;
        }


        public static char PeekFromBack(this List<char> collection)
        {
            return collection[collection.Count - 1];
        }
        public static string PeekFromBack(this List<char> collection, int count)
        {
            return new string(collection.Skip(collection.Count - Math.Min(collection.Count, count)).ToArray());
        }


        public static char PeekFromStart(this List<char> collection)
        {
            return collection[0];
        }
        public static string PeekFromStart(this List<char> collection, int count)
        {
            return new string(collection.Take(Math.Min(collection.Count, count)).ToArray());
        }


        public static void PushToBack(this List<char> queue, IEnumerable<char> values)
        {
            queue.AddRange(values);
        }
        public static void PushToBack(this List<char> queue, char value)
        {
            queue.Add(value);
        }
        public static void PushToStart(this List<char> queue, IEnumerable<char> values)
        {
            queue.InsertRange(0, values);
        }
        public static void PushToStart(this List<char> queue, char value)
        {
            queue.Insert(0, value);
        }
    }
}
