using System;
using System.Collections.Generic;

namespace sorceryFight;

public static class SFUtils
{
    /// <summary>
    /// THANK YOU CALAMITY MOD SOURCE CODE FOR THIS !!
    /// 
    /// Adds to a list on a given condition.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="type"></param>
    /// <param name="condition"></param>
    public static void AddWithCondition<T>(this List<T> list, T type, bool condition)
    {
        if (condition)
            list.Add(type);
    }
}
