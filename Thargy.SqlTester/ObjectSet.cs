#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Solution: Thargy.SqlTester 
// Project: Thargy.SqlTester
// File: ObjectSet.cs
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
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace Thargy.SqlTester
{
    /// <summary>
    /// Holds a collection of records.
    /// </summary>
    /// <remarks></remarks>
    /// <seealso cref="IObjectSet"/>
    public class ObjectSet : IObjectSet, ICollection<IObjectRecord>
    {
        /// <summary>
        /// Holds the <see cref="RecordSetDefinition"/>.
        /// </summary>
        [NotNull] private readonly RecordSetDefinition _definition;

        /// <summary>
        /// The underlying list of records.
        /// </summary>
        [NotNull] private readonly List<IObjectRecord> _records = new List<IObjectRecord>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectSet" /> class.
        /// </summary>
        /// <param name="recordSetDefinition">The record set definition.</param>
        /// <param name="records">The records.</param>
        /// <remarks></remarks>
        public ObjectSet([NotNull] RecordSetDefinition recordSetDefinition, IEnumerable<IObjectRecord> records = null)
        {
            Contract.Requires(recordSetDefinition != null);
            _definition = recordSetDefinition;

            if (records == null)
                return;

            foreach (IObjectRecord record in records)
                Add(record);
        }

        #region ICollection<IObjectRecord> Members
        /// <inheritdoc/>
        public void Add(IObjectRecord item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if ((item.RecordSetDefinition != _definition) &&
                (item.RecordSetDefinition != RecordSetDefinition.ExceptionRecord))
                throw new ArgumentException(
                    "The record must have an identical recordset definition to be added to the current record.", "item");

            _records.Add(item);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _records.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(IObjectRecord item)
        {
            return _records.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(IObjectRecord[] array, int arrayIndex)
        {
            _records.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public bool Remove(IObjectRecord item)
        {
            return _records.Remove(item);
        }

        /// <inheritdoc/>
        public int Count
        {
            get { return _records.Count; }
        }

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            get { return false; }
        }
        #endregion

        #region IObjectSet Members
        /// <inheritdoc/>
        public RecordSetDefinition Definition
        {
            get { return _definition; }
        }

        /// <inheritdoc/>
        public IEnumerator<IObjectRecord> GetEnumerator()
        {
            return _records.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        /// <inheritdoc/>
        public override string ToString()
        {
            return _records.ToString();
        }
    }
}