#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Solution: Thargy.SqlTester 
// Project: Thargy.SqlTester
// File: ExceptionRecord.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
#endregion

using System;
using System.Data;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace Thargy.SqlTester
{
    /// <summary>
    /// Implements a record that throws an exception.
    /// </summary>
    /// <remarks>
    /// When added to an <see cref="T:Thargy.SqlTester.IObjectSet">IObjectSet</see> will
    /// cause the reader to throw the specified exception once this record is accessed.
    /// </remarks>
    /// <seealso cref="SqlExceptionPrototype">SqlExceptionPrototype</seealso>
    /// <seealso cref="T:Thargy.SqlTester.IObjectSet">IObjectSet</seealso>
    public sealed class ExceptionRecord : IObjectRecord
    {
        /// <summary>
        /// The exception that will be thrown when accessing this record.
        /// </summary>
        [NotNull] public readonly Exception Exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionRecord" /> class.
        /// </summary>
        /// <param name="exception">The exception that will be thrown when accessing this record.</param>
        /// <remarks></remarks>
        public ExceptionRecord([NotNull] Exception exception = null)
        {
            Contract.Requires(exception != null);
            Exception = exception;
        }

        #region IObjectRecord Members
        /// <inheritdoc />
        public string GetName(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public string GetDataTypeName(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public Type GetFieldType(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public object GetValue(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public int GetValues(object[] values)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public int GetOrdinal(string name)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public bool GetBoolean(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public byte GetByte(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public char GetChar(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public Guid GetGuid(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public short GetInt16(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public int GetInt32(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public long GetInt64(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public float GetFloat(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public double GetDouble(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public string GetString(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public decimal GetDecimal(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public DateTime GetDateTime(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public IDataReader GetData(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public bool IsDBNull(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public int FieldCount
        {
            get { throw Exception; }
        }

        /// <inheritdoc />
        object IDataRecord.this[int i]
        {
            get { throw Exception; }
        }

        /// <inheritdoc />
        object IDataRecord.this[string name]
        {
            get { throw Exception; }
        }

        /// <inheritdoc />
        public RecordSetDefinition RecordSetDefinition
        {
            get { return RecordSetDefinition.ExceptionRecord; }
        }
        #endregion
    }
}