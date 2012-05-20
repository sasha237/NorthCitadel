/*
 * Author: Rodolfo Finochietti
 * Email: ml@pboard.com.ar
 * Web: http://weblogs.sockbyte.com.ar/rodolfof
 *
 * This work is licensed under the Creative Commons Attribution License. 
 * To view a copy of this license, visit http://creativecommons.org/licenses/by/2.0/ 
 * or send a letter to Creative Commons, 559 Nathan Abbott Way, Stanford, California 94305, USA.
 * 
 * No warranties expressed or implied, use at your own risk.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using NerZul.Core.Utils;

namespace Pop3
{
    public class Pop3Client : TcpClient
    {
        #region Constants

        public const int Pop3PortNumber = 110;

        #endregion

        #region Private Fields

        private bool _isConnected = false;

        #endregion

        #region Constructors

        public Pop3Client( ) { }

        #endregion

        #region Public Methods

        public void Connect( string server, string username, string password )
        {
            if ( _isConnected )
                throw new Pop3Exception( "Pop3 client already connected" );

            Connect( server, Pop3PortNumber );

            string response = Response( );
            if ( response.Substring( 0, 3 ) != "+OK" )
            {
                throw new Pop3Exception( response );
            }
            else
            {
                SendCommand( String.Format( CultureInfo.InvariantCulture, "USER {0}", username ) );
                SendCommand( String.Format( CultureInfo.InvariantCulture, "PASS {0}", password ) );
            }

            _isConnected = true;
        }

        public void Disconnect( )
        {
            if ( !_isConnected )
                return;

            try
            {
                SendCommand( "QUIT" );
            }
            finally
            {
                _isConnected = false;
            }
        }

        public List<Pop3Message> List( )
        {
            string response;

            List<Pop3Message> result = new List<Pop3Message>( );

            SendCommand( "LIST" );

            while ( true )
            {
                response = Response( );
                if ( response == ".\r\n" )
                {
                    return result;
                }
                else
                {
                    Pop3Message message = new Pop3Message( );
                    
                    char[ ] seps = { ' ' };
                    string[ ] values = response.Split( seps );
                    
                    message.Number = Int32.Parse( values[ 0 ] );
                    message.Bytes = Int32.Parse( values[ 1 ] );
                    message.Retrieved = false;
                    
                    result.Add( message );
                }
            }
        }

        public void RetrieveHeader( Pop3Message message )
        {
            string response;

			SendCommand( "TOP", "0", message );

            while ( true )
            {
                response = Response( );
                if ( response == ".\r\n" )
                    break;
                else
                    message.Header += response;
            }
        }
        
        public void Retrieve( Pop3Message message )
        {
            string response;

            SendCommand( "RETR", message );

            message.Retrieved = true;
            while ( true )
            {
                response = Response( );
                if ( response == ".\r\n" )
                    break;
                else
                    message.Message += response;
            }
        }

        public void Retrieve( List<Pop3Message> messages )
        {
            foreach ( Pop3Message message in messages )
                Retrieve( message );
        }

        public List<Pop3Message> ListAndRetrieve( )
        {
            List<Pop3Message> messages = List( );

            Retrieve( messages );

            return messages;
        }
        
        public void Delete( Pop3Message message )
        {
            try
            {
                SendCommand("DELE", message);
            }
            catch (Pop3Exception e)
            {
                ConsoleLog.WriteLine("Error deleteing message: " + e.Message);
            }
        }

        public void Delete(List<Pop3Message> messages)
        {
            foreach (Pop3Message message in messages)
                Delete(message);
        }
        
        public void Dispose( )
        {
            Dispose( true );

            GC.SuppressFinalize( this );
        }

        #endregion

        #region Private Methods

        protected override void Dispose( bool disposing )
        {
            try
            {
                Disconnect();
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Error: " + e.Message);
            }

            base.Dispose( disposing );
        }

        private void SendCommand( string command )
        {
            SendCommand( command, null, null );
        }

        private void SendCommand( string command, Pop3Message message )
        {
			SendCommand( command, null, message );
		}

		private void SendCommand( string command, string aditionalParameters, Pop3Message message )
		{
			StringBuilder request = new StringBuilder();
			string response;

			if ( message == null )
				request.AppendFormat( CultureInfo.InvariantCulture, "{0}", command );
			else
				request.AppendFormat( CultureInfo.InvariantCulture, "{0} {1}", command, message.Number );

			if ( !String.IsNullOrEmpty( aditionalParameters ))
				request.AppendFormat( " {0}", aditionalParameters );
				
			request.Append( "\r\n" );
			
			Write( request.ToString( ) );

			response = Response( );
			if ( response.Substring( 0, 3 ) != "+OK" )
				throw new Pop3Exception( response );
		}
		
		private void Write( string message )
        {
            ASCIIEncoding en = new ASCIIEncoding( );

            byte[ ] writeBuffer = new byte[ 1024 ];
            writeBuffer = en.GetBytes( message );

            NetworkStream stream = GetStream( );
            stream.Write( writeBuffer, 0, writeBuffer.Length );
        }

        private string Response( )
        {
            ASCIIEncoding enc = new ASCIIEncoding( );

            byte[] serverBuffer = new Byte[1048575];
            NetworkStream stream = GetStream( );

            int count = 0;

            while ( true )
            {
                byte[ ] buff = new Byte[ 2 ];
                int bytes = stream.Read( buff, 0, 1 );
                if ( bytes == 1 )
                {
                    serverBuffer[ count ] = buff[ 0 ];
                    count++;

                    if ( buff[ 0 ] == '\n' )
                        break;
                }
                else
                {
                    break;
                }
            }

            return enc.GetString( serverBuffer, 0, count );
        }

        #endregion
    }
}