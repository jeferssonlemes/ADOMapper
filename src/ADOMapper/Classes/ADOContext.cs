using ADODB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO.Mapper.Classes
{
    public static class ADOContext
    {
        #region fields
        public static Connection CN { get; set; }
        public static string ConnectionString { get; set; }
        #endregion

        #region methods
        /// <summary>
        /// abre uma nova conexão
        /// </summary>
        public static void OpenConnection()
        {
            if (string.IsNullOrEmpty(ConnectionString))
                throw new ArgumentException("Conection string em branco, verifique!");

            // só abre se não estiver aberta
            if (CN == null || CN.State != 1)
            {
                CN = new Connection();
                CN.CursorLocation = CursorLocationEnum.adUseServer;
                CN.ConnectionString = ConnectionString;
                CN.IsolationLevel = IsolationLevelEnum.adXactReadCommitted;
                CN.ConnectionTimeout = 15;
                CN.Mode = ConnectModeEnum.adModeReadWrite;
                CN.CommandTimeout = 30;
                CN.Open();
                // tamanho maximo do group concat
                CN.Execute("SET GROUP_CONCAT_MAX_LEN=1000000", out _);
                //max seta no banco tamanho arquivo 16 mb.
                CN.Execute("SET GLOBAL max_allowed_packet=16777216", out _);
            }


        }

        /// <summary>
        /// fecha a conexão
        /// </summary>
        private static void CloseConnection()
        {
            if (!(CN == null))
            {
                if (CN.State == 1)
                {
                    CN.Close();
                }
                CN = null;
            }
        }

        /// <summary>
        /// executa as query sql
        /// </summary>
        /// <param name="sql">sql a ser executado</param>
        /// <param name="nrReg">quantidade de registros afetados</param>
        /// <returns></returns>
        public static Recordset MyExecute(string sql)
        {
            CloseConnection();
            OpenConnection();
                       
            try
            {
                // marco a execução como assyncrona
                var ret = CN.Execute(sql, out _);

                // mantenho um loop até que tenha acabado de executar
                //  while (CN.State == 5) { }

                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro na função ADODBContext.MyExecute(), Mensagem: " + ex.Message);
            }
        }

        #endregion
    }
}
