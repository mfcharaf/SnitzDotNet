using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Snitz.IDAL;

namespace Snitz.BLL
{
    public class DbsFileProcessor
    {
        public bool Applied { get; set; }
        private XDocument dbsDocument;
        private string _dbType = "";
        private string _filename = "";
        private StringBuilder _errors;

        public DbsFileProcessor(string filename)
        {
            _filename = filename;
            dbsDocument = XDocument.Load(filename);
            ISetup dal = Factory<ISetup>.Create("SnitzSetup");
            _dbType = dal.CheckVersion();
            XElement root = dbsDocument.Element("Tables");
            if (root != null) Applied = Convert.ToBoolean(root.Attribute("applied").Value);
        }

        public string Process()
        {
            if (Applied)
            {
                throw new Exception("File already processed");
                return null;
            }

            XElement root = dbsDocument.Element("Tables");

            try
            {
                _errors = new StringBuilder();
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
                    XElement indices = root.Element("Indexing");
                    if (indices != null)
                    {
                        CreateIndices(indices);
                    }
                }
                root.SetAttributeValue("applied", true);
                dbsDocument.Save(_filename);
            }
            catch (Exception ex)
            {
                _errors.AppendLine(ex.Message);
            }
            
            if(!String.IsNullOrEmpty(_errors.ToString()))
                return _errors.ToString();

            return "Success";
        }

        private void CreateIndices(XElement indices)
        {
            ISetup dal = Factory<ISetup>.Create("SnitzSetup");
            IEnumerable<XElement> tables = indices.Elements("Table");
            foreach (var table in tables)
            {
                var xTable = table.Attribute("name");
                foreach (var index in table.Elements("Index"))
                {
                    //<Index name="PK_FORUM_CATEGORY" columns="CAT_ID,COL_2" direction="ASC" unique="true"/>
                    //CREATE [ UNIQUE ] INDEX index ON table (field [ASC|DESC][, field [ASC|DESC], …]) [WITH { PRIMARY | DISALLOW NULL | IGNORE NULL }]
                    StringBuilder sql = new StringBuilder();
                    string unique = "";
                    if (index.Attribute("unique") != null)
                        unique = "UNIQUE";
                    sql.AppendFormat("CREATE {0} INDEX {1} ON {2} (", unique, index.Attribute("name").Value,
                        xTable.Value);
                    var columns = index.Attribute("columns").Value.Split(',');
                    bool first = true;
                    foreach (string column in columns)
                    {
                        if (!first) sql.Append(", ");
                        sql.AppendFormat("{0} {1}", column, index.Attribute("direction").Value);
                        first = false;
                    }
                    sql.AppendLine(")");
                    try
                    {
                        var ret = dal.ExecuteScript(sql.ToString());
                        if (ret != null)
                            _errors.AppendFormat("CreateIndices: {0}", ret).AppendLine();
                    }
                    catch(Exception ex)
                    {
                        _errors.AppendFormat("CreateIndices: {0}", ex.Message);
                    }
                }
            }
        }

        private void DropTables(XElement droptables)
        {
            ISetup dal = Factory<ISetup>.Create("SnitzSetup");
            IEnumerable<XElement> tables = droptables.Elements("Table");
            foreach (var table in tables)
            {
                var xTable = table.Attribute("name");
                StringBuilder sql = new StringBuilder();
                if (xTable != null) sql.AppendFormat("DROP TABLE {0}", xTable.Value);
                try
                {
                    var ret = dal.ExecuteScript(sql.ToString());
                    if (ret != null)
                        _errors.AppendFormat("DropTables: {0}", ret).AppendLine();
                }
                catch (Exception ex)
                {
                    _errors.AppendFormat("DropTables: {0}", ex.Message);
                }
            }
        }

        private void TableDeletes(XElement deletetables)
        {
            ISetup dal = Factory<ISetup>.Create("SnitzSetup");
            IEnumerable<XElement> tables = deletetables.Elements("Table");
            foreach (var table in tables)
            {
                var xTable = table.Attribute("name");
                var xWhere = table.Attribute("condition");
                StringBuilder sql = new StringBuilder();
                if (xTable != null)
                    if (xWhere != null) sql.AppendFormat("DELETE FROM {0} WHERE {1}", xTable.Value, xWhere.Value);
                try
                {
                    var ret = dal.ExecuteScript(sql.ToString());
                    if (ret != null)
                        _errors.AppendFormat("TableDeletes: {0}", ret).AppendLine();
                }
                catch (Exception ex)
                {
                    _errors.AppendFormat("TableDeletes: {0}", ex.Message);
                }
            }
        }

        private void TableInserts(XElement inserttables)
        {
            ISetup dal = Factory<ISetup>.Create("SnitzSetup");
            IEnumerable<XElement> tables = inserttables.Elements("Table");
            foreach (var table in tables)
            {
                bool first = true;
                var xTable = table.Attribute("name");
                StringBuilder sql = new StringBuilder("INSERT INTO TABLE " + xTable).AppendLine();
                StringBuilder cols = new StringBuilder();
                StringBuilder vals = new StringBuilder();
                foreach (var column in table.Elements("Column"))
                {
                    var xColumn = column.Attribute("name");
                    var xValue = column.Attribute("value");
                    var xType = column.Attribute("type");
                    if (!first)
                    {
                        cols.Append(",");
                        vals.Append(",");
                    }
                    if (xColumn != null) cols.Append(xColumn.Value);
                    if (xType != null && (xType.Value == "varchar" || xType.Value == "memo"))
                        vals.Append("'");
                    if (xValue != null) vals.Append(xValue.Value);
                    if (xType != null && (xType.Value == "varchar" || xType.Value == "memo"))
                        vals.Append("'");
                    first = false;
                }
                sql.AppendFormat("({0}) VALUES ({1})",cols.ToString(),vals.ToString());
                try
                {
                    var ret = dal.ExecuteScript(sql.ToString());
                    if (ret != null)
                        _errors.AppendFormat("TableInserts: {0}", ret).AppendLine();
                }
                catch (Exception ex)
                {
                    _errors.AppendFormat("TableInserts: {0}", ex.Message);
                }
            }
        }

        private void TableUpdates(XElement updatetables)
        {
            ISetup dal = Factory<ISetup>.Create("SnitzSetup");

            IEnumerable<XElement> tables = updatetables.Elements("Table");
            foreach (var table in tables)
            {
                var xWhere = table.Attribute("condition");
                var xTable = table.Attribute("name");

                StringBuilder sql = new StringBuilder("UPDATE " + xTable.Value + " SET ").AppendLine();
                bool first = true;
                foreach (XElement column in table.Elements("Column"))
                {
                    var xColumn = column.Attribute("name");
                    var xValue = column.Attribute("value");
                    var xType = column.Attribute("type");
                    //SqlDbType type = (SqlDbType)Enum.Parse(typeof(SqlDbType), xType.Value, true);
                    
                    if (!first)
                        sql.Append(",");
                    if (xColumn != null) sql.Append(xColumn.Value).Append("=");
                    if (xType != null && (xType.Value == "varchar" || xType.Value == "memo"))
                        sql.Append("'");
                    if (xValue != null) sql.Append(xValue.Value);
                    if (xType != null && (xType.Value == "varchar" || xType.Value == "memo"))
                        sql.Append("'");
                    sql.AppendLine();

                    first = false;
                }
                if (xWhere != null && !String.IsNullOrEmpty(xWhere.Value))
                    sql.AppendLine(xWhere.Value);
                try
                {
                    var ret = dal.ExecuteScript(sql.ToString());
                    if (ret != null)
                        _errors.AppendFormat("TableUpdates: {0}", ret).AppendLine();
                }
                catch (Exception ex)
                {
                    _errors.AppendFormat("TableUpdates: {0}", ex.Message);
                }
            }
        }

        /// <summary>
        /// AlterTables
        /// </summary>
        /// <param name="altertables"></param>
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
                            sql.AppendFormat("ALTER TABLE {0} {1} ",xTable.Value,action.Value);
                            if (_dbType == "access" || action.Value != "ADD")
                            {
                                sql.Append(" COLUMN ");
                            }

                            sql.AppendFormat("{0} {1} {2} {3} {4}",
                                xColumn.Value,
                                ColumnType(column.Attribute("type").Value),
                                ColumnSize(column.Attribute("size"), column.Attribute("type").Value),
                                DefaultVal(column.Attribute("default"), column.Attribute("type").Value),
                                ColumnNull(column.Attribute("allownulls").Value));
                            try
                            {
                                var ret = dal.ExecuteScript(sql.ToString());
                                if (ret != null)
                                    _errors.AppendFormat("<b>AlterTables:</b>{0}", ret).AppendLine();
                            }
                            catch (Exception ex)
                            {
                                _errors.AppendFormat("AlterTables: {0}", ex.Message);
                            }

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
                var dropfirst = table.Attribute("droprename");
                string sqlDrop = "";

                if (xTable != null)
                {
                    if (dropfirst != null)
                    {
                        if (dropfirst.Value == "")
                        {
                            sqlDrop =
                                "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[" + xTable.Value + "]') AND type in (N'U')) " +
                                "DROP TABLE [" + xTable.Value + "];";
                            
                        }else if (dropfirst.Value == "rename")
                        {
                            sqlDrop = "EXEC sp_rename '" + xTable.Value + "', '" + xTable.Value + "_BAK';";
                        }

                        //

                    }                    
                    StringBuilder sql = new StringBuilder(sqlDrop);
                    sql.AppendLine("");
                    sql.Append("CREATE TABLE " + xTable.Value + " (");
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
                            ColumnSize(column.Attribute("size"), column.Attribute("type").Value),
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
                    try
                    {
                        var ret = dal.ExecuteScript(sql.ToString());
                        if (ret != null)
                            _errors.AppendFormat("<b>CreateTables:</b> {0}", ret).Append("</br>");
                    }
                    catch (Exception ex)
                    {
                        _errors.AppendFormat("CreateTables: {0}", ex.Message).Append("</br>"); 
                    }
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
                    try
                    {
                        dal.ExecuteScript(sql.ToString());
                    }
                    catch (Exception ex)
                    {
                        _errors.AppendFormat("PrimaryKeys: {0}", ex.Message);
                    }
                }
            }
        }

        private string ColumnSize(XAttribute size, string type)
        {
            if (size != null)
            {
                switch (type)
                {
                    case "smallint":
                        if (_dbType == "mysql")
                        {
                            return "(" + size.Value + ")";
                        }
                        return "";
                    case "int":
                        if (_dbType == "mysql")
                        {
                            return "(" + size.Value + ")";
                        }
                        return "";
                    case "nvarchar":
                    case "varchar":
                        return "(" + size.Value + ")";
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
                    case "int":
                        return "DEFAULT " + value.Value;
                    case "float":
                        return "DEFAULT '" + value.Value + "'";
                    case "nvarchar":
                    case "varchar":
                        return "DEFAULT '" + value.Value + "'";
                    case "date":
                        return "DEFAULT '" + value.Value + "'";
                    default :
                        return value.Value;
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
                            return "nvarchar(MAX)";
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
