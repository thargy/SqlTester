#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Solution: Thargy.SqlTester 
// Project: Thargy.SqlTester
// File: IObjectRecord.cs
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

using System.Data;
using JetBrains.Annotations;

namespace Thargy.SqlTester
{
    /// <summary>
    /// Interface that defines data records that are implemented by the test system.
    /// </summary>
    /// <see cref="IDataRecord"/>
    public interface IObjectRecord : IDataRecord
    {
        /// <summary>
        /// Gets the record set definition.
        /// </summary>
        /// <value>The record set definition.</value>
        /// <remarks></remarks>
        [NotNull]
        RecordSetDefinition RecordSetDefinition { get; }
    }
}