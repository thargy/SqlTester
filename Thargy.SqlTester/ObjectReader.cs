#region � Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Solution: Thargy.SqlTester 
// Project: Thargy.SqlTester
// File: ObjectReader.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using JetBrains.Annotations;

namespace Thargy.SqlTester
{
    /// <summary>
    /// The object reader, implements <see cref="IDataReader"/> in a way that closely
    /// simulates a <see cref="SqlDataReader"/>.
    /// </summary>
    /// <remarks></remarks>
    public sealed class ObjectReader : IDataReader, ICollection<IObjectSet>
    {
        /// <summary>
        /// Holds the internal record sets.
        /// </summary>
        [NotNull] private readonly List<IObjectSet> _recordSets;

        /// <summary>
        /// Whether the reader is closed.
        /// </summary>
        private bool _isClosed;

        /// <summary>
        /// The internal enumeror over the records in the current set.
        /// </summary>
        [CanBeNull] private IEnumerator<IObjectRecord> _recordEnumerator;

        /// <summary>
        /// The internal enumeror over the sets.
        /// </summary>
        [CanBeNull] private IEnumerator<IObjectSet> _setEnumerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectReader" /> class.
        /// </summary>
        /// <param name="recordSets">The record sets.</param>
        /// <remarks></remarks>
        public ObjectReader(IEnumerable<IObjectSet> recordSets = null)
        {
            _recordSets = recordSets == null
                              ? new List<IObjectSet>()
                              : recordSets.ToList();
        }

        /// <summary>
        /// Gets the current record.
        /// </summary>
        /// <value>The current record.</value>
        /// <remarks></remarks>
        public IObjectSet CurrentSet
        {
            get
            {
                if (_isClosed)
                    return null;

                // Create a set enumerator.
                if (_setEnumerator == null)
                {
                    _recordEnumerator = null;
                    _setEnumerator = _recordSets.GetEnumerator();
                    if (!_setEnumerator.MoveNext())
                        return null;
                }
                return _setEnumerator.Current;
            }
        }

        /// <summary>
        /// Gets the current record.
        /// </summary>
        /// <value>The current record.</value>
        /// <remarks></remarks>
        public IObjectRecord Current
        {
            get
            {
                if (_isClosed)
                    return null;

                IObjectSet set = CurrentSet;
                if (set == null)
                {
                    _recordEnumerator = null;
                    return null;
                }

                // If we haven't got a record enumerator create one.
                if (_recordEnumerator == null)
                {
                    _recordEnumerator = set.GetEnumerator();
                    if (!_recordEnumerator.MoveNext())
                        return null;
                }

                return _recordEnumerator.Current;
            }
        }

        #region ICollection<IObjectSet> Members
        /// <inheritdoc/>
        public IEnumerator<IObjectSet> GetEnumerator()
        {
            return _recordSets.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        public void Add(IObjectSet item)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            if (_setEnumerator != null)
                throw new InvalidOperationException("Cannot modify data reader once it has started being read.");

            _recordSets.Add(item);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            if (_setEnumerator != null)
                throw new InvalidOperationException("Cannot modify data reader once it has started being read.");

            _recordSets.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(IObjectSet item)
        {
            return _recordSets.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(IObjectSet[] array, int arrayIndex)
        {
            _recordSets.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public bool Remove(IObjectSet item)
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            if (_setEnumerator != null)
                throw new InvalidOperationException("Cannot modify data reader once it has started being read.");

            return _recordSets.Remove(item);
        }

        /// <inheritdoc/>
        public int Count
        {
            get { return _recordSets.Count; }
        }

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            get { return false; }
        }
        #endregion

        #region IDataReader Members
        /// <inheritdoc/>
        public void Dispose()
        {
            if (_setEnumerator != null)
                _setEnumerator.Dispose();
            _setEnumerator = null;
            if (_recordEnumerator != null)
                _recordEnumerator.Dispose();
            _recordEnumerator = null;
        }

        /// <inheritdoc/>
        public string GetName(int i)
        {
            IObjectSet set = CurrentSet;
            if (set == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            return set.Definition[i].Name;
        }

        /// <inheritdoc/>
        public string GetDataTypeName(int i)
        {
            IObjectSet set = CurrentSet;
            if (set == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            return set.Definition[i].TypeName;
        }

        /// <inheritdoc/>
        public Type GetFieldType(int i)
        {
            IObjectSet set = CurrentSet;
            if (set == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            return set.Definition[i].ClassType;
        }

        /// <inheritdoc/>
        public int GetOrdinal(string name)
        {
            IObjectSet set = CurrentSet;
            if (set == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            return set.Definition.GetOrdinal(name);
        }

        /// <inheritdoc/>
        public int FieldCount
        {
            get
            {
                IObjectSet set = CurrentSet;
                if (set == null)
                    throw new InvalidOperationException("Invalid attempt to read when no data is present.");

                return set.Definition.FieldCount;
            }
        }

        /// <inheritdoc/>
        public object GetValue(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException(string.Format("Invalid attempt to call {0} when reader is closed.",
                                                                  "GetValue"));
            IObjectRecord record = Current;
            if (record == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");

            return record.GetValue(i);
        }

        /// <inheritdoc/>
        public int GetValues(object[] values)
        {
            if (_isClosed)
                throw new InvalidOperationException(string.Format("Invalid attempt to call {0} when reader is closed.",
                                                                  "GetValues"));

            IObjectRecord record = Current;
            if (record == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");

            return record.GetValues(values);
        }

        /// <inheritdoc/>
        public bool GetBoolean(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException(string.Format("Invalid attempt to call {0} when reader is closed.",
                                                                  "GetBoolean"));
            IObjectRecord record = Current;
            if (record == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");

            return record.GetBoolean(i);
        }

        /// <inheritdoc/>
        public byte GetByte(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException(string.Format("Invalid attempt to call {0} when reader is closed.",
                                                                  "GetByte"));
            IObjectRecord record = Current;
            if (record == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");

            return record.GetByte(i);
        }

        /// <inheritdoc/>
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            if (_isClosed)
                throw new InvalidOperationException(string.Format("Invalid attempt to call {0} when reader is closed.",
                                                                  "GetBytes"));
            IObjectRecord record = Current;
            if (record == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");

            return record.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        /// <inheritdoc/>
        public char GetChar(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException(string.Format("Invalid attempt to call {0} when reader is closed.",
                                                                  "GetChar"));
            IObjectRecord record = Current;
            if (record == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");

            return record.GetChar(i);
        }

        /// <inheritdoc/>
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            if (_isClosed)
                throw new InvalidOperationException(string.Format("Invalid attempt to call {0} when reader is closed.",
                                                                  "GetChars"));
            IObjectRecord record = Current;
            if (record == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");

            return record.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        /// <inheritdoc/>
        public Guid GetGuid(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException(string.Format("Invalid attempt to call {0} when reader is closed.",
                                                                  "GetGuid"));
            IObjectRecord record = Current;
            if (record == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");

            return record.GetGuid(i);
        }

        /// <inheritdoc/>
        public short GetInt16(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException(string.Format("Invalid attempt to call {0} when reader is closed.",
                                                                  "GetInt16"));
            IObjectRecord record = Current;
            if (record == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");

            return record.GetInt16(i);
        }

        /// <inheritdoc/>
        public int GetInt32(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException(string.Format("Invalid attempt to call {0} when reader is closed.",
                                                                  "GetInt32"));
            IObjectRecord record = Current;
            if (record == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");

            return record.GetInt32(i);
        }

        /// <inheritdoc/>
        public long GetInt64(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException(string.Format("Invalid attempt to call {0} when reader is closed.",
                                                                  "GetInt64"));
            IObjectRecord record = Current;
            if (record == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");

            return record.GetInt64(i);
        }

        /// <inheritdoc/>
        public float GetFloat(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException(string.Format("Invalid attempt to call {0} when reader is closed.",
                                                                  "GetFloat"));
            IObjectRecord record = Current;
            if (record == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");

            return record.GetFloat(i);
        }

        /// <inheritdoc/>
        public double GetDouble(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException(string.Format("Invalid attempt to call {0} when reader is closed.",
                                                                  "GetDouble"));
            IObjectRecord record = Current;
            if (record == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");

            return record.GetDouble(i);
        }

        /// <inheritdoc/>
        public string GetString(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException(string.Format("Invalid attempt to call {0} when reader is closed.",
                                                                  "GetString"));
            IObjectRecord record = Current;
            if (record == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");

            return record.GetString(i);
        }

        /// <inheritdoc/>
        public decimal GetDecimal(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException(string.Format("Invalid attempt to call {0} when reader is closed.",
                                                                  "GetDecimal"));
            IObjectRecord record = Current;
            if (record == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");

            return record.GetDecimal(i);
        }

        /// <inheritdoc/>
        public DateTime GetDateTime(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException(string.Format("Invalid attempt to call {0} when reader is closed.",
                                                                  "GetDateTime"));
            IObjectRecord record = Current;
            if (record == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");

            return record.GetDateTime(i);
        }

        /// <inheritdoc/>
        public IDataReader GetData(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException(string.Format("Invalid attempt to call {0} when reader is closed.",
                                                                  "GetData"));
            IObjectRecord record = Current;
            if (record == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");

            return record.GetData(i);
        }

        /// <inheritdoc/>
        public bool IsDBNull(int i)
        {
            if (_isClosed)
                throw new InvalidOperationException(string.Format("Invalid attempt to call {0} when reader is closed.",
                                                                  "IsDBNull"));
            IObjectRecord record = Current;
            if (record == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");

            return record.IsDBNull(i);
        }

        /// <inheritdoc/>
        public object this[int i]
        {
            get { return GetValue(i); }
        }

        /// <inheritdoc/>
        public object this[string name]
        {
            get { return GetValue(GetOrdinal(name)); }
        }

        /// <inheritdoc/>
        public void Close()
        {
            Dispose();
            _isClosed = true;
        }

        /// <inheritdoc/>
        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool NextResult()
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");
            _recordEnumerator = null;

            // Create set enumerator if not present.
            if (_setEnumerator == null)
            {
                _setEnumerator = _recordSets.GetEnumerator();
                if (!_setEnumerator.MoveNext())
                    return false;
            }

            return _setEnumerator.MoveNext();
        }

        /// <inheritdoc/>
        public bool Read()
        {
            if (_isClosed)
                throw new InvalidOperationException("Data reader is closed.");

            // If we haven't got a record enumerator create one.
            if (_recordEnumerator == null)
            {
                IObjectSet set = CurrentSet;
                if (set == null)
                    return false;

                _recordEnumerator = set.GetEnumerator();
                if (!_recordEnumerator.MoveNext())
                    return false;
            }

            return _recordEnumerator.Current != null && _recordEnumerator.MoveNext();
        }

        /// <inheritdoc/>
        public int Depth
        {
            get { return 0; }
        }

        /// <inheritdoc/>
        public bool IsClosed
        {
            get { return _isClosed; }
        }

        /// <inheritdoc/>
        public int RecordsAffected
        {
            get { return -1; }
        }
        #endregion

        /// <summary>
        /// Resets the data reader, clearing enumerators, re-opening and allowing recordset collection to be modified.
        /// </summary>
        /// <remarks></remarks>
        public void Reset()
        {
            _isClosed = false;
            Dispose();
        }
    }
}