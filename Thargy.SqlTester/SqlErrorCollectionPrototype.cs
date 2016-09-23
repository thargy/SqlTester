#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Solution: Thargy.SqlTester 
// Project: Thargy.SqlTester
// File: SqlErrorCollectionPrototype.cs
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
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace Thargy.SqlTester
{
    /// <summary>
    /// Allows creation of a <see cref="SqlErrorCollection"/>, and addition of <see cref="SqlError">SqlErrors</see>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// To add <see cref="SqlError">SqlErrors</see> you will probably want to create them with the <see cref="SqlErrorPrototype"/> class,
    /// which supports implicit conversion to <see cref="SqlError"/>.</para>
    /// <para>In the same way, this class supports implicit conversion to <see cref="SqlErrorCollection"/>.</para>
    /// </remarks>
    /// <seealso cref="SqlErrorPrototype"/>
    public sealed class SqlErrorCollectionPrototype : ICollection, IEnumerable
    {
        /// <summary>
        /// Creates a new <see cref="SqlErrorCollection"/>.
        /// </summary>
        [NotNull] private static readonly Func<SqlErrorCollection> _constructor;

        /// <summary>
        /// Adds a <see cref="SqlError"/> to a <see cref="SqlErrorCollection"/>.
        /// </summary>
        [NotNull] private static readonly Action<SqlErrorCollection, SqlError> _adder;

        /// <summary>
        /// The equivalent <see cref="SqlErrorCollection" />.
        /// </summary>
        [NotNull] public readonly SqlErrorCollection SqlErrorCollection;

        /// <summary>
        /// Creates the <see cref="_constructor"/> function and the <see cref="_adder"/> action.
        /// </summary>
        static SqlErrorCollectionPrototype()
        {
            // Find constructor
            ConstructorInfo constructorInfo =
                typeof (SqlErrorCollection).GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance,
                    null, Type.EmptyTypes, null);
            Contract.Assert(constructorInfo != null);

            // Create lambda expression.
            _constructor =
                Expression.Lambda<Func<SqlErrorCollection>>(Expression.New(constructorInfo),
                                                            Enumerable.Empty<ParameterExpression>()).Compile();

            // Find the add method
            MethodInfo addMethod = typeof (SqlErrorCollection).GetMethod("Add",
                                                                         BindingFlags.NonPublic | BindingFlags.Instance |
                                                                         BindingFlags.InvokeMethod);
            Contract.Assert(addMethod != null);

            // Create instance parameter
            ParameterExpression instanceParameter = Expression.Parameter(typeof (SqlErrorCollection), "collection");

            // Create method parameter
            ParameterExpression errorParamter = Expression.Parameter(typeof (SqlError), "error");

            _adder =
                Expression.Lambda<Action<SqlErrorCollection, SqlError>>(
                    Expression.Call(instanceParameter, addMethod, errorParamter), instanceParameter, errorParamter).
                    Compile();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlErrorCollectionPrototype" /> class.
        /// </summary>
        /// <remarks></remarks>
        public SqlErrorCollectionPrototype() : this(_constructor())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlErrorCollectionPrototype" /> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <remarks></remarks>
        public SqlErrorCollectionPrototype([NotNull] SqlErrorCollection collection)
        {
            Contract.Assert(collection != null);
            SqlErrorCollection = collection;
        }

        /// <summary>
        /// Gets the error at the specified index.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="T:System.Data.SqlClient.SqlError"/> that contains the error at the specified index.
        /// </returns>
        /// <param name="index">The zero-based index of the error to retrieve. </param><exception cref="T:System.IndexOutOfRangeException">Index parameter is outside array bounds. </exception><filterpriority>2</filterpriority>
        public SqlError this[int index]
        {
            get { return SqlErrorCollection[index]; }
        }

        #region ICollection Members
        /// <inheritdoc/>
        public IEnumerator GetEnumerator()
        {
            return SqlErrorCollection.GetEnumerator();
        }

        /// <inheritdoc/>
        public void CopyTo(Array array, int index)
        {
            SqlErrorCollection.CopyTo(array, index);
        }

        /// <inheritdoc/>
        public int Count
        {
            get { return SqlErrorCollection.Count; }
        }

        /// <inheritdoc/>
        public object SyncRoot
        {
            get { return ((ICollection) SqlErrorCollection).SyncRoot; }
        }

        /// <inheritdoc/>
        public bool IsSynchronized
        {
            get { return false; }
        }
        #endregion

        /// <summary>
        /// Adds the specified error to the collection.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <remarks></remarks>
        public void Add(SqlError error)
        {
            _adder(SqlErrorCollection, error);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return SqlErrorCollection.ToString();
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlErrorCollectionPrototype" /> to <see cref="SqlErrorCollection" />.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlErrorCollection(SqlErrorCollectionPrototype prototype)
        {
            return prototype != null
                       ? prototype.SqlErrorCollection
                       : null;
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlErrorCollection" /> to <see cref="SqlErrorCollectionPrototype" />.
        /// </summary>
        /// <param name="collection">The SQL error.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlErrorCollectionPrototype(SqlErrorCollection collection)
        {
            return collection != null
                       ? new SqlErrorCollectionPrototype(collection)
                       : null;
        }
    }
}