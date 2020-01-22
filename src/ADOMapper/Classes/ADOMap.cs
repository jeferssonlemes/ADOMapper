using System;

namespace ADO.Mapper.Classes
{
    [System.AttributeUsage(System.AttributeTargets.Field | AttributeTargets.Property)]
    public partial class ADOMap : Attribute
    {
        public string FieldName;
        public object DefaultValue = default;
        public ADOMap() { }
    }
}
