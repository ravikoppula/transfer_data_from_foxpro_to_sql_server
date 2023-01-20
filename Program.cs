 
using System.Data.Common;
using System.Data;
using System.Text;
using System.Data.OleDb;
using System.Data.SqlClient;

Console.WriteLine("I'm in Program.cs file.");
string DBF_FileName = "D:\\Workspace\\po2\\temp upgrade\\tables\\gfmas.dbc";

string conn = "Provider=VFPOLEDB;Data Source=" + DBF_FileName;
using (OleDbConnection connection = new OleDbConnection(conn))
{
    connection.Open();
    OleDbCommand command = connection.CreateCommand();
    command.CommandText = "select * from gfmasbshist";
    command.ExecuteNonQuery();
    connection.Close();
}

using (OleDbDataAdapter adapter = new OleDbDataAdapter("select * from " + DBF_FileName, conn))
//connect to the vfp table
{

    DataTable dt = new DataTable();

    adapter.Fill(dt);

    using (SqlConnection myConnection = new SqlConnection("Server=(local);Database=gfmasbshist;Trusted_Connection=True;"))
    {

        StringBuilder sbu = new StringBuilder("Create Table " + DBF_FileName + " (");

        foreach (DataColumn column in dt.Columns)
        {
            sbu.Append(column.ColumnName.ToString() + " varchar (500) not null, ");

        }

        sbu.Remove(sbu.Length - 2, 2);
        sbu.Append(")");
        string sql = sbu.ToString();


        //Here you should analyze the columns by using dt.Columns within a foreach loop,
        //and then get the columnName as well as other
        //info and dynamically create the table based on the SQL. And then use SQLBULK.

        //create a reader for the datatable
        DataTableReader reader = dt.CreateDataReader();
        myConnection.Open();   ///this is my connection to the sql server
        SqlBulkCopy sqlcpy = new SqlBulkCopy(myConnection);

        SqlCommand cmd = new SqlCommand(sql, myConnection);
        cmd.ExecuteNonQuery();

        string SQLTableName = "testTable1";
        sqlcpy.DestinationTableName = SQLTableName;  //copy the datatable to the sql table
        sqlcpy.WriteToServer(dt);

        //sqlcpy.ColumnMappings.Equals(DBF_FileName);

        myConnection.Close();

        reader.Close();

        Console.WriteLine("Congratulations, your .dbf file has been transferred to SQL Server.  " +
            "The name of the new table is " + SQLTableName + ".", "Success");

        //DeleteFile();
    }

}
