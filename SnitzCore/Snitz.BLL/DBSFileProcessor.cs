using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Snitz.IDAL;

namespace Snitz.BLL
{
    public class DbsFileProcessor
    {
        private XDocument dbsDocument=null;
        private string _dbType = "";

        public DbsFileProcessor(string filename)
        {
            dbsDocument = XDocument.Load(filename);
            ISetup dal = Factory<ISetup>.Create("SnitzSetup");
            _dbType = dal.CheckVersion();
        }

        public string Process()
        {

            XElement createtables = dbsDocument.Element("Create");
            if (createtables != null)
            {
                CreateTables(createtables);
            }
            XElement altertables = dbsDocument.Element("Alter");
            if (altertables != null)
            {
                AlterTables(altertables);
            }
            XElement updatetables = dbsDocument.Element("Update");
            if (updatetables != null)
            {
                TableUpdates(updatetables);
            }
            XElement inserttables = dbsDocument.Element("Insert");
            if (inserttables != null)
            {
                TableInserts(inserttables);
            }
            XElement deletetables = dbsDocument.Element("Delete");
            if (deletetables != null)
            {
                TableDeletes(deletetables);
            }
            XElement droptables = dbsDocument.Element("Drop");
            if (droptables != null)
            {
                DropTables(droptables);
            }
            return null;
        }

        private void DropTables(XElement droptables)
        {
            IEnumerable<XElement> tables = droptables.Elements("Table");
            foreach (var table in tables)
            {
                //Create table sql

            }
        }

        private void TableDeletes(XElement deletetables)
        {
            IEnumerable<XElement> tables = deletetables.Elements("Table");
            foreach (var table in tables)
            {
                //Create table sql

            }
        }

        private void TableInserts(XElement inserttables)
        {
            IEnumerable<XElement> tables = inserttables.Elements("Table");
            foreach (var table in tables)
            {
                //Create table sql
                foreach (var column in table.Elements("Column"))
                {

                }
            }
        }

        private void TableUpdates(XElement updatetables)
        {
            IEnumerable<XElement> tables = updatetables.Elements("Table");
            foreach (var table in tables)
            {
                //Create table sql
                foreach (var column in table.Elements("Column"))
                {

                }
            }
        }

        private void AlterTables(XElement altertables)
        {
            ISetup dal = Factory<ISetup>.Create("SnitzSetup");
            StringBuilder sql = new StringBuilder();
            IEnumerable<XElement> tables = altertables.Elements("Table");
            foreach (var table in tables)
            {
                //Create table sql
                var xTable = table.Attribute("name");
                if (xTable != null)
                {
                    foreach (var column in table.Elements("Column"))
                    {
                        sql.Length = 0;
                        var xColumn = column.Attribute("name");
                        if (xColumn != null)
                        {
                            sql.AppendFormat("ALTER TABLE {0} {1} [COLUMN] {2} {3} {4} {5}",
                                xTable.Value,
                                column.Attribute("action").Value,
                                xColumn.Value,
                                ColumnType(column.Attribute("type").Value),
                                ColumnSize(column.Attribute("size").Value),
                                DefaultVal(column.Attribute("default").Value, column.Attribute("type").Value));
                            var res = dal.ExecuteScript(sql.ToString());
                            if(!String.IsNullOrEmpty(res))
                                throw new Exception(res);
                        }
                    }
                }
            }
        }

        private void CreateTables(XElement createtables)
        {
            ISetup dal = Factory<ISetup>.Create("SnitzSetup");
            IEnumerable<XElement> tables = createtables.Elements("Table");
            foreach (var table in tables)
            {
                var xTable = table.Attribute("name");

                if (xTable != null)
                {
                    StringBuilder sql = new StringBuilder("CREATE TABLE " + xTable.Value + " (");
                    if (table.Attribute("idfield") != null)
                    {
                        switch (_dbType)
                        {
                            case "access":
                                sql = sql.AppendLine(table.Attribute("idfield") + " COUNTER CONSTRAINT PrimaryKey PRIMARY KEY,");
                                break;
                            case "mssql":
                                sql = sql.AppendLine(table.Attribute("idfield") + " int IDENTITY (1, 1) PRIMARY KEY NOT NULL,");
                                break;
                            case "mysql":
                                sql = sql.AppendLine(table.Attribute("idfield") + " INT (11) NOT NULL auto_increment,");
                                break;
                        }

                    }
                    bool first = true;
                    foreach (var column in table.Elements("Column"))
                    {
                        if (!first) sql.AppendLine(", ");
                        sql.AppendFormat("{0} {1} {2} {3} {4}", column.Attribute("name").Value,
                            ColumnType(column.Attribute("type").Value),
                            ColumnSize(column.Attribute("size").Value),
                            ColumnNull(column.Attribute("allownulls").Value),
                            DefaultVal(column.Attribute("default").Value, column.Attribute("type").Value));
                        first = false;
                    }

                    if (_dbType == "mysql" && table.Attribute("idfield") != null)
                    {
                        sql.AppendFormat(",KEY {0}_{1} ({1})", xTable.Value,
                            table.Attribute("idfield").Value);
                    }
                    sql.AppendLine(")");
                    dal.ExecuteScript(sql.ToString());
                }

            }
        }

        private string ColumnSize(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                switch (value)
                {
                    case "smallint":
                        if (_dbType == "mysql")
                        {
                            if (string.IsNullOrEmpty(value))
                                value = "6";
                            return "(" + value + ")";
                        }
                        return "";
                    case "int":
                        if (_dbType == "mysql")
                        {
                            if (string.IsNullOrEmpty(value))
                                value = "11";
                            return "(" + value + ")";
                        }
                        return "";
                    case "nvarchar":
                    case "varchar":
                        return "(" + value + ")";
                    case "date":
                        return "";
                    default:
                        return "";
                }

            }
            return "";
        }

        private string DefaultVal(string value, string type)
        {
            if (!String.IsNullOrEmpty(value))
            {
                switch (type)
                {
                    case "smallint":
                        return "DEFAULT " + value;
                    case "int":
                        return "DEFAULT " + value;
                    case "nvarchar":
                    case "varchar":
                        return "DEFAULT '" + value + "'";
                    case "date":
                        return "DEFAULT '" + value + "'";
                    default :
                        return "";
                }
                
            }
            return "";
        }

        private String ColumnNull(string value)
        {
            if (String.IsNullOrEmpty(value) || value == "true")
            {
                return "NULL";
            }
            return "NOT NULL";
        }

        private string ColumnType(string value)
        {
            switch (_dbType)
            {
                case "access" :
                    switch (value)
                    {
                        case "smallint" :
                            return value;
                        case "int" :
                            return value;
                        case "nvarchar":
                        case "varchar" :
                            return "text";
                        case "memo" :
                            return value;
                        case "date" :
                            return value;
                    }
                    break;
                case "mssql" :
                    switch (value)
                    {
                        case "smallint":
                            return value;
                        case "int":
                            return value;
                        case "nvarchar":
                        case "varchar":
                            return value;
                        case "memo":
                            return "ntext";
                        case "date":
                            return "datetime";
                    }
                    break;
                case "mysql" :
                    break;
            }
            return String.Empty;
        }
    }
}
