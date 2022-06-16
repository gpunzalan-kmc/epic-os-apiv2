using System.Collections.Generic;
using System.Reflection;
using QueryDesignerCore;

namespace EPICOS_API.Helpers
{
    public class QueryDesigner<T>
    {
        public static FilterContainer Query(T filters)
        {
            var operands = new List<TreeFilter>();
            operands.Add(new TreeFilter{
                Field = "isDeleted",
                FilterType = WhereFilterType.Equal,
                Value = false
            });
            foreach(PropertyInfo propertyInfo in filters.GetType().GetProperties()){
                var name = propertyInfo.Name;
                var value1 = propertyInfo.GetValue(filters);
                if(value1 != null && !name.Contains("Page") && !name.Contains("Limit")){
                    if(!string.IsNullOrEmpty(value1.ToString())){
                        if(name.ToString() == "Name"){
                            operands.Add(new TreeFilter{
                            Field = name.ToString(),
                            FilterType = WhereFilterType.Contains,
                            Value = value1
                            });
                        }else {
                            operands.Add(new TreeFilter{
                            Field = name.ToString(),
                            FilterType = WhereFilterType.Equal,
                            Value = value1
                        });
                        }

                    }
                }
            }
            var filter = new FilterContainer();
            filter.Where = new TreeFilter{
                OperatorType = TreeFilterType.And,
                Operands = operands
            };
            return filter;
        }
    }
}