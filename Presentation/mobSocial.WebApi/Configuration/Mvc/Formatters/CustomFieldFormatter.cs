using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using mobSocial.Core.Infrastructure.Utils;
using mobSocial.Services.Extensions;
using mobSocial.WebApi.Configuration.Mvc.Models;
using mobSocial.WebApi.Models.CustomFields;
using Newtonsoft.Json;

namespace mobSocial.WebApi.Configuration.Mvc.Formatters
{
    public class CustomFieldFormatter : BufferedMediaTypeFormatter
    {

        public CustomFieldFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));

        }
        public override bool CanReadType(Type type)
        {

            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(WithCustomFieldsModel<>) ||
                    type.IsArray && type.GetElementType().GetGenericTypeDefinition() == typeof(WithCustomFieldsModel<>) ||
                    type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>) && type.GetGenericArguments()[0].GetGenericTypeDefinition() == typeof(WithCustomFieldsModel<>)
            );
        }

        public override bool CanWriteType(Type type)
        {
            return false;
        }

        public override object ReadFromStream(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var jsonBody = "";
            //read the body content
            using (var reader = new StreamReader(readStream))
            {
                jsonBody = reader.ReadToEnd();
            }

            if (type.IsArray)
            {
                //todo: to be added when needed
            }
            else
            {
                if (type.GetGenericTypeDefinition() == typeof(WithCustomFieldsModel<>))
                {
                    //get the argument types
                    var typeArguments = type.GetGenericArguments();
                    var modelType = typeArguments.First();

                    var modelInstance = CreateTypeInstance(type, modelType, jsonBody);
                    return modelInstance;
                }
            }
            return null;
        }

        private object CreateTypeInstance(Type singleObjectType, Type modelType, string jsonBody)
        {
            var passedValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonBody);
            if (passedValues == null)
                return null;

            //child instance
            var childInstance = Activator.CreateInstance(modelType);

            var customFields = new List<CustomFieldItemModel>();
            //set the properties of this instance
            foreach (var property in modelType.GetProperties())
            {
                if (!passedValues.ContainsKey(property.Name))
                    continue; //value wasn't passed or it's Id column which shouldn't be patched anyways

                //get passed value if exist
                var passedPropertyValue = passedValues[property.Name]?.ToString();
                try
                {
                    var compatibleValue = TypeConverter.CastPropertyValue(property, passedPropertyValue);
                    property.SetValue(childInstance, compatibleValue);
                }
                catch (Exception ex)
                {
                    // ignored
                }
            }

            //are there any extra fields?
            if (passedValues.ContainsKey(CustomFieldExtensions.CustomFieldsRequestKey))
            {
                //deserialize to another dictionary
                var customFieldsDictionary =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(passedValues[CustomFieldExtensions.CustomFieldsRequestKey].ToString());

                foreach (var ef in customFieldsDictionary)
                {
                    var efField = new CustomFieldItemModel()
                    {
                        FieldName = ef.Key,
                        FieldValue = ef.Value
                    };
                    customFields.Add(efField);
                }
            }

            //create the instance now
            var modelInstance = Activator.CreateInstance(singleObjectType);
            singleObjectType.GetProperty("Model").SetValue(modelInstance, childInstance);
            singleObjectType.GetProperty("SubmittedCustomFields").SetValue(modelInstance, customFields);
            return modelInstance;
        }
    }
}