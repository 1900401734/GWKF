using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesDatas.DataAcess
{
    public class TypeSugarClass
    {
        /// <summary>
        /// 创建表Type
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName"></param>
        /// <param name="typelist"></param>
        /// <returns></returns>
        public static Type GetListType(SqlSugarClient db, string tableName, List<string> typelist)
        {
            var typeBuilder = db.DynamicBuilder().CreateClass(tableName,
             new SugarTable()
             {
                 TableDescription = tableName,//表备注
                                              //DisabledUpdateAll=true 可以禁止更新只创建
             });
            foreach (var typestr in typelist)
            {
                typeBuilder.CreateProperty(typestr,
                    typeof(string),
                    new SugarColumn() { Length = 200, IsNullable = true });
            }
            var type = typeBuilder.BuilderType();
            return type;
        }
    }
}
