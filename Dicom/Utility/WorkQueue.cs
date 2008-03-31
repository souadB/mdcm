// mDCM: A C# DICOM library
//
// Copyright (c) 2006-2008  Colby Dillion
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Author:
//    Colby Dillion (colby.dillion@gmail.com)

using System;
using System.Collections.Generic;
using System.Threading;

namespace Dicom.Utility {
	public class WorkQueue<T> : IDisposable {
		public delegate void WorkItemProcessor(T workItem);

		#region Private Members
		private WorkItemProcessor _processor;

		private object _queueLock;
		private Queue<T> _queue;

		private bool _stop;
		private int _threadCount;
		private List<Thread> _threads;
		private ManualResetEvent _stopEvent;
		private Semaphore _semaphore;

		private int _processed;
		private int _active;
		#endregion

		#region Public Constructors
		public WorkQueue(WorkItemProcessor processor)
			: this(processor, 1) {
		}

		public WorkQueue(WorkItemProcessor processor, int threads) {
			_threadCount = Math.Max(1, Math.Min(Environment.ProcessorCount * 2, threads));

			_processor = processor;

			_queueLock = new object();
			_queue = new Queue<T>();

			Start();
		}
		#endregion

		#region Public Properties
		public int PendingWorkItems {
			get {
				lock (_queueLock) {
					return _queue.Count;
				}
			}
		}

		public int ProcessedWorkItems {
			get { return _processed; }
		}

		public int ActiveThreads {
			get { return _active; }
		}

		public int ThreadCount {
			get { return _threadCount; }
		}
		#endregion

		#region Public Methods
		public void Start() {
			if (_threads == null) {
				_stop = false;
				_stopEvent = new ManualResetEvent(false);
				_semaphore = new Semaphore(0, _threadCount);
				_threads = new List<Thread>(_threadCount);
				for (int i = 0; i < _threadCount; i++) {
					Thread thread = new Thread(WorkerProc);
					thread.IsBackground = true;
					thread.Start();
				}
			}
		}

		public void Stop(bool join) {
			if (_threads != null) {
				_stop = true;
				_stopEvent.Set();
				if (join) {
					while (_threads.Count > 0) {
						_threads[0].Join();
						_threads.RemoveAt(0);
					}
				}
				else {
					_threads.Clear();
				}
				_threads = null;
				_stopEvent.Close();
				_stopEvent = null;
				_semaphore.Close();
				_semaphore = null;
			}
		}

		public void QueueWorkItem(T workItem) {
			lock (_queueLock) {
				_queue.Enqueue(workItem);
			}
			_semaphore.Release();
		}
		#endregion

		#region Private Members
		private void WorkerProc() {
			WaitHandle[] waitHandles = new WaitHandle[] { _stopEvent, _semaphore };

			while (!_stop) {
				if (WaitHandle.WaitAny(waitHandles) == 0)
					return;

				Interlocked.Increment(ref _active);

				T workItem;
				lock (_queueLock) {
					workItem = _queue.Dequeue();
				}

				try {
					if (_processor != null)
						_processor(workItem);
				}
				catch {
				}

				Interlocked.Increment(ref _processed);
				Interlocked.Decrement(ref _active);
			}
		}
		#endregion

		#region IDisposable Members

		public void Dispose() {
			Stop(true);
			_queue.Clear();
			_queue = null;
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}