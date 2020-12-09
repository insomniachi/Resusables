﻿using KEI.Infrastructure.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;

namespace KEI.Infrastructure
{
    /// <summary>
    /// PropertyObject Implementation for storing <see cref="IList"/> of not primitive types
    /// </summary>
    internal class ContainerCollectionPropertyObject : PropertyObject
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ContainerCollectionPropertyObject(string name, IList value)
        {
            Name = name;

            int count = 0;
            foreach (var item in value)
            {
                Value.Add(PropertyContainerBuilder.CreateObject($"{name}[{count++}]", item));
            }

            CollectionType = value.GetType();
        }

        public ContainerCollectionPropertyObject(string name, ObservableCollection<IDataContainer> value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Type of <see cref="IList"/>
        /// </summary>
        public Type CollectionType { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public ObservableCollection<IDataContainer> Value { get; set; } = new ObservableCollection<IDataContainer>();

        /// <summary>
        /// Imlementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => "dcl";

        /// <summary>
        /// Implementation for <see cref="DataObject.GetDataType"/>
        /// </summary>
        /// <returns></returns>
        public override Type GetDataType()
        {
            return CollectionType;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.GetValue()"/>
        /// </summary>
        /// <returns></returns>
        public override object GetValue()
        {
            return Value;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.SetValue(object)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool SetValue(object value)
        {
            if(Value is not ObservableCollection<IDataContainer>)
            {
                return false;
            }

            Value = value as ObservableCollection<IDataContainer>;

            return true;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.GetStartElementName"/>
        /// </summary>
        /// <returns></returns>
        protected override string GetStartElementName()
        {
            return ContainerDataObject.DC_START_ELEMENT_NAME;
        }

        protected override bool CanWriteValueAsXmlAttribute() { return false; }

        /// <summary>
        /// Implementation for <see cref="DataObject.WriteXmlContent(XmlWriter)"/>
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlContent(XmlWriter writer)
        {
            // Write base impl
            base.WriteXmlContent(writer);

            // Write collection type
            if (CollectionType is not null)
            {
                writer.WriteObjectXml(new TypeInfo(CollectionType));
            }

            // Write values
            foreach (var dc in Value)
            {
                new ContainerPropertyObject(dc.Name, dc).WriteXml(writer);
            }
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.ReadXmlElement(string, XmlReader)"/>
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            // call base
            if(base.ReadXmlElement(elementName, reader))
            {
                return true;
            }

            /// Read DataObject implementation
            if (elementName == ContainerDataObject.DC_START_ELEMENT_NAME)
            {
                var obj = DataObjectFactory.GetPropertyObject("dc");

                if (obj is ContainerPropertyObject cdo)
                {
                    using var newReader = XmlReader.Create(new StringReader(reader.ReadOuterXml()));

                    newReader.Read();

                    cdo.ReadXml(newReader);

                    Value.Add(cdo.Value);
                }

                return true;
            }

            /// Read type info
            else if(elementName == nameof(TypeInfo))
            {
                CollectionType = reader.ReadObjectXml<TypeInfo>();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Implementatino for <see cref="DataObject.InitializeObject"/>
        /// </summary>
        protected override void InitializeObject()
        {
            Value = new ObservableCollection<IDataContainer>();
        }
    }
}
