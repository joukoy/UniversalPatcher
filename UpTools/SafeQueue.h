#ifndef SAFEQUEUE
#define SAFEQUEUE
#include "windows.h"
#include <queue>
#include <mutex>
#include <condition_variable>

template<class T>
class SafeQueue{
public:
	SafeQueue(void);
	void enqueue(T t);
	T dequeue(void);
	BOOL empty();
	int size();
private:
	std::queue<T> q;
	mutable std::mutex m;
	std::condition_variable c;
};
#endif SAFEQUEUE
