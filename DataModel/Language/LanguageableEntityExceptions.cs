using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Language
{
    public class LanguageItemNotSupportedException : ApplicationException
    {
        public LanguageItemNotSupportedException(string languageItem, string entityName) :
            base($"Language item: {languageItem} is not supported by Entity: {entityName}")
        {

        }
    }

    public class LanguageAttributeNotSupportedException : ApplicationException
    {
        public LanguageAttributeNotSupportedException(string languageAttribute, string entityName) :
            base($"Language attribute: {languageAttribute} is not supported by Entity: {entityName}")
        {

        }
    }
}
