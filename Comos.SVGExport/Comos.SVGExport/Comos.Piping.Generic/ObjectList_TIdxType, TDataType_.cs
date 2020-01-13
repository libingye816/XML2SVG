using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Comos.Piping.Generic
{
	internal class ObjectList<TIdxType, TDataType> : IEnumerable
	{
		protected const int Minsearchdiff = 4;

		protected TIdxType[] idxMem;

		protected TDataType[] dataMem;

		protected int _count;

		private int _capacity;

		private int _increaseStep;

		protected bool _sorted;

		private bool _autosort;

		protected int _sortcount;

		protected int _compares;

		protected OlDuplicates _duplicates;

		public bool Autosort
		{
			get
			{
				return this._autosort;
			}
			set
			{
				this._autosort = value;
			}
		}

		public int Capacity
		{
			get
			{
				return this._capacity;
			}
			set
			{
				this._capacity = value;
				this.FitArrays();
			}
		}

		public int Compares
		{
			get
			{
				return this._compares;
			}
		}

		public int Count
		{
			get
			{
				return this._count;
			}
		}

		public OlDuplicates Duplicates
		{
			get
			{
				return this._duplicates;
			}
			set
			{
				this._duplicates = value;
			}
		}

		public int IncreaseStep
		{
			get
			{
				return this._increaseStep;
			}
			set
			{
				this._increaseStep = value;
			}
		}

		public bool IsSorted
		{
			get
			{
				return this._sorted;
			}
			set
			{
				bool flag = false;
				if (this._sortcount < this._count)
				{
					flag = true;
				}
				if (!this._sorted)
				{
					flag = true;
				}
				if (flag)
				{
					this.Sort();
				}
			}
		}

		public KeyValuePair<TIdxType, TDataType> this[int idx]
		{
			get
			{
				this._compares = 0;
				if (idx < 0 || idx >= this._count)
				{
					return new KeyValuePair<TIdxType, TDataType>(default(TIdxType), default(TDataType));
				}
				return new KeyValuePair<TIdxType, TDataType>(this.idxMem[idx], this.dataMem[idx]);
			}
			set
			{
				if (idx >= 0 && idx < this._count)
				{
					this.idxMem[idx] = value.Key;
					this.dataMem[idx] = value.Value;
				}
			}
		}

		public TDataType this[TIdxType idxValue]
		{
			get
			{
				int num = this.IndexOf(idxValue);
				if (num < 0 || num >= this._count)
				{
					return default(TDataType);
				}
				return this.dataMem[num];
			}
			set
			{
				int num = this.IndexOf(idxValue);
				if (num >= 0 && num < this._count)
				{
					this.dataMem[num] = value;
				}
			}
		}

		private int SortCount
		{
			get
			{
				return this._sortcount;
			}
			set
			{
			}
		}

		public ObjectList() : this(16)
		{
		}

		public ObjectList(int newCapacity)
		{
			this._capacity = newCapacity;
			this.FitArrays();
		}

		public void Add(TIdxType newIdxValue)
		{
			if (!this.CanInsertItem(newIdxValue))
			{
				return;
			}
			if (this._count == this._capacity)
			{
				this._capacity += this._increaseStep;
				this.FitArrays();
			}
			if (this._count < this._capacity)
			{
				this.idxMem[this._count] = newIdxValue;
				this.dataMem[this._count] = default(TDataType);
				this._count++;
				this._sorted = false;
				if (this._autosort)
				{
					this.Sort();
				}
			}
		}

		public void AddObject(TIdxType newIdxValue, TDataType newDataValue)
		{
			if (!this.CanInsertItem(newIdxValue))
			{
				return;
			}
			if (this._count == this._capacity)
			{
				this._capacity += this._increaseStep;
				this.FitArrays();
			}
			if (this._count < this._capacity)
			{
				this.idxMem[this._count] = newIdxValue;
				this.dataMem[this._count] = newDataValue;
				this._count++;
				this._sorted = false;
				if (this._autosort)
				{
					this.Sort();
				}
			}
		}

		private bool BubbleSort()
		{
			this._sorted = false;
			this._sortcount = -1;
			try
			{
				for (int i = 0; i < this._count; i++)
				{
					for (int j = 0; j < this._count; j++)
					{
						if (this.GetCompareIndex(this.idxMem[i], this.idxMem[j]) < 0)
						{
							TIdxType tIdxType = this.idxMem[i];
							this.idxMem[i] = this.idxMem[j];
							this.idxMem[j] = tIdxType;
							TDataType tDataType = this.dataMem[i];
							this.dataMem[i] = this.dataMem[j];
							this.dataMem[j] = tDataType;
						}
					}
				}
				this._sorted = true;
				this._sortcount = this._count;
			}
			catch
			{
			}
			return this._sorted;
		}

		private bool CanInsertItem(TIdxType newIdxValue)
		{
			bool flag = true;
			if (this._duplicates == OlDuplicates.DupIgnore && this.IndexOf(newIdxValue) > -1)
			{
				flag = false;
			}
			return flag;
		}

		public bool Delete(TIdxType aIdx)
		{
			return this.Delete(this.IndexOf(aIdx));
		}

		public bool Delete(int aIdx)
		{
			bool flag = false;
			if (this._count > 0 && aIdx >= 0 && aIdx < this._count)
			{
				try
				{
					for (int i = aIdx + 1; i < this._count; i++)
					{
						this.idxMem[i - 1] = this.idxMem[i];
						this.dataMem[i - 1] = this.dataMem[i];
					}
					this.idxMem[this._count - 1] = default(TIdxType);
					this.dataMem[this._count - 1] = default(TDataType);
					this._count--;
					flag = true;
				}
				catch
				{
				}
			}
			return flag;
		}

		private void FitArrays()
		{
			if (this.idxMem == null || this.dataMem == null)
			{
				this.idxMem = new TIdxType[this._capacity];
				this.dataMem = new TDataType[this._capacity];
				return;
			}
			if ((int)this.idxMem.Length == this._capacity)
			{
				return;
			}
			if ((int)this.idxMem.Length >= this._capacity)
			{
				TIdxType[] tIdxTypeArray = new TIdxType[this._capacity];
				TDataType[] tDataTypeArray = new TDataType[this._capacity];
				for (int i = 0; i < this._capacity; i++)
				{
					tIdxTypeArray[i] = this.idxMem[i];
					tDataTypeArray[i] = this.dataMem[i];
				}
				this.idxMem = null;
				this.dataMem = null;
				this.idxMem = tIdxTypeArray;
				this.dataMem = tDataTypeArray;
			}
			else
			{
				if (this.idxMem.Length == 0)
				{
					this.idxMem = new TIdxType[this._capacity];
					this.dataMem = new TDataType[this._capacity];
					return;
				}
				if (this.idxMem.Length != 0)
				{
					TIdxType[] tIdxTypeArray1 = new TIdxType[this._capacity];
					TDataType[] tDataTypeArray1 = new TDataType[this._capacity];
					this.idxMem.CopyTo(tIdxTypeArray1, 0);
					this.dataMem.CopyTo(tDataTypeArray1, 0);
					this.idxMem = null;
					this.dataMem = null;
					this.idxMem = tIdxTypeArray1;
					this.dataMem = tDataTypeArray1;
					return;
				}
			}
		}

		private int GetCompareIndex(TIdxType idx1, TIdxType idx2)
		{
			return Comparer<TIdxType>.Default.Compare(idx1, idx2);
		}

		public IEnumerator GetEnumerator()
		{
			return new ObjectList<TIdxType, TDataType>.ObjectListEnumerator(this);
		}

		private int IndexOf(TIdxType idx)
		{
			int num;
			int num1 = -1;
			bool flag = false;
			int num2 = 0;
			if (this._sorted && this._sortcount > 1)
			{
				int num3 = 0;
				int num4 = this._sortcount - 1;
				do
				{
				Label0:
					if (flag)
					{
						break;
					}
					int num5 = (num3 + num4 + 1) / 2;
					int num6 = Comparer<TIdxType>.Default.Compare(this.idxMem[num5], idx);
					num2++;
					if (num6 == 0)
					{
						num1 = num5;
						flag = true;
						goto Label0;
					}
					else if (num6 <= 0)
					{
						num = num4 - num5 + 1;
						num3 = num4 - num;
					}
					else
					{
						num = num5 - num3 + 1;
						num4 = num3 + num;
					}
				}
				while (num >= 4);
				if (!flag)
				{
					int num7 = num3;
					while (num7 <= num4)
					{
						num2++;
						if (Comparer<TIdxType>.Default.Compare(this.idxMem[num7], idx) != 0)
						{
							num7++;
						}
						else
						{
							flag = true;
							num1 = num7;
							break;
						}
					}
				}
			}
			if (!flag && this._sortcount < this._count)
			{
				int num8 = this._sortcount;
				while (num8 < this._count)
				{
					num2++;
					if (Comparer<TIdxType>.Default.Compare(this.idxMem[num8], idx) != 0)
					{
						num8++;
					}
					else
					{
						num1 = num8;
						break;
					}
				}
			}
			this._compares = num2;
			return num1;
		}

		public TIdxType[] Keys()
		{
			List<TIdxType> tIdxTypes = new List<TIdxType>();
			tIdxTypes.AddRange(this.idxMem);
			tIdxTypes.Capacity = this._count;
			return tIdxTypes.ToArray();
		}

		public TDataType[] Objects()
		{
			List<TDataType> tDataTypes = new List<TDataType>();
			tDataTypes.AddRange(this.dataMem);
			tDataTypes.Capacity = this._count;
			return tDataTypes.ToArray();
		}

		private bool QuickSort(int left, int right)
		{
			this._sorted = false;
			try
			{
				int num = left;
				int num1 = right;
				TIdxType tIdxType = this.idxMem[left];
				TDataType tDataType = this.dataMem[left];
				while (left < right)
				{
					while (this.GetCompareIndex(this.idxMem[right], tIdxType) >= 0 && left < right)
					{
						right--;
					}
					if (left != right)
					{
						this.idxMem[left] = this.idxMem[right];
						this.dataMem[left] = this.dataMem[right];
						left++;
					}
					while (this.GetCompareIndex(this.idxMem[left], tIdxType) <= 0 && left < right)
					{
						left++;
					}
					if (left == right)
					{
						continue;
					}
					this.idxMem[right] = this.idxMem[left];
					this.dataMem[right] = this.dataMem[left];
					right--;
				}
				this.idxMem[left] = tIdxType;
				this.dataMem[left] = tDataType;
				int num2 = left;
				left = num;
				right = num1;
				if (left < num2)
				{
					this.QuickSort(left, num2 - 1);
				}
				if (right > num2)
				{
					this.QuickSort(num2 + 1, right);
				}
				this._sorted = true;
				this._sortcount = this._count;
			}
			catch
			{
			}
			return this._sorted;
		}

		public void setObject(int idx, ref TDataType newData)
		{
			if (idx >= 0 && idx < this._count)
			{
				this.dataMem[idx] = newData;
			}
		}

		public void setObject(TIdxType setIdx, ref TDataType newData)
		{
			int num = this.IndexOf(setIdx);
			if (num >= 0 && num < this._count)
			{
				this.setObject(num, ref newData);
			}
		}

		public bool Sort()
		{
			return this.QuickSort(0, this._count - 1);
		}

		private class ObjectListEnumerator : IEnumerator
		{
			private int _pos;

			private ObjectList<TIdxType, TDataType> ol;

			public object Current
			{
				get
				{
					return this.ol[this._pos];
				}
			}

			public ObjectListEnumerator(ObjectList<TIdxType, TDataType> ol)
			{
				this.ol = ol;
			}

			public bool MoveNext()
			{
				if (this._pos >= this.ol.Count - 1)
				{
					return false;
				}
				this._pos++;
				return true;
			}

			public void Reset()
			{
				this._pos = -1;
			}
		}
	}
}