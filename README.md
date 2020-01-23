#Auto Mapeamento e extração de dados via ADODB.Recordset

### Uso simples

ADOContext.ConnectionString = "Driver={MySQL ODBC 5.3 ANSI Driver};Server={yourServer};DataBase={yourDataBase};Uid=root;Pwd={yourPassWord};port={yourPort};Option=3;";

Recordset rs = ADOContext.MyExecute("select 1 as number1,2 as number2,3 as number3");

var myClass = new numbers().BindClassFromRS(ref rs);


private class numbers
{
    public int number1 { get; set; }
    public int number2 { get; set; }
    public int number3 { get; set; }
}
