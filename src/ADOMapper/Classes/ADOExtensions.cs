using ADODB;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ADO.Mapper.Classes
{
    public static class ADOExtensions
    {
        /// <summary>
        /// Função para buscar valor de um campo recordset dinamicamente
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rs"></param>
        /// <param name="field"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static T GetVal<T>(this Recordset rs, string field, T defaultVal = default)
        {
            try
            {
                dynamic result;
                var type = typeof(T);

                // estando nulo, retorna o valor default
                if (rs.Fields[field].Value is DBNull || rs.Fields[field].Value == null)
                    return defaultVal;

                // busco o valor do ADO Field
                result = ADOUtil.GetValField(rs.Fields[field]);

                // verificar se essa opção está passando
                if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    type = Nullable.GetUnderlyingType(type);

                // converto o tipo e devolvo
                return Convert.ChangeType(result, type);
            }
            catch (Exception ex)
            {
                var msg = string.Format("Ocorreu um erro sem tratamento na função MyExtensions.ToInt(), field : {0}, mensagem : {1}", field, ex.Message);
                Console.WriteLine(msg);
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// Busca o valor padrão baseado no tipo do T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typeProp"></param>
        /// <returns></returns>
        public static dynamic GetDefault<T>(this T typeProp)
        {
            dynamic val = typeProp;
            if (val.IsValueType)
            {
                if (val.IsGenericType && val.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    return null;
                }
                else
                {
                    return Activator.CreateInstance(val);
                }
            }

            return default;
        }

        /// <summary>
        /// Faz o binding de um recordSet a uma Lista do tipo <T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listClass"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<T> BindListFromRS<T>(this List<T> listClass, Recordset rs)
        {

            while (!rs.EOF)
            {
                T obj = (T)Activator.CreateInstance(typeof(T));

                listClass.Add(obj.BindClassFromRS(ref rs));
                rs.MoveNext();
            }

            return listClass;
        }

        /// <summary>
        /// Faz o binding de um recordset a uma classe do tipo<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetClass"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static T BindClassFromRS<T>(this T targetClass, ref Recordset rs)
        {
            if (!rs.EOF)
            {
                var type = typeof(T);
                PropertyInfo[] props = type.GetProperties();

                foreach (var property in props)
                {
                    var atributes = property.GetCustomAttributes(true);
                    Type typeProp;
                    typeProp = Type.GetType(property.PropertyType.ToString());

                    var defaultValue = typeProp.GetDefault();
                    string fieldName = property.Name;


                    // pego todos os atributos, caso tenha
                    foreach (object atr in atributes)
                    {
                        if (atr.GetType().Equals(typeof(ADOMap)))
                        {
                            try
                            {
                                // caso tenha algo setado em {FieldName},{DefaultValue} seto isso
                                var atribute = (ADOMap)atr;
                                if (atribute != null)
                                {
                                    if (atribute.FieldName != null)
                                        fieldName = atribute.FieldName == "" ? property.Name : atribute.FieldName;

                                    if (atribute.DefaultValue != null)
                                        defaultValue = atribute.DefaultValue;
                                }

                            }
                            catch (Exception) { }
                        }
                    }

                    dynamic obj = ADOUtil.GetValFromRS(rs, fieldName, defaultValue, typeProp);


                    property.SetValue(targetClass, obj);
                }

                return targetClass;

            }

            return default;
        }

    }
}
