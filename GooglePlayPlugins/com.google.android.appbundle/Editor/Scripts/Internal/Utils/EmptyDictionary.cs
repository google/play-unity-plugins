// Copyright 2019 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Google.Android.AppBundle.Editor.Internal.Utils
{
    /// <summary>
    /// An empty <see cref="IDictionary"/> that cannot be added to.
    /// </summary>
    public class EmptyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly EmptyCollection<TKey> EmptyKeys = new EmptyCollection<TKey>();
        private readonly EmptyCollection<TValue> EmptyValues = new EmptyCollection<TValue>();

        public int Count
        {
            get { return 0; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ICollection<TKey> Keys
        {
            get { return EmptyKeys; }
        }

        public ICollection<TValue> Values
        {
            get { return EmptyValues; }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            return false;
        }

        public TValue this[TKey key]
        {
            get { throw new KeyNotFoundException(); }
            set { throw new NotSupportedException(); }
        }

        public void Add(TKey key, TValue value)
        {
            throw new NotSupportedException();
        }

        public bool Remove(TKey key)
        {
            throw new NotSupportedException();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            // No-op.
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            // No-op.
        }
    }
}