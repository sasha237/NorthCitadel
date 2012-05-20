using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Xml;

namespace Fuel.AmfNet
{
   public class AmfNetConfiguration : ConfigurationSection
   {
      #region Fields and Properties
      private List< IReconstitutor > _reconstitutors;

      public ReadOnlyCollection< IReconstitutor > Reconstitutors
      {
         get { return _reconstitutors.AsReadOnly( ); }
      }
      #endregion

      #region Constructors and Factories
      public static AmfNetConfiguration GetConfig
      {
         get { return ( AmfNetConfiguration ) ConfigurationManager.GetSection( ( "Fuel/AmfNet" ) ); }
      }
      #endregion

      public void LoadValues(XmlNode node)
      {
         _reconstitutors = new List< IReconstitutor >( );

         foreach ( XmlNode childNode in node.ChildNodes )
         {
            switch ( childNode.Name.ToLower( ) )
            {
               case "reconstitutor":
                  ParseReconstitutor( childNode );
                  break;
            }
         }
      }

      #region Private Methods
      private void ParseReconstitutor( XmlNode node )
      {
         foreach ( XmlNode childNode in node.ChildNodes )
         {
            switch ( childNode.Name.ToLower( ) )
            {
               case "add":
                  if ( childNode.Attributes[ "type" ] == null )
                  {
                     throw new ConfigurationErrorsException( "No type attribute was specified when adding a reconstitutor." );
                  }

                  string typeName = childNode.Attributes[ "type" ].Value;

                  Type type = Type.GetType( typeName );

                  if ( type == null )
                  {
                     throw new ConfigurationErrorsException( string.Format( "Could not load reconstitutor type for {0}.", typeName ) );
                  }

                  object o = Activator.CreateInstance( type );

                  if ( ! ( o is IReconstitutor ) )
                  {
                     throw new ConfigurationErrorsException( "Attribute type specified for the reconstitutor does not implement IReconstitutor." );
                  }

                  _reconstitutors.Add( ( IReconstitutor ) o );
                  break;
            }
         }
      }
      #endregion
   }

   public class AmfNetConfigurationHandler : IConfigurationSectionHandler
   {
      public object Create( object parent, object context, XmlNode node )
      {
         AmfNetConfiguration config = new AmfNetConfiguration( );
         config.LoadValues( node );
         return config;
      }
   }
}