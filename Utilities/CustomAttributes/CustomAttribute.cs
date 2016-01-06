using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.CustomAttributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class CustomAttribute : Attribute
    {
        private List<string> _attributeValues;
        
        public CustomAttribute() : base()
        {
            _attributeValues = new List<string>();
        }

        public CustomAttribute(params string[] attributeValues):this()
        {
            _attributeValues = attributeValues.ToList();
        }

        public string ValuesString
        {
            get
            {
                return string.Join(", ", this._attributeValues);
            }
        }

        public List<string> AttributeValues
        {
            get
            {
                return this._attributeValues;
            }
        }
    }
}
