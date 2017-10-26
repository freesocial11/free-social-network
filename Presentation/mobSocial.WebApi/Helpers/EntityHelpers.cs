using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using mobSocial.Data.Extensions;

namespace mobSocial.WebApi.Helpers
{
    public static class EntityHelpers
    {
        public static List<dynamic> GetSelectItemList<T, TIdProperty, TTextProperty>(IList<T> entities, Expression<Func<T, TIdProperty>> idPropertyExpression, Expression<Func<T, TTextProperty>> textPropertyExpression)
        {
            
            var tType = typeof(T);
            var idMemberExpression = idPropertyExpression.Body as MemberExpression;
            if(idMemberExpression == null)
                throw new ArgumentException($"Expression {idPropertyExpression} refers a method, not a property");

            var idFieldName = idMemberExpression.Member.Name;

            var textMemberExpression = textPropertyExpression.Body as MemberExpression;
            if (textMemberExpression == null)
                throw new ArgumentException($"Expression {textPropertyExpression} refers a method, not a property");

            var textFieldName = textMemberExpression.Member.Name;

            //check if the fields asked actually exist
            var idProperty = tType.GetProperty(idFieldName);
            var textProperty = tType.GetProperty(textFieldName);

            //so we are good to go
            var selectList = new List<dynamic>();

            foreach (var entity in entities)
            {
                var idValue = idProperty.GetValue(entity)?.GetInteger() ?? 0;
                var textValue = textProperty.GetValue(entity)?.ToString() ?? "";
                selectList.Add(new
                {
                    Id = idValue,
                    Text = textValue
                });
            }
            return selectList;
        }

        public static List<dynamic> GetSelectItemList<T>(IEnumerable<T> restrictToList = null) where T : IConvertible
        {
            //so we are good to go
            var selectList = new List<dynamic>();
            foreach (var eType in System.Enum.GetValues(typeof(T)))
            {
                if (restrictToList != null && restrictToList.Any(x => x.ToString(CultureInfo.InvariantCulture) != eType.ToString()))
                    continue;
                var field = eType.GetType().GetField(eType.ToString());
                //do we have a description attribute
                var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                            as DescriptionAttribute;
                var id = (int)eType;
                var text = attribute == null ? eType.ToString() : attribute.Description;
                selectList.Add(new {
                    Id = id,
                    Text = text
                });
            }
            return selectList;
        }

        public static string GetEnumLabel<T>(T enumValue) where T : IConvertible
        {
            var field = enumValue.GetType().GetField(enumValue.ToString(CultureInfo.InvariantCulture));
            //do we have a description attribute
            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                        as DescriptionAttribute;
            return attribute == null ? enumValue.ToString(CultureInfo.InvariantCulture) : attribute.Description;
        }

        public static bool DoesEntitySupportCustomFields(string entityName)
        {
            switch (entityName.ToLower())
            {
                case "user":
                    return true;
                default:
                    return false;
            }
        }
    }
}