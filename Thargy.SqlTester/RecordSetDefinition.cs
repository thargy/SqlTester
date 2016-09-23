#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Solution: Thargy.SqlTester 
// Project: Thargy.SqlTester
// File: RecordSetDefinition.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;

namespace Thargy.SqlTester
{
    /// <summary>
    /// Defines a record set.
    /// </summary>
    /// <remarks></remarks>
    public class RecordSetDefinition : IEnumerable<ColumnDefinition>
    {
        /// <summary>
        /// Special case record set definition used to identify exception records.
        /// </summary>
        [NotNull] public static readonly RecordSetDefinition ExceptionRecord = new RecordSetDefinition(
            new ColumnDefinition("Exception", SqlDbType.Variant));

        /// <summary>
        /// Gets the column definitions array.
        /// </summary>
        /// <value>The column definitions array.</value>
        /// <remarks></remarks>
        [NotNull] private readonly ColumnDefinition[] _columnsArray;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordSetDefinition" /> class.
        /// </summary>
        /// <param name="columnDefinitions">The column definitions.</param>
        /// <remarks></remarks>
        public RecordSetDefinition([NotNull] IEnumerable<ColumnDefinition> columnDefinitions)
        {
            _columnsArray = columnDefinitions.ToArray();

            if (_columnsArray.Length < 1)
                throw new ArgumentOutOfRangeException("columnDefinitions", columnDefinitions,
                                                      "The column definitions must have at least one column.");

            for (int c = 0; c < _columnsArray.Length; c++)
            {
                ColumnDefinition columnDefinition = _columnsArray[c];
                if (columnDefinition == null)
                    throw new ArgumentOutOfRangeException("columnDefinitions", columnDefinitions,
                                                          string.Format(
                                                              "The column definition at index '{0} must not be null.", c));

                if (columnDefinition.RecordSetDefinition != null)
                    throw new InvalidOperationException(
                        "The column definition cannot be added to the recordset definition as it already belongs to a different record set definition.");

                // ReSharper disable HeuristicUnreachableCode
                columnDefinition.RecordSetDefinition = this;
                columnDefinition.Ordinal = c;
                // ReSharper restore HeuristicUnreachableCode
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordSetDefinition" /> class.
        /// </summary>
        /// <param name="columnDefinitions">The column definitions.</param>
        /// <remarks></remarks>
        public RecordSetDefinition([NotNull] params ColumnDefinition[] columnDefinitions)
            : this((IEnumerable<ColumnDefinition>) columnDefinitions)
        {
        }

        /// <summary>
        /// Gets the column definitions in the record set.
        /// </summary>
        /// <value>The column definitions.</value>
        /// <remarks></remarks>
        [NotNull]
        public IEnumerable<ColumnDefinition> Columns
        {
            get { return _columnsArray; }
        }

        /// <summary>
        /// Gets the field count (number of columns).
        /// </summary>
        /// <value>The field count.</value>
        /// <remarks></remarks>
        public int FieldCount
        {
            get { return _columnsArray.Length; }
        }

        /// <summary>
        /// Gets the <see cref="ColumnDefinition" /> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="ColumnDefinition" />.</returns>
        /// <exception cref="System.IndexOutOfRangeException">Thrown if the supplied index is out of range.</exception>
        /// <remarks></remarks>
        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        [NotNull]
        public ColumnDefinition this[int index]
        {
            get
            {
                if ((index < 0) ||
                    (index > FieldCount))
                    throw new IndexOutOfRangeException(index.ToString(CultureInfo.InvariantCulture));
                return _columnsArray[index];
            }
        }


        /// <summary>
        /// Gets the ordinal.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        /// <remarks></remarks>
        public int GetOrdinal(string name)
        {
            CompareInfo compare = CultureInfo.InvariantCulture.CompareInfo;
            for (int c = 0; c < FieldCount; c++)
            {
                if (
                    compare.Compare(_columnsArray[c].Name, name,
                                    CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType |
                                    CompareOptions.IgnoreWidth) == 0)
                    return c;
            }
            throw new IndexOutOfRangeException(name);
        }

        #region Implementation of IEnumerable
        /// <inheritdoc />
        public IEnumerator<ColumnDefinition> GetEnumerator()
        {
            return ((IEnumerable<ColumnDefinition>) _columnsArray).GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}