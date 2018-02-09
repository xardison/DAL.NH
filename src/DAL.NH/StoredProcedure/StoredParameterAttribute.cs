using System;

namespace DAL.NH.StoredProcedure
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class StoredParameterAttribute : Attribute
    {
        public StoredParameterAttribute(string parameterName, TypeParameter typeParameter = TypeParameter.Input)
        {
            ParameterName = parameterName;
            TypeParameter = typeParameter;
        }

        public string ParameterName { get; private set; }
        public TypeParameter TypeParameter { get; private set; }
    }
}
