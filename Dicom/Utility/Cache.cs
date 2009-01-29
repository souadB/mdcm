using System;
using System.Collections.Generic;
using System.Threading;

namespace Dicom.Utility {
	public class LeastRecentUsedCache<T> {
		#region Private Members
		private const int LockTimeout = 1000; // 1 second

		private int _maxItems;
		private List<string> _removalQueue;
		private Dictionary<string, T> _index;
		private object _lock;
		#endregion

		#region Public Constructors
		public LeastRecentUsedCache(int maxItems) {
			_maxItems = maxItems;
			_removalQueue = new List<string>(_maxItems);
			_index = new Dictionary<string, T>(_maxItems);
			_lock = new object();
		}
		#endregion

		#region Public Properties
		public int MaximumItems {
			get { return _maxItems; }
		}

		public T this[string key] {
			get {
				lock (_lock) {
					if (_index.ContainsKey(key)) {
						_removalQueue.Remove(key);
						_removalQueue.Add(key);
						return _index[key];
					}
					return default(T);
				}
			}
			set {
				Add(key, value);
			}
		}
		#endregion

		#region Public Members
		public void Add(string key, T value) {
			lock (_lock) {
				if (_index.ContainsKey(key)) {
					_removalQueue.Remove(key);
				}
				else {
					if (_removalQueue.Count == MaximumItems) {
						string removeKey = _removalQueue[0];
						Remove(removeKey);
					}
				}

				_index[key] = value;
				_removalQueue.Add(key);
			}
		}

		public void Remove(string key) {
			lock (_lock) {
				_index.Remove(key);
				_removalQueue.Remove(key);
			}
		}

		public bool ContainsKey(string key) {
			lock (_lock) {
				return _index.ContainsKey(key);
			}
		}

		public void Clear() {
			lock (_lock) {
				_index.Clear();
				_removalQueue.Clear();
			}
		}
		#endregion
	}
}
