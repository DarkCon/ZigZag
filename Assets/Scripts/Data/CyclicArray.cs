using System;
using System.Collections;
using System.Collections.Generic;

namespace Data {
    public class CyclicArray<T> : IReadOnlyList<T> {
        private T[] _a;
        private int _start;
        private int _end;
        private int _count;
        private int _capacity;

        public CyclicArray(int capacity = 10) {
            _capacity = capacity;
            _a = new T[_capacity];
            Clear();
        }

        public void Clear() {
            _start = 0;
            _end = 0;
            _count = 0;
        }

        public void Enqueue(T elem) {
            if (_capacity - _count <= 1) {
                Capacity += Math.Max(_capacity, 10);
            } 
            _a[_end] = elem;
            ++_end;
            _end %= _capacity;
            ++_count;
        }

        public T Dequeue() {
            var start = _start;
            --_count;
            ++_start;
            _start %= _capacity;
            return _a[start];
        }

        public T Peek() {
            return _a[_start];
        }

        public int Capacity {
            get { return _capacity; }
            set {
                if (_capacity != value) {
                    var newArray = new T[value];
                    _count = Math.Min(_count, value);
                    for (int i = 0; i < _count; ++i) {
                        newArray[i] = _a[i];
                    }
                    _a = newArray;
                    _start = 0;
                    _end = _count;
                    _capacity = value;
                }
            }
        }

        public int Count => _count;

        public T this[int index] => _a[(_start + index) % _capacity];

        public T GetFromLast(int index) {
            return _a[(_capacity + _end - index - 1) % _capacity];
        }

        public IEnumerator<T> GetEnumerator() {
            for (int i = 0; i < _count; ++i) {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}