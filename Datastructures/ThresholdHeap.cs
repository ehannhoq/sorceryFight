using System;
using System.Collections;
using System.Collections.Generic;

namespace sorceryFight
{
    public class ThresholdHeap<TElement, TPriority> where TPriority : IComparable<TPriority>
    {
        private TElement[] heap;
        private TPriority[] priorities;
        private TPriority threshold;
        private int count;

        public ThresholdHeap(TPriority threshold)
        {
            heap = new TElement[16];
            priorities = new TPriority[16];
            this.threshold = threshold;
        }

        public void Insert(TElement element, TPriority priority)
        {
            if (count == heap.Length)
            {
                Resize();
            }

            heap[count] = element;
            priorities[count] = priority;
            SiftUp(count);
            count++;
        }

        public TElement Pop()
        {
            if (count == 0)
            {
                throw new InvalidOperationException("Heap is empty");
            }

            TElement rootElement = heap[0];

            count--;
            heap[0] = heap[count];
            priorities[0] = priorities[count];
            heap[count] = default;
            priorities[count] = default;

            SiftDown(0);

            return rootElement;
        }

        public bool Peek(out TElement element)
        {
            if (count == 0)
            {
                element = default;
                return false;
            }

            element = heap[0];
            return true;
        }

        private void SiftUp(int i)
        {
            while (i > 0)
            {
                int parent = GetParentIndex(i);
                TPriority parentPlusThreshold = AddThreshold(priorities[parent]);

                if (priorities[i].CompareTo(parentPlusThreshold) > 0)
                {
                    Swap(i, parent);
                    i = parent;
                }
                else
                {
                    break;
                }
            }
        }

        private void SiftDown(int i)
        {
            while (true)
            {
                int left = GetLeftChildIndex(i);
                int right = GetRightChildIndex(i);
                int largest = i;

                if (left < count && priorities[left].CompareTo(priorities[largest]) > 0)
                {
                    largest = left;
                }

                if (right < count && priorities[right].CompareTo(priorities[largest]) > 0)
                {
                    largest = right;
                }

                if (largest == i)
                {
                    break;
                }

                Swap(i, largest);
                i = largest;
            }
        }

        private TPriority AddThreshold(TPriority p)
        {
            return (TPriority)((dynamic)p + (dynamic)threshold);
        }

        private void Swap(int i, int j)
        {
            TElement tempE = heap[i];
            heap[i] = heap[j];
            heap[j] = tempE;

            TPriority tempP = priorities[i];
            priorities[i] = priorities[j];
            priorities[j] = tempP;
        }

        private void Resize()
        {
            int newSize = heap.Length * 2;
            Array.Resize(ref heap, newSize);
            Array.Resize(ref priorities, newSize);
        }

        private int GetParentIndex(int i)
        {
            return (i - 1) / 2;
        }
        private int GetLeftChildIndex(int i)
        {
            return 2 * i + 1; 
        }

        private int GetRightChildIndex(int i)
        {
            return 2 * i + 2;
        }
    }

}