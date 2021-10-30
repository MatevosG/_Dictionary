using System;
using System.Collections;
using System.Collections.Generic;



namespace _Dictionary
{
    class Program
    {
        static void Main(string[] args)
        {
            _Dictionary<int, string> dictionary = new _Dictionary<int, string>(5);
            dictionary.Add(1,"kl");
            dictionary.Add(2,"lkkl");
            dictionary.Add(3,"kkl");
           
        }
        public class _Dictionary <TKey, TValue>
        {
            private struct Entry
            {
                public int hashCode;
                public int next;
                public TKey key;
                public TValue value;
            }
            private int[] buckets;
            private Entry[] entries;
            private int count;
            private int version;
            private int freeList;
            private int freeCount;
            private IEqualityComparer<TKey> comparer;
            
            internal const Int32 HashPrime = 101;
            public _Dictionary() 
            {
            
            }

            public _Dictionary(int capacity)
            {

            }
            
            public void Add(TKey key, TValue value)
            {
                Insert(key, value, true);
            }

            public TValue this[TKey key]
            {
                get
                {
                    int i = FindEntry(key);
                    if (i >= 0) return entries[i].value;
                    return default(TValue);
                }
                set
                {
                    Insert(key, value, false);
                }
            }
            private int FindEntry(TKey key)
            {


                if (buckets != null)
                {
                    int hashCode = comparer.GetHashCode(key) & 0x7FFFFFFF;
                    for (int i = buckets[hashCode % buckets.Length]; i >= 0; i = entries[i].next)
                    {
                        if (entries[i].hashCode == hashCode && comparer.Equals(entries[i].key, key)) return i;
                    }
                }
                return -1;
            }
            private void Insert(TKey key, TValue value, bool add)
            {


                if (buckets == null) Initialize(0);
                int hashCode = EqualityComparer<TKey>.Default.GetHashCode() & 0x7FFFFFFF;
                int targetBucket = hashCode % buckets.Length;


                for (int i = buckets[targetBucket]; i >= 0; i = entries[i].next)
                {
                    if (entries[i].hashCode == hashCode && EqualityComparer<TKey>.Default.Equals(entries[i].key, key))
                    {
                        entries[i].value = value;
                        version++;
                        return;
                    }
                }
                int index;
                if (freeCount > 0)
                {
                    index = freeList;
                    freeList = entries[index].next;
                    freeCount--;
                }
                else
                {
                    if (count == entries.Length)
                    {
                        targetBucket = hashCode % buckets.Length;
                    }
                    index = count;
                    count++;
                }

                entries[index].hashCode = hashCode;
                entries[index].next = buckets[targetBucket];
                entries[index].key = key;
                entries[index].value = value;
                buckets[targetBucket] = index;
                version++;
            }
            private void Initialize(int capacity)
            {
                int size = GetPrime(capacity);
                buckets = new int[size];
                for (int i = 0; i < buckets.Length; i++) buckets[i] = -1;
                entries = new Entry[size];
                freeList = -1;
            }
            public static readonly int[] primes = {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
            1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369};
            
            public static int GetPrime(int min)
            {
                for (int i = 0; i < primes.Length; i++)
                {
                    int prime = primes[i];
                    if (prime >= min) return prime;
                }
                for (int i = (min | 1); i < Int32.MaxValue; i += 2)
                {
                    if (IsPrime(i) && ((i - 1) % HashPrime != 0))
                        return i;
                }
                return min;
            }
            public static bool IsPrime(int candidate)
            {
                if ((candidate & 1) != 0)
                {
                    int limit = (int)Math.Sqrt(candidate);
                    for (int divisor = 3; divisor <= limit; divisor += 2)
                    {
                        if ((candidate % divisor) == 0)
                            return false;
                    }
                    return true;
                }
                return (candidate == 2);
            }
        }
    }
}

