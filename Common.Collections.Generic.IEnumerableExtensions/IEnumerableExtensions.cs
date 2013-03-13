namespace Common.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// System.Collections.Generic.IEnumerableの拡張クラス
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// 列挙子の中身をシャッフルする。
        /// PHPのshuffle(&Array)のパクリ。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random random)
        {
            List<T> copy = source.ToList();

            int count = 0;
            while (0 < (count = copy.Count))
            {
                int index = random.Next(count);
                yield return copy[index];
                copy.RemoveAt(index);
            }
        }

        /// <summary>
        /// 列挙子の中身をシャッフルする。
        /// PHPのshuffle(&Array)のパクリ。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.Shuffle<T>(new Random());
        }

        /// <summary>
        /// 列挙子の先頭からelementsに該当する要素を削除する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static IEnumerable<T> TrimLeft<T>(this IEnumerable<T> source, params T[] elements)
        {
            return source.SkipWhile(e => e.Equals(elements));
        }

        /// <summary>
        /// 列挙子の後方からelementsに該当する要素を削除する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static IEnumerable<T> TrimRight<T>(this IEnumerable<T> source, params T[] elements)
        {
            return source.Reverse() // 反転して
                .TrimLeft(elements) // 左トリムして
                .Reverse();         // もう一度反転して並びを戻す
        }

        /// <summary>
        /// 列挙子の両端からelementsに該当する要素を削除する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static IEnumerable<T> Trim<T>(this IEnumerable<T> source, params T[] elements)
        {
            return source.TrimLeft(elements).TrimRight();
        }

        /// <summary>
        /// シーケンスに含まれる唯一の要素を返す。
        /// シーケンスに含まれる要素が複数の場合には指定された条件を満たす唯一の要素を返す。
        /// たとえばidと名前のペアがある場合に、idから名前を引くなどという使い方をするもの
        /// だが、あまり使い道はない。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicateIfNotSingle"></param>
        /// <returns></returns>
        public static T SingleOrPredicatedSingle<T>(this IEnumerable<T> source, Func<T, Boolean> predicateIfNotSingle)
        {
            return source.Count() < 1 ? source.Single() : source.Single(predicateIfNotSingle);
        }

        /// <summary>
        /// KeyValuePairコレクションをIDictionaryに変換する。
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            return collection.ToDictionary(p => p.Key, p => p.Value);
        }
    }
}
