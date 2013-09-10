namespace Snitz.Entities
{
    public class ProfileColumn
    {
        public string ColumnName { get; set; }
        public bool AllowNull { get; set; }
        public string DataType { get; set; }
        public string DefaultValue { get; set; }
        public string Precision { get; set; }
    }
}