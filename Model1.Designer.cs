﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data.EntityClient;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;

[assembly: EdmSchemaAttribute()]

namespace AniSharp
{
    #region Contexts
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    public partial class AniSharpDBEntities : ObjectContext
    {
        #region Constructors
    
        /// <summary>
        /// Initializes a new AniSharpDBEntities object using the connection string found in the 'AniSharpDBEntities' section of the application configuration file.
        /// </summary>
        public AniSharpDBEntities() : base("name=AniSharpDBEntities", "AniSharpDBEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new AniSharpDBEntities object.
        /// </summary>
        public AniSharpDBEntities(string connectionString) : base(connectionString, "AniSharpDBEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new AniSharpDBEntities object.
        /// </summary>
        public AniSharpDBEntities(EntityConnection connection) : base(connection, "AniSharpDBEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        #endregion
    
        #region Partial Methods
    
        partial void OnContextCreated();
    
        #endregion
    
    }
    

    #endregion
    
    
}
