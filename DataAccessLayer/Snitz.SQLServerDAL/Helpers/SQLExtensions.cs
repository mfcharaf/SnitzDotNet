using System;
using System.Data.SqlClient;
using System.Globalization;

namespace Snitz.SQLServerDAL
{
    public static class SQLExtensions
    {
        /// <summary>
        /// Safely handles null values when fetching strings from a DataReader
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="colIndex"></param>
        /// <returns></returns>
        public static string SafeGetString(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetString(colIndex);
            
            return string.Empty;
        }

        /// <summary>
        /// Safely handles null values when fetching integers from a DataReader
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="colIndex"></param>
        /// <returns></returns>
        public static int? SafeGetInt32(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetInt32(colIndex);
            
            return null;
        }
        public static int SafeGetInt16(this SqlDataReader reader, int colIndex)
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
        public static DateTime? GetSnitzDate(this SqlDataReader reader, int colIndex)
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
