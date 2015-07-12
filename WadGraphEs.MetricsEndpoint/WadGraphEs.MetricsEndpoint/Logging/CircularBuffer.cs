using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzurePlot.Web.Logging {
	class CircularBuffer<T> where T:class{
		readonly T[] _buffer;
		readonly int _size;

		int _pointer;
		
		public CircularBuffer(int size) {
			_size = size;
			_buffer = new T[size];
			_pointer = 0;
		}
		internal void Add(T item) {
			lock(_buffer) {
				_buffer[_pointer] = item;
				_pointer = (_pointer+1)%_size;
			}
		}

		internal ICollection<T> ToCollection() {
			var fromPointer = _buffer.Skip(_pointer).ToList();
			var toPointer = _buffer.Take(_pointer).ToList();
			return fromPointer.Concat(toPointer).Where(_=>_!=null).ToList();
		}
	}
}
