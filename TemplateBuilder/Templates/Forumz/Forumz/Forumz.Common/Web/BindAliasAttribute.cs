using System;
using System.ComponentModel;

namespace Forumz.Common.Web
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class BindAliasAttribute : Attribute
    {
        public BindAliasAttribute(string alias)
        {
            Alias = alias;
        }

        public string Alias { get; private set; }

        public override object TypeId
        {
            get { return Alias; }
        }

        #region Nested type: AliasedPropertyDescriptor

        public sealed class AliasedPropertyDescriptor : PropertyDescriptor
        {
            public AliasedPropertyDescriptor(string alias, PropertyDescriptor inner)
                : base(alias, null)
            {
                Inner = inner;
            }

            public PropertyDescriptor Inner { get; private set; }

            public override Type ComponentType
            {
                get { return Inner.ComponentType; }
            }

            public override bool IsReadOnly
            {
                get { return Inner.IsReadOnly; }
            }

            public override Type PropertyType
            {
                get { return Inner.PropertyType; }
            }

            public override bool CanResetValue(object component)
            {
                return Inner.CanResetValue(component);
            }

            public override object GetValue(object component)
            {
                return Inner.GetValue(component);
            }

            public override void ResetValue(object component)
            {
                Inner.ResetValue(component);
            }

            public override void SetValue(object component, object value)
            {
                Inner.SetValue(component, value);
            }

            public override bool ShouldSerializeValue(object component)
            {
                return Inner.ShouldSerializeValue(component);
            }
        }

        #endregion
    }
}