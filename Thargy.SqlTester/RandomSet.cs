#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Solution: Thargy.SqlTester 
// Project: Thargy.SqlTester
// File: RandomSet.cs
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
using System.Linq;
using JetBrains.Annotations;

namespace Thargy.SqlTester
{
    /// <summary>
    /// Create a random record set where the values and columns are created randomly at runtime.
    /// </summary>
    /// <remarks></remarks>
    public class RandomSet : ObjectSet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSet" /> class.
        /// </summary>
        /// <param name="columns">The number of columns, if less than one, then a random number is chosen.</param>
        /// <param name="minRows">The minimum number of rows [defaults to 0].</param>
        /// <param name="maxRows">The maximum number of rows [defaults to 1000].</param>
        /// <param name="nullProbability">The probability of a column's value being set to SQL null (0.0 for no nulls) [Defaults to 0.1 = 10%].</param>
        /// <param name="columnGenerators">The column generators is an array of functions that generate a value for each column, if the function is
        /// <see langword="null"/> for a particular index then a random value is generated, if it is not null then the function is used.  The function takes
        /// the current row number as it's only parameter and must return an object of the correct type for the column.</param>
        /// <remarks></remarks>
        public RandomSet(
            int columns = 0,
            int minRows = 0,
            int maxRows = 1000,
            double nullProbability = 0.1,
            Func<int, object>[] columnGenerators = null)
            : this(
                Tester.RandomGenerator.RandomRecordSetDefinition(columns), minRows, maxRows, nullProbability,
                columnGenerators)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSet" /> class.
        /// </summary>
        /// <param name="recordSetDefinition">The record set definition.</param>
        /// <param name="minRows">The min rows.</param>
        /// <param name="maxRows">The max rows.</param>
        /// <param name="nullProbability">The probability of a column's value being set to SQL null (0.0 for no nulls) [Defaults to 0.1 = 10%].</param>
        /// <param name="columnGenerators">The column generators is an array of functions that generate a value for each column, if the function is
        /// <see langword="null"/> for a particular index then a random value is generated, if it is not null then the function is used.  The function takes
        /// the current row number as it's only parameter and must return an object of the correct type for the column.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <remarks></remarks>
        public RandomSet([NotNull] RecordSetDefinition recordSetDefinition, int minRows = 0, int maxRows = 1000,
                         double nullProbability = 0.1,
                         Func<int, object>[] columnGenerators = null)
            : base(
                recordSetDefinition,
                GenerateRecords(recordSetDefinition, minRows, maxRows, nullProbability, columnGenerators))
        {
        }

        /// <summary>
        /// Generates the records.
        /// </summary>
        /// <param name="recordSetDefinition">The record set definition.</param>
        /// <param name="minRows">The min rows.</param>
        /// <param name="maxRows">The max rows.</param>
        /// <param name="nullProbability">The probability of a column's value being set to SQL null (0.0 for no nulls) [Defaults to 0.1 = 10%].</param>
        /// <param name="columnGenerators">The column generators is an array of functions that generate a value for each column, if the function is
        /// <see langword="null"/> for a particular index then a random value is generated, if it is not null then the function is used.  The function takes
        /// the current row number as it's only parameter and must return an object of the correct type for the column.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        ///   <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <remarks></remarks>
        [NotNull]
        private static IEnumerable<IObjectRecord> GenerateRecords([NotNull] RecordSetDefinition recordSetDefinition,
                                                                  int minRows, int maxRows, double nullProbability,
                                                                  Func<int, object>[] columnGenerators = null)
        {
            if (minRows < 0)
                throw new ArgumentOutOfRangeException("minRows", minRows,
                                                      String.Format(
                                                          "The minimum number of rows '{0}' cannot be negative.",
                                                          minRows));
            if (maxRows < 0)
                throw new ArgumentOutOfRangeException("maxRows", maxRows,
                                                      String.Format(
                                                          "The maximum number of rows '{0}' cannot be negative.",
                                                          maxRows));

            if (minRows > maxRows)
            {
                throw new ArgumentOutOfRangeException("minRows", minRows,
                                                      String.Format(
                                                          "The minimum number of rows '{0}' cannot exceed the maximum number of rows '{1}'.",
                                                          minRows,
                                                          maxRows));
            }

            // Calculate number of rows.
            int rows = minRows == maxRows
                           ? minRows
                           : Tester.RandomGenerator.Next(minRows, maxRows);

            if (rows < 1)
                return Enumerable.Empty<IObjectRecord>();

            // Create random records
            List<IObjectRecord> records = new List<IObjectRecord>();
            for (int r = 0; r < rows; r++)
                records.Add(new ObjectRecord(recordSetDefinition, true, nullProbability, columnGenerators, r + 1));

            return records;
        }
    }
}