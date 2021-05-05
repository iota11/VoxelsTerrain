﻿using System;
using System.Collections.Generic;

namespace Generics.Pool
{
    public class ObjectPool<T> where T : class
    {
        private readonly Stack<T> m_available;
        private readonly HashSet<T> m_inUse;
        private readonly Func<T> m_generator;

        public ObjectPool(Func<T> generator, int initialCapacity, int initialCount = 0)
        {
            if (initialCapacity < initialCount)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), initialCapacity, "Initial capacity must be at least as large as initial preheat count!");
            }

            m_available = new Stack<T>(initialCapacity);
            m_inUse = new HashSet<T>();
            m_generator = generator;

            Populate(initialCount);
        }

        public T Acquire()
        {
            T obj;

            lock(m_available)
            {
                if (m_available.Count == 0)
                {
                    obj = m_generator();
                }
                else
                {
                    obj = m_available.Pop();
                }
            }
            m_inUse.Add(obj);

            return obj;
        }

        public void Release(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            lock(m_available)
            {
                if (!m_inUse.Remove(obj))
                {
                    throw new ArgumentException("Released object doesn't belong to this pool!", nameof(obj));
                }

                m_available.Push(obj);
            }
        }

        public void Populate(int count)
        {
            for (int index = 0; index < count; index++)
            {
                T obj = m_generator();
                m_available.Push(obj);
            }
        }

        public void Apply(Action<T, bool> action)
        {
            foreach (T obj in m_available)
            {
                action(obj, false);
            }

            foreach (T obj in m_inUse)
            {
                action(obj, true);
            }
        }
    }
}