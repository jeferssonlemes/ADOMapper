using ADODB;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ADO.Mapper.Classes
{
    public static class ADOUtil
    {
        #region methods ADO
        /// <summary>
        /// Função para buscar valor de um campo recordset dinamicamente
        /// </summary>
        /// <param name="rs">recordset para buscar dados</param>
        /// <param name="field">nome field para buscar dentro do recordset</param>
        /// <param name="defaultVal">valor default para retorno</param>
        /// <param name="tipoCastFinal">type para forçar cast</param>
        /// <returns></returns>
        public static dynamic GetValFromRS(Recordset rs, string field, dynamic defaultVal, Type tipoCastFinal)
        {
            try
            {
                dynamic result;

                // verifico se o campo existe
                if (!rs.CheckFieldExists(field))
                    throw new ArgumentOutOfRangeException(string.Format("O campo {0} não foi encontrado dentro do contexto do recordset, verifique", field));

                var originalValue = rs.Fields[field].Value;
                Field fieldValue = rs.Fields[field];

                // caso tenha perdido a referência após a 1ª referência
                if (originalValue != null && (fieldValue.Value is DBNull || fieldValue.Value == null))
                {
                    result = GetValField(originalValue);
                }
                else
                {
                    // estando nulo, retorna o valor default
                    if (fieldValue.Value is DBNull || fieldValue.Value == null)
                        return defaultVal;

                    // busco o valor do ADO Field
                    result = GetValField(fieldValue);
                }


                // verificar se isso ta passando
                if (tipoCastFinal.IsGenericType && tipoCastFinal.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    tipoCastFinal = Nullable.GetUnderlyingType(tipoCastFinal);

                // no final converto pro tipo chamado
                return Convert.ChangeType(result, tipoCastFinal);
            }
            catch (Exception ex)
            {
                var msg = string.Format("Ocorreu um erro sem tratamento na função MyExtensions.GetVal(), field : {0}, mensagem : {1}", field, ex.Message);
                Console.WriteLine(msg);
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// Retorna o valor do field dinamicamente
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static dynamic GetValField(Field field)
        {
            switch (field.Value.GetType().ToString())
            {
                case "System.Boolean":
                    return (bool)field.Value;
                case "System.Byte":
                    return (byte)field.Value;
                case "System.SByte":
                    return (sbyte)field.Value;
                case "System.Char":
                    return (char)field.Value;
                case "System.Decimal":
                    return (decimal)field.Value;
                case "System.Double":
                    return (double)field.Value;
                case "System.Single":
                    return (float)field.Value;
                case "System.Int32":
                    return (int)field.Value;
                case "System.UInt32":
                    return (uint)field.Value;
                case "System.Int64":
                    return (long)field.Value;
                case "System.UInt64":
                    return (ulong)field.Value;
                case "System.Object":
                    return (object)field.Value;
                case "System.Int16":
                    return (short)field.Value;
                case "System.UInt16":
                    return (ushort)field.Value;
                case "System.String":
                    return (string)field.Value;
                case "System.DateTime":
                    return (DateTime)field.Value;
                default:
                    throw new NotImplementedException("Opção não encontrada");
            }
        }

        /// <summary>
        /// Retorna o valor do field dinamicamente
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static dynamic GetValField(object field)
        {
            switch (field.GetType().ToString())
            {
                case "System.Boolean":
                    return (bool)field;
                case "System.Byte":
                    return (byte)field;
                case "System.SByte":
                    return (sbyte)field;
                case "System.Char":
                    return (char)field;
                case "System.Decimal":
                    return (decimal)field;
                case "System.Double":
                    return (double)field;
                case "System.Single":
                    return (float)field;
                case "System.Int32":
                    return (int)field;
                case "System.UInt32":
                    return (uint)field;
                case "System.Int64":
                    return (long)field;
                case "System.UInt64":
                    return (ulong)field;
                case "System.Object":
                    return (object)field;
                case "System.Int16":
                    return (short)field;
                case "System.UInt16":
                    return (ushort)field;
                case "System.String":
                    return (string)field;
                case "System.DateTime":
                    return (DateTime)field;
                default:
                    throw new NotImplementedException("Opção não encontrada");
            }
        }


        public static long GetLastInsertId()
        {
            return ADOContext.MyExecute("SELECT LAST_INSERT_ID() ID").GetVal<long>("ID");
        }

        #endregion
    }

}

