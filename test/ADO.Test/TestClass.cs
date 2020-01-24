using ADO.Mapper.Classes;

namespace ADO.Test
{
    public class TestClass
    {
        [ADOMap(FieldName = "IDARMAZEM_ARMAZEM")]
        public int idArmazem { get; set; }
        public string Nome { get; set; }
        public string Status { get; set; }

        [ADOMap(IgnoreField = true)]
        public string CampoExtra { get; set; }
    }
}
