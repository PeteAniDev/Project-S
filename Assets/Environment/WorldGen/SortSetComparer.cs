using System.Collections.Generic;
using System;
using System.Collections;

public abstract class SortSetComparer<T> : Comparer<T> where T : IComparable {

	public override int Compare(T x, T y) {
		int result = x.CompareTo(y);

		if (result == 0)
			return 1; // Handle equality as being greater. Note: this will break Remove(key) or
		else          // IndexOfKey(key) since the comparer never returns 0 to signal key equality
			return result;
	}

}