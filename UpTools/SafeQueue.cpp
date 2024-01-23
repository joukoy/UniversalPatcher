#include "pch.h"
#include "SafeQueue.h"

// A threadsafe-queue.
template <class T>
SafeQueue<T>::SafeQueue(void)
    : q()
    , m()
    , c()
{}

//~SafeQueue(void)
//{}

// Add an element to the queue.
template <class T>
void SafeQueue<T>::enqueue(T t)
{
    std::lock_guard<std::mutex> lock(m);
    q.push(t);
    c.notify_one();
}

// Get the "front"-element.
// If the queue is empty, wait till a element is avaiable.
template <class T>
T SafeQueue<T>::dequeue(void)
{
    std::unique_lock<std::mutex> lock(m);
    while (q.empty())
    {
        // release lock as long as the wait and reaquire it afterwards.
        c.wait(lock);
    }
    T val = q.front();
    q.pop();
    return val;
}
template <class T>
BOOL SafeQueue<T>::empty()
{
    return q.empty();
}
template <class T>
int SafeQueue<T>::size()
{
    return q.size();
}
