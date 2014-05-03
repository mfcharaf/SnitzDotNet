using System;
using System.Data.OleDb;
using System.Globalization;

namespace Snitz.OLEDbDAL
{
    public static class SQLExtensions
    {
        /// <summary>
        /// Safely handles null values when fetching strings from a DataReader
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="colIndex"></param>
        /// <returns></returns>
        public static string SafeGetString(this OleDbDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetString(colIndex);

            return string.Empty;
        }
        public static string SafeGetString(this OleDbDataReader reader, string colName)
        {
            int col = reader.GetOrdinal(colName);
            if (!reader.IsDBNull(col))
                return reader.GetString(col);

            return string.Empty;
        }
        /// <summary>
        /// Safely handles null values when fetching integers from a DataReader
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="colIndex"></param>
        /// <returns></returns>
        public static int? SafeGetInt32(this OleDbDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetInt32(colIndex);

            return null;
        }
        public static int SafeGetInt32(this OleDbDataReader reader, string colName)
        {
            int col = reader.GetOrdinal(colName);
            if (!reader.IsDBNull(col))
                return reader.GetInt32(col);

            return 0;
        }
        public static int SafeGetInt16(this OleDbDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetInt16(colIndex);

            return 0;
        }
        /// <summary>
        /// Fetches Snitz date strings from a DataReader and converts to a system DateTime?
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="colIndex"></param>
        /// <returns></returns>
        public static DateTime? GetSnitzDate(this OleDbDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                //CultureInfo provider = CultureInfo.CurrentUICulture;
                CultureInfo ci = CultureInfo.CreateSpecificCulture("en-GB");
                //pad the forumdate incase we are converting the DOB
                try
                {
                    return DateTime.ParseExact(reader.GetString(colIndex).PadRight(14, '0'), "yyyyMMddHHmmss", ci);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;
        }
    }
}
