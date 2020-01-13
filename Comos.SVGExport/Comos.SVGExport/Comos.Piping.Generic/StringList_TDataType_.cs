using System;
using System.Collections.Generic;

namespace Comos.Piping.Generic
{
	internal class StringList<TDataType> : ObjectList<string, TDataType>
	{
		public StringList()
		{
		}

		public StringList(int capacity) : base(capacity)
		{
		}

		public TDataType[] StartsWith(string aStrPrefix)
		{
			return this.StartsWith(aStrPrefix, false);
		}

		public TDataType[] StartsWith(string aStrPrefix, bool caseSensitive)
		{
			int num;
			int num1;
			int num2;
			int num3;
			int num4;
			int num5;
			int num6 = -1;
			bool flag = false;
			int num7 = 0;
			List<TDataType> tDataTypes = new List<TDataType>();
			if (!caseSensitive)
			{
				aStrPrefix = aStrPrefix.ToLower();
			}
			if (this._sorted && this._sortcount > 0)
			{
				int num8 = 0;
				int num9 = this._sortcount - 1;
				do
				{
				Label0:
					if (flag)
					{
						break;
					}
					int num10 = (num8 + num9 + 1) / 2;
					num = (!caseSensitive ? string.Compare(this.idxMem[num10].ToLower(), 0, aStrPrefix, 0, aStrPrefix.Length) : string.Compare(this.idxMem[num10], 0, aStrPrefix, 0, aStrPrefix.Length));
					num7++;
					if (num == 0)
					{
						num6 = num10;
						flag = true;
						goto Label0;
					}
					else if (num <= 0)
					{
						num1 = num9 - num10 + 1;
						num8 = num9 - num1;
					}
					else
					{
						num1 = num10 - num8 + 1;
						num9 = num8 + num1;
					}
				}
				while (num1 >= 4);
				if (!flag)
				{
					int num11 = num8;
					while (num11 <= num9)
					{
						num2 = (!caseSensitive ? string.Compare(this.idxMem[num11].ToLower(), 0, aStrPrefix, 0, aStrPrefix.Length) : string.Compare(this.idxMem[num11], 0, aStrPrefix, 0, aStrPrefix.Length));
						num7++;
						if (num2 != 0)
						{
							num11++;
						}
						else
						{
							flag = true;
							num6 = num11;
							break;
						}
					}
				}
			}
			if (flag)
			{
				for (int i = num6; i >= 0; i--)
				{
					num3 = (!caseSensitive ? string.Compare(this.idxMem[i].ToLower(), 0, aStrPrefix, 0, aStrPrefix.Length) : string.Compare(this.idxMem[i], 0, aStrPrefix, 0, aStrPrefix.Length));
					if (num3 == 0)
					{
						tDataTypes.Add(this.dataMem[i]);
					}
				}
				for (int j = num6 + 1; j < this._count; j++)
				{
					num4 = (!caseSensitive ? string.Compare(this.idxMem[j].ToLower(), 0, aStrPrefix, 0, aStrPrefix.Length) : string.Compare(this.idxMem[j], 0, aStrPrefix, 0, aStrPrefix.Length));
					if (num4 == 0)
					{
						tDataTypes.Add(this.dataMem[j]);
					}
				}
			}
			if (!flag && this._sortcount < this._count)
			{
				for (int k = this._sortcount; k < this._count; k++)
				{
					num5 = (!caseSensitive ? string.Compare(this.idxMem[k].ToLower(), 0, aStrPrefix, 0, aStrPrefix.Length) : string.Compare(this.idxMem[k], 0, aStrPrefix, 0, aStrPrefix.Length));
					if (num5 == 0)
					{
						tDataTypes.Add(this.dataMem[k]);
					}
					num7++;
				}
			}
			this._compares = num7;
			return tDataTypes.ToArray();
		}
	}
}