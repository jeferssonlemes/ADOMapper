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

                if (!rs.CheckFieldExists(field))
                    throw new ArgumentOutOfRangeException(string.Format("O campo {0} não foi encontrado dentro do contexto do recordset, verifique", field));

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
        public static List<T> BindList<T>(this List<T> listClass, Recordset rs)
        {

            while (!rs.EOF)
            {
                T obj = (T)Activator.CreateInstance(typeof(T));

                listClass.Add(obj.BindClass(ref rs));
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
        public static T BindClass<T>(this T targetClass, ref Recordset rs)
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


                    string fieldName = property.Name;
                    bool ignoreBindField = false;
                    bool customDefaultValue = false;
                    dynamic defaultValue = null;

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
                                    {
                                        customDefaultValue = true;
                                        defaultValue = atribute.DefaultValue;
                                    }
                                       

                                    if (atribute.IgnoreField != false)
                                        ignoreBindField = true;

                                }

                            }
                            catch (Exception) { }
                        }
                    }

                    // se ainda está nada, tento setar um valor default
                    if(!ignoreBindField && !customDefaultValue && defaultValue == null)
                    {
                        defaultValue = typeProp.GetDefault();
                    }
                   

                    if (!ignoreBindField)
                    {
                        property.SetValue(targetClass, ADOUtil.GetValFromRS(rs, fieldName, defaultValue, typeProp));
                    }
                }

                return targetClass;
            }

            return default;
        }


        /// <summary>
        /// Faço uma busca a fim de verificar se esse field existe na coleção
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static bool CheckFieldExists(this Recordset rs, string field)
        {
            foreach (Field f in rs.Fields)
            {
                if (f.Name.ToString().ToLower().Equals(field.ToLower()))
                    return true;
            }

            return false;
        }
    }
}
