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
            ADOContext.ConnectionString = "Driver={MySQL ODBC 5.3 ANSI Driver};Server={yourServer};DataBase={yourDataBase};Uid=root;Pwd={yourPassWord};port={yourPort};Option=3;";
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

            var myClass = new numbers().BindClassFromRS(ref rs);

            Assert.IsNotNull(myClass.number1);
            Assert.IsNotNull(myClass.number2);
            Assert.IsNotNull(myClass.number3);
        }

        private class numbers
        {
            public int number1 { get; set; }
            public int number2 { get; set; }
            public int number3 { get; set; }
        }
    }
}
