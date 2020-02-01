using System;
using ADO.Mapper.Classes;
using ADODB;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADO.Test
{
    [TestClass]
    public class UnitTest
    {

        public UnitTest()
        {
            ADOContext.ConnectionString = "Driver={MySQL ODBC 5.3 ANSI Driver};Server=localhost;DataBase=base_mix_31122019;Uid=root;Pwd=bdcs;port=3308;Option=3;";
        }

        [TestMethod]
        public void GetData()
        {
            Recordset rs = ADOContext.MyExecute("select 1");

            Assert.AreEqual(1, rs.GetVal<int>("1"));
        }

        [TestMethod]
        public void BindData()
        {
            Recordset rs = ADOContext.MyExecute("select 1 as number1,2 as number2,3 as number3");

            var myClass = new Numbers().BindClass(ref rs);

            Assert.IsNotNull(myClass.number1);
            Assert.IsNotNull(myClass.number2);
            Assert.IsNotNull(myClass.number3);
        }


        [TestMethod]
        public void BindList()
        {
            Recordset rs = ADOContext.MyExecute("SELECT * FROM armazem");

            var myClass = new TestClass().BindClass(ref rs);

            Assert.IsNotNull(myClass.idArmazem);
            Assert.IsNotNull(myClass.Nome);
            Assert.IsNotNull(myClass.Status);
            Assert.IsNull(myClass.CampoExtra);

        }

        [TestMethod]
        public void TesteSabor()
        {
            Recordset rs = ADOContext.MyExecute("SELECT id_sabor_id,ativo,valor_adicional,nome,recheio,categoria_id FROM sabores ");

            var myClass = new Sabores().BindClass(ref rs);

            Assert.IsNotNull(myClass.id_sabor_id);
        }

        [TestMethod]
        public void TesteTabela()
        {
            Recordset rs = ADOContext.MyExecute("SELECT * FROM custom_rede_credenciada c WHERE c.ST_REGISTRO = 'G' AND c.ID_EMPRESA = 1 AND c.CLASSIFICACAO_REDE_CREDENCIADA_ID = 1");

            var myClass = new ClassRedeCredenciadoLiteModel().BindClass(ref rs);

            Assert.IsNotNull(myClass.ClassificacaoRedeCredenciadaId);
        }

        public class ClassRedeCredenciadoLiteModel
        {
            [ADOMap(FieldName = "ID_CUSTOM_REDE_CREDENCIADA_ID")] public int Id { get; set; }
            [ADOMap(FieldName = "NOME")] public string Nome { get; set; }
            [ADOMap(FieldName = "CLASSIFICACAO_REDE_CREDENCIADA_ID")] public int ClassificacaoRedeCredenciadaId { get; set; }
            [ADOMap(IgnoreField = true)] public Byte[] Imagem { get; set; }
            [ADOMap(FieldName = "IMAGEM_PERFIL")] public string ImagemStr { get; set; }
        }


        public class Sabores
        {
            public Sabores()
            {
                Number = new Numbers();
            }
            public long id_sabor_id { get; set; }
            public bool ativo { get; set; }
            public decimal valor_adicional { get; set; }
            public string nome { get; set; }
            public string recheio { get; set; }
            public long? categoria_id { get; set; }
            [ADOMap(IgnoreField = true)]
            public Numbers Number { get; set; }
        }

        public class Numbers
        {
            public int number1 { get; set; }
            public int number2 { get; set; }
            public int number3 { get; set; }
        }
    }
}
