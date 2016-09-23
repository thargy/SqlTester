#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Solution: Thargy.SqlTester 
// Project: Thargy.SqlTester.Test
// File: Examples.cs
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
using System.Data.SqlClient;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Thargy.SqlTester.Test
{
    [TestClass]
    public class Examples
    {
        /// <summary>
        /// Example code
        /// </summary>
        [TestMethod]
        public void RecordExample()
        {
            // To create a record that implement IDataRecord we start with a record set definition.
            RecordSetDefinition recordSetDefinition = new RecordSetDefinition(
                new ColumnDefinition("ID", SqlDbType.Int),
                new ColumnDefinition("Name", SqlDbType.Char, 50),
                new ColumnDefinition("Description", SqlDbType.NVarChar),
                // This column is not nullable so defaults to true
                new ColumnDefinition("Active", SqlDbType.Bit, isNullable: false, defaultValue: true)
                );

            // Now we can create a record
            IObjectRecord dataRecord = new ObjectRecord(recordSetDefinition, 1, "Test", "This is my test record");

            // Or we can create one with random values
            IObjectRecord randomRecord = new ObjectRecord(recordSetDefinition, true);

            // To create a record that throws an exception we first create a SqlException
            // We can't do this directly, but we can use our prototypes to construct one.

            // SqlExceptions are made from a collection of SqlErrors - which can make like this :
            SqlErrorCollection errorCollection = new SqlErrorCollectionPrototype
                                                     {
                                                         new SqlErrorPrototype(1000, 80, 17, "MyFakeServer",
                                                                               "Connection Timeout.", "spMySproc", 54)
                                                     };

            SqlException sqlException = new SqlExceptionPrototype(errorCollection, "9.0.0.0", Guid.NewGuid());
            IObjectRecord exceptionRecord = new ExceptionRecord(sqlException);

            // We can stick these records into a recordset
            // Note the records must have the same RecordSetDefinition (unless it's an exception record)
            // The final record will through an exception when reached!
            ObjectSet recordSet = new ObjectSet(recordSetDefinition)
                                      {
                                          dataRecord,
                                          randomRecord,
                                          //exceptionRecord
                                      };

            // We can add recordsets to an ObjectReader
            ObjectReader reader = new ObjectReader
                                      {
                                          recordSet
                                      };

            // We can also add random record sets - this one has the same definition as the first.
            reader.Add(new RandomSet(recordSetDefinition));

            // We can also fix certain rows values using the column generators arry, a null indicates
            // that the column should us a random value, otherwise a lambda can be supplied - in this case
            // it sets the row to the row number (1 - indexed).
            reader.Add(new RandomSet(recordSetDefinition,
                                     columnGenerators: new Func<int, object>[] {null, row => "Row #" + row}));

            // Whereas this one has a random set of columns (with random types).
            reader.Add(new RandomSet(10));

            // Now that we have a reader we can use it like a normal reader - it even simulates disposal.
            using (IDataReader dataReader = reader)
            {
                int recordset = 1;
                do
                {
                    Trace.Write("Recordset #" + recordset);
                    int rows = 0;
                    while (dataReader.Read())
                        rows++;
                    Trace.WriteLine(" - " + rows + " rows.");
                    recordset++;
                } while (dataReader.NextResult());
            }
        }
    }
}