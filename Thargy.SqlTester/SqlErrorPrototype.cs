#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Solution: Thargy.SqlTester 
// Project: Thargy.SqlTester
// File: SqlErrorPrototype.cs
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace Thargy.SqlTester
{
    /// <summary>
    /// Allows creation of a <see cref="SqlError"/>.
    /// </summary>
    /// <remarks></remarks>
    public class SqlErrorPrototype
    {
        /// <summary>
        /// Function to creates a <see cref="SqlError"/>.
        /// </summary>
        /// <remarks>
        /// This calls the
        /// internal SqlError(int infoNumber, byte errorState, byte errorClass, string server, string errorMessage, string procedure, int lineNumber, uint win32ErrorCode)
        /// constructor.</remarks>
        [NotNull] private static readonly Func<int, byte, byte, string, string, string, int, uint, SqlError>
            _constructor;

        /// <summary>
        /// The equivalent <see cref="SqlError"/>.
        /// </summary>
        [NotNull] public readonly SqlError SqlError;

        /// <summary>
        /// Creates the <see cref="_constructor"/> lambda.
        /// </summary>
        /// <remarks></remarks>
        static SqlErrorPrototype()
        {
            // Find SqlError constructor.
            ConstructorInfo constructorInfo =
                typeof (SqlError).GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null,
                    new[]
                        {
                            typeof (int),
                            typeof (byte),
                            typeof (byte),
                            typeof (string),
                            typeof (string),
                            typeof (string),
                            typeof (int),
                            typeof (uint)
                        }, null);
            Contract.Assert(constructorInfo != null);

            // Create parameters
            List<ParameterExpression> parameters = new List<ParameterExpression>(4)
                                                       {
                                                           Expression.Parameter(typeof (int), "infoNumber"),
                                                           Expression.Parameter(typeof (byte), "errorState"),
                                                           Expression.Parameter(typeof (byte), "errorClass"),
                                                           Expression.Parameter(typeof (string), "server"),
                                                           Expression.Parameter(typeof (string), "errorMessage"),
                                                           Expression.Parameter(typeof (string), "procedure"),
                                                           Expression.Parameter(typeof (int), "lineNumber"),
                                                           Expression.Parameter(typeof (uint), "win32ErrorCode")
                                                       };

            // Create lambda expression.
            _constructor = Expression.Lambda<Func<int, byte, byte, string, string, string, int, uint, SqlError>>(
                Expression.New(constructorInfo, parameters), parameters).Compile();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlErrorPrototype" /> class, and in doing so initializes the <see cref="SqlError" /> property.
        /// </summary>
        /// <param name="infoNumber">The info number.</param>
        /// <param name="errorState">State of the error.</param>
        /// <param name="errorClass">The error class.</param>
        /// <param name="server">The server.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="procedure">The procedure.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="win32ErrorCode">The win32 error code (if this error is the first in a <see cref="SqlException">SqlException's</see> collection then
        /// this value will create an <see cref="SqlException.InnerException">inner exception</see> of type <see cref="Win32Exception"/>.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <remarks></remarks>
        public SqlErrorPrototype(int infoNumber, byte errorState, byte errorClass = 17,
                                 string server = "Unspecified server", string errorMessage = "Unspecified error",
                                 string procedure = "Unspecified procedure", int lineNumber = 0, uint win32ErrorCode = 0)
            : this(
                _constructor(infoNumber, errorState, errorClass, server, errorMessage, procedure, lineNumber,
                             win32ErrorCode))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlErrorPrototype" /> class.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <remarks></remarks>
        public SqlErrorPrototype([NotNull] SqlError error)
        {
            Contract.Assert(error != null);
            SqlError = error;
        }

        /// <summary>
        /// Gets the name of the provider that generated the error.
        /// </summary>
        /// 
        /// <returns>
        /// The name of the provider that generated the error.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public string Source
        {
            get { return SqlError.Source; }
        }

        /// <summary>
        /// Gets a number that identifies the type of error.
        /// </summary>
        /// 
        /// <returns>
        /// The number that identifies the type of error.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public int Number
        {
            get { return SqlError.Number; }
        }

        /// <summary>
        /// Gets a numeric error code from SQL Server that represents an error, warning or "no data found" message.
        /// </summary>
        /// 
        /// <returns>
        /// The number that represents the error code.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public byte State
        {
            get { return SqlError.State; }
        }

        /// <summary>
        /// Gets the severity level of the error returned from SQL Server.
        /// </summary>
        /// 
        /// <returns>
        /// A value from 1 to 25 that indicates the severity level of the error. The default is 0.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public byte Class
        {
            get { return SqlError.Class; }
        }

        /// <summary>
        /// Gets the name of the instance of SQL Server that generated the error.
        /// </summary>
        /// 
        /// <returns>
        /// The name of the instance of SQL Server.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public string Server
        {
            get { return SqlError.Server; }
        }

        /// <summary>
        /// Gets the text describing the error.
        /// </summary>
        /// 
        /// <returns>
        /// The text describing the error.For more information on errors generated by SQL Server, see SQL Server Books Online.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public string Message
        {
            get { return SqlError.Message; }
        }

        /// <summary>
        /// Gets the name of the stored procedure or remote procedure call (RPC) that generated the error.
        /// </summary>
        /// 
        /// <returns>
        /// The name of the stored procedure or RPC.For more information on errors generated by SQL Server, see SQL Server Books Online.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public string Procedure
        {
            get { return SqlError.Procedure; }
        }

        /// <summary>
        /// Gets the line number within the Transact-SQL command batch or stored procedure that contains the error.
        /// </summary>
        /// 
        /// <returns>
        /// The line number within the Transact-SQL command batch or stored procedure that contains the error.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public int LineNumber
        {
            get { return SqlError.LineNumber; }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return SqlError.ToString();
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlErrorPrototype"/> to <see cref="SqlError"/>.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlError(SqlErrorPrototype prototype)
        {
            return prototype != null
                       ? prototype.SqlError
                       : null;
        }

        /// <summary>
        /// Implicit conversion from <see cref="SqlError" /> to <see cref="SqlErrorPrototype" />.
        /// </summary>
        /// <param name="sqlError">The SQL error.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlErrorPrototype(SqlError sqlError)
        {
            return sqlError != null
                       ? new SqlErrorPrototype(sqlError)
                       : null;
        }
    }
}