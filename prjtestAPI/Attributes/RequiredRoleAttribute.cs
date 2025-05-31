using System;

namespace prjtestAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequiredRoleAttribute : Attribute
    {
        public string[] Roles { get; }

        public RequiredRoleAttribute(params string[] roles)
        {
            Roles = roles;
        }
    }
}