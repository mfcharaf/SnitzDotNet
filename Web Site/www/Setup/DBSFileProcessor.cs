using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Snitz.IDAL;



namespace SnitzUI.Setup
{
    public class DbsFileProcessor
    {
        private XDocument dbsDocument;
        private string _dbType = "";

        public DbsFileProcessor(string filename)
        {
            dbsDocument = XDocument.Load(filename);
            ISetup dal = Factory<ISetup>.Create("SnitzSetup");
            _dbType = dal.CheckVersion();
        }

        public string Process()
        {
            XElement root = dbsDocument.Element("Tables");

            if (root != null)
            {
                XElement createtables = root.Element("Create");
                if (createtables != null)
                {
                    CreateTables(createtables);
                }
                XElement altertables = root.Element("Alter");
                if (altertables != null)
                {
                    AlterTables(altertables);
                }
                XElement updatetables = root.Element("Update");
                if (updatetables != null)
                {
                    TableUpdates(updatetables);
                }
                XElement inserttables = root.Element("Insert");
                if (inserttables != null)
                {
                    TableInserts(inserttables);
                }
                XElement deletetables = root.Element("Delete");
                if (deletetables != null)
                {
                    TableDeletes(deletetables);
                }
                XElement droptables = root.Element("Drop");
                if (droptables != null)
                {
                    DropTables(droptables);
                }
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
                        var action = column.Attribute("action");

                        if (xColumn != null)
                        {
                            sql.AppendFormat("ALTER TABLE {0} {1} ",
                                xTable.Value,
                                action.Value);
                            if (_dbType == "access" || action.Value != "ADD")
                            {
                                sql.Append(" COLUMN ");
                            }
                            sql.AppendFormat("{0} {1} {2} {3}",
                                xColumn.Value,
                                ColumnType(column.Attribute("type").Value),
                                ColumnSize(column.Attribute("size")),
                                DefaultVal(column.Attribute("default"), column.Attribute("type").Value));
                            dal.ExecuteScript(sql.ToString());

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
                    var idcolumn = table.Attribute("idfield");
                    if (idcolumn != null)
                    {
                        switch (_dbType)
                        {
                            case "access":
                                sql = sql.AppendLine(idcolumn.Value + " COUNTER CONSTRAINT PrimaryKey PRIMARY KEY,");
                                break;
                            case "mssql":
                                sql = sql.AppendLine(idcolumn.Value + " int IDENTITY (1, 1) PRIMARY KEY NOT NULL,");
                                break;
                            case "mysql":
                                sql = sql.AppendLine(idcolumn.Value + " INT (11) NOT NULL auto_increment,");
                                break;
                        }

                    }
                    bool first = true;
                    foreach (var column in table.Elements("Column"))
                    {
                        if (!first) sql.AppendLine(", ");
                        sql.AppendFormat("{0} {1} {2} {3} {4}", column.Attribute("name").Value,
                            ColumnType(column.Attribute("type").Value),
                            ColumnSize(column.Attribute("size")),
                            ColumnNull(column.Attribute("allownulls").Value),
                            DefaultVal(column.Attribute("default"), column.Attribute("type").Value));
                        first = false;
                    }

                    if (_dbType == "mysql" && idcolumn != null)
                    {
                        sql.AppendFormat(",KEY {0}_{1} ({1})", xTable.Value,
                            idcolumn.Value);
                    }
                    sql.AppendLine(")");
                    dal.ExecuteScript(sql.ToString());
                    //create indexes
                    sql.Length = 0;
                    foreach (var index in table.Elements("Index"))
                    {
                        //<Index name="PK_FORUM_CATEGORY" columns="CAT_ID,COL_2" direction="ASC" unique="true"/>
                        //CREATE [ UNIQUE ] INDEX index ON table (field [ASC|DESC][, field [ASC|DESC], …]) [WITH { PRIMARY | DISALLOW NULL | IGNORE NULL }]

                        string unique = "";
                        if (index.Attribute("unique") != null)
                            unique = "UNIQUE";
                        sql.AppendFormat("CREATE {0} INDEX {1} ON {2} (", unique, index.Attribute("name").Value, xTable.Value);
                        var columns = index.Attribute("columns").Value.Split(',');
                        first = true;
                        foreach (string column in columns)
                        {
                            if (!first) sql.Append(", ");
                            sql.AppendFormat("{0} {1}",column,index.Attribute("direction").Value);
                            first = false;
                        }
                        sql.AppendLine(")");
                    }
                    dal.ExecuteScript(sql.ToString());
                }

            }
        }

        private string ColumnSize(XAttribute value)
        {
            if (value != null)
            {
                switch (value.Value)
                {
                    case "smallint":
                        if (_dbType == "mysql")
                        {
                            return "(" + value + ")";
                        }
                        return "";
                    case "int":
                        if (_dbType == "mysql")
                        {
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

        private string DefaultVal(XAttribute value, string type)
        {
            if (value == null) return "";

            if (!String.IsNullOrEmpty(value.Value))
            {
                switch (type)
                {
                    case "smallint":
                        return "DEFAULT " + value.Value;
                    case "int":
                        return "DEFAULT " + value.Value;
                    case "nvarchar":
                    case "varchar":
                        return "DEFAULT '" + value.Value + "'";
                    case "date":
                        return "DEFAULT '" + value.Value + "'";
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
                        case "memo":
                            return value;
                        case "date" :
                            return value;
                        default :
                            return value;
                    }
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
                        case "text" :
                            return "ntext";
                        case "date":
                            return "datetime";
                        case "guid" :
                            return "uniqueidentifier";
                        default :
                            return value;
                    }
                case "mysql" :
                    break;
            }
            return String.Empty;
        }
    }
}
