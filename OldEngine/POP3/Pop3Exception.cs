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

namespace Pop3
{
    public class Pop3Exception : ApplicationException
    {
        #region Constructors

        public Pop3Exception( string errorMsg ) : base( errorMsg ) { }

        #endregion
    }
}