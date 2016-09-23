#region � Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Solution: Thargy.SqlTester 
// Project: Thargy.SqlTester
// File: ObjectRecord.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the �Software�) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the �Agreement�).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// � Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using JetBrains.Annotations;

namespace Thargy.SqlTester
{
    /// <summary>
    /// A record that can be created in code that implements <see cref="IDataRecord"/>.
    /// </summary>
    /// <remarks></remarks>
    public sealed class ObjectRecord : IObjectRecord
    {
        /// <summary>
        /// The column values.
        /// </summary>
        [NotNull] private readonly object[] _columnValues;

        /// <summary>
        /// The current table definition.
        /// </summary>
        [NotNull] private readonly RecordSetDefinition _recordSetDefinition;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectRecord" /> class.
        /// </summary>
        /// <param name="recordSetDefinition">The table definition.</param>
        /// <param name="randomData">if set to <see langword="true" /> fills columns with random data; otherwise fills them with their default values.</param>
        /// <param name="nullProbability">The probability of a column's value being set to SQL null (0.0 for no nulls) -
        /// this is only applicable is <see paramref="randomData" /> is set to <see langword="true" /> [Defaults to 0.1 = 10%].</param>
        /// <param name="columnGenerators">The column generators is an array of functions that generate a value for each column, if the function is
        /// <see langword="null" /> for a particular index then a random value is generated, if it is not null then the function is used.  The function takes
        /// the current row number as it's only parameter and must return an object of the correct type for the column.</param>
        /// <param name="rowNumber">The optional row number to pass to the generator.</param>
        /// <exception cref="System.ArgumentException">Thrown if the number of column generators exceeds the number of columns in the record set definition.</exception>
        /// <remarks></remarks>
        public ObjectRecord([NotNull] RecordSetDefinition recordSetDefinition, bool randomData = false,
                            double nullProbability = 0.1,
                            Func<int, object>[] columnGenerators = null, int rowNumber = 1)
        {
            _recordSetDefinition = recordSetDefinition;
            int columnCount = recordSetDefinition.FieldCount;
            _columnValues = new object[columnCount];

            if ((columnGenerators != null) &&
                (columnGenerators.Length > recordSetDefinition.FieldCount))
                throw new ArgumentException(
                    "The number of column generators must not exceed the number of columns in the record set definition.",
                    "columnGenerators");

            for (int c = 0; c < columnCount; c++)
            {
                // Check if we have a generator
                if ((columnGenerators != null) &&
                    (columnGenerators.Length > c) &&
                    (columnGenerators[c] != null))
                {
                    // Use generator to get value
                    this[c] = columnGenerators[c](rowNumber);
                }
                else if (randomData)
                {
                    // Generate random value.
                    this[c] = recordSetDefinition[c].GetRandomValue(nullProbability);
                }
                else
                {
                    // Just set to default value (no need to revalidate so set directly).
                    _columnValues[c] = recordSetDefinition[c].DefaultValue;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectRecord" /> class.
        /// </summary>
        /// <param name="recordSetDefinition">The table definition.</param>
        /// <param name="columnValues">The column values.</param>
        /// <remarks>
        /// If the number of column values supplied is less than the number of columns then the remaining columns are set to
        /// their equivalent default value.
        /// </remarks>
        public ObjectRecord([NotNull] RecordSetDefinition recordSetDefinition, [NotNull] params object[] columnValues)
        {
            int length = columnValues.Length;
            int columns = recordSetDefinition.FieldCount;
            if (length > columns)
                throw new ArgumentException(
                    string.Format(
                        "The number of values specified '{0}' cannot exceed the number of expected columns '{1}'.",
                        length, columns), "columnValues");

            _recordSetDefinition = recordSetDefinition;
            _columnValues = new object[recordSetDefinition.FieldCount];

            // Import values or set to null.
            for (int i = 0; i < columns; i++)
                SetValue(i, i < length ? columnValues[i] : _recordSetDefinition[i].DefaultValue);
        }

        /// <summary>
        /// Gets the column values.
        /// </summary>
        /// <value>The column data.</value>
        /// <remarks></remarks>
        [NotNull]
        public IEnumerable<object> ColumnValues
        {
            get { return _columnValues; }
        }

        #region IObjectRecord Members
        /// <inhertidoc />
        public string GetName(int i)
        {
            if ((i < 0) ||
                (i > FieldCount))
                throw new IndexOutOfRangeException(i.ToString(CultureInfo.InvariantCulture));
            return _recordSetDefinition[i].Name;
        }

        /// <inhertidoc />
        public string GetDataTypeName(int i)
        {
            if ((i < 0) ||
                (i > FieldCount))
                throw new IndexOutOfRangeException(i.ToString(CultureInfo.InvariantCulture));
            return _recordSetDefinition[i].TypeName;
        }

        /// <inhertidoc />
        public Type GetFieldType(int i)
        {
            if ((i < 0) ||
                (i > FieldCount))
                throw new IndexOutOfRangeException(i.ToString(CultureInfo.InvariantCulture));
            return _recordSetDefinition[i].ClassType;
        }

        /// <inhertidoc />
        public object GetValue(int i)
        {
            if ((i < 0) ||
                (i > FieldCount))
                throw new IndexOutOfRangeException(i.ToString(CultureInfo.InvariantCulture));
            return _columnValues[i];
        }

        /// <inhertidoc />
        public int GetValues(object[] values)
        {
            if (values == null)
                throw new NullReferenceException();
            int length = values.Length < _columnValues.Length ? values.Length : _columnValues.Length;
            Array.Copy(_columnValues, values, length);
            return length;
        }

        /// <inhertidoc />
        public int GetOrdinal(string name)
        {
            return _recordSetDefinition.GetOrdinal(name);
        }

        /// <inhertidoc />
        public bool GetBoolean(int i)
        {
            if ((i < 0) ||
                (i > FieldCount))
                throw new IndexOutOfRangeException(i.ToString(CultureInfo.InvariantCulture));
            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is bool))
                throw new InvalidCastException();
            return (bool) o;
        }

        /// <inhertidoc />
        public byte GetByte(int i)
        {
            if ((i < 0) ||
                (i > FieldCount))
                throw new IndexOutOfRangeException(i.ToString(CultureInfo.InvariantCulture));
            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is byte))
                throw new InvalidCastException();
            return (byte) o;
        }

        /// <inhertidoc />
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            if ((i < 0) ||
                (i > FieldCount))
                throw new IndexOutOfRangeException(i.ToString(CultureInfo.InvariantCulture));
            if (buffer == null)
                return 0;

            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is byte[]))
                throw new InvalidCastException();
            byte[] bytes = (byte[]) o;
            length = (bytes.Length - fieldOffset) < length ? (int) (bytes.Length - fieldOffset) : length;
            Array.Copy(bytes, fieldOffset, buffer, bufferoffset, length);
            return length;
        }

        /// <inhertidoc />
        public char GetChar(int i)
        {
            if ((i < 0) ||
                (i > FieldCount))
                throw new IndexOutOfRangeException(i.ToString(CultureInfo.InvariantCulture));
            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is char))
                throw new InvalidCastException();
            return (char) o;
        }

        /// <inhertidoc />
        public long GetChars(int i, long fieldOffset, char[] buffer, int bufferoffset, int length)
        {
            if ((i < 0) ||
                (i > FieldCount))
                throw new IndexOutOfRangeException(i.ToString(CultureInfo.InvariantCulture));
            if (buffer == null)
                return 0;

            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is char[]))
                throw new InvalidCastException();
            char[] chars = (char[]) o;
            length = (chars.Length - fieldOffset) < length ? (int) (chars.Length - fieldOffset) : length;
            Array.Copy(chars, fieldOffset, buffer, bufferoffset, length);
            return length;
        }

        /// <inhertidoc />
        public Guid GetGuid(int i)
        {
            if ((i < 0) ||
                (i > FieldCount))
                throw new IndexOutOfRangeException(i.ToString(CultureInfo.InvariantCulture));
            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is Guid))
                throw new InvalidCastException();
            return (Guid) o;
        }

        /// <inhertidoc />
        public short GetInt16(int i)
        {
            if ((i < 0) ||
                (i > FieldCount))
                throw new IndexOutOfRangeException(i.ToString(CultureInfo.InvariantCulture));
            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is short))
                throw new InvalidCastException();
            return (short) o;
        }

        /// <inhertidoc />
        public int GetInt32(int i)
        {
            if ((i < 0) ||
                (i > FieldCount))
                throw new IndexOutOfRangeException(i.ToString(CultureInfo.InvariantCulture));
            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is int))
                throw new InvalidCastException();
            return (int) o;
        }

        /// <inhertidoc />
        public long GetInt64(int i)
        {
            if ((i < 0) ||
                (i > FieldCount))
                throw new IndexOutOfRangeException(i.ToString(CultureInfo.InvariantCulture));
            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is long))
                throw new InvalidCastException();
            return (long) o;
        }

        /// <inhertidoc />
        public float GetFloat(int i)
        {
            if ((i < 0) ||
                (i > FieldCount))
                throw new IndexOutOfRangeException(i.ToString(CultureInfo.InvariantCulture));
            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is float))
                throw new InvalidCastException();
            return (float) o;
        }

        /// <inhertidoc />
        public double GetDouble(int i)
        {
            if ((i < 0) ||
                (i > FieldCount))
                throw new IndexOutOfRangeException(i.ToString(CultureInfo.InvariantCulture));
            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is double))
                throw new InvalidCastException();
            return (double) o;
        }

        /// <inhertidoc />
        public string GetString(int i)
        {
            if ((i < 0) ||
                (i > FieldCount))
                throw new IndexOutOfRangeException(i.ToString(CultureInfo.InvariantCulture));
            object o = _columnValues[i];
            if (!(o is string))
                throw new InvalidCastException();
            return (string) o;
        }

        /// <inhertidoc />
        public decimal GetDecimal(int i)
        {
            if ((i < 0) ||
                (i > FieldCount))
                throw new IndexOutOfRangeException(i.ToString(CultureInfo.InvariantCulture));
            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is decimal))
                throw new InvalidCastException();
            return (decimal) o;
        }

        /// <inhertidoc />
        public DateTime GetDateTime(int i)
        {
            if ((i < 0) ||
                (i > FieldCount))
                throw new IndexOutOfRangeException(i.ToString(CultureInfo.InvariantCulture));
            object o = _columnValues[i];
            if (o == null)
                throw new SqlNullValueException();
            if (!(o is DateTime))
                throw new InvalidCastException();
            return (DateTime) o;
        }

        /// <inhertidoc />
        IDataReader IDataRecord.GetData(int i)
        {
            // This isn't supported by SqlDataReader.
            throw new NotSupportedException();
        }

        /// <inhertidoc />
        public bool IsDBNull(int i)
        {
            if ((i < 0) ||
                (i > FieldCount))
                throw new IndexOutOfRangeException(i.ToString(CultureInfo.InvariantCulture));
            return _columnValues[i].IsNull();
        }

        /// <inhertidoc />
        public int FieldCount
        {
            get { return _recordSetDefinition.FieldCount; }
        }

        /// <inhertidoc />
        public object this[int i]
        {
            get { return GetValue(i); }
            set { SetValue(i, value); }
        }

        /// <inhertidoc />
        public object this[string name]
        {
            get { return GetValue(GetOrdinal(name)); }
        }

        /// <inhertidoc />
        public RecordSetDefinition RecordSetDefinition
        {
            get { return _recordSetDefinition; }
        }
        #endregion

        /// <summary>
        /// Sets the value. of the column with the specified index.
        /// </summary>
        /// <param name="i">The index.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.ArgumentException">The value is not valid for the specified index.</exception>
        /// <remarks></remarks>
        public void SetValue(int i, object value)
        {
            if ((i < 0) ||
                (i > FieldCount))
                throw new IndexOutOfRangeException(i.ToString(CultureInfo.InvariantCulture));

            // Check to see if value is changing
            if (_columnValues[i] == value)
                return;

            // Validate value
            object sqlValue;
            if (!_recordSetDefinition[i].Validate(value, out sqlValue))
                throw new ArgumentException(
                    string.Format(
                        "Cannot set the value of column '{0}' to {1}.",
                        _recordSetDefinition[i],
                        value == null ? "null" : "'" + value + "'"),
                    "value");

            _columnValues[i] = sqlValue;
        }
    }
}