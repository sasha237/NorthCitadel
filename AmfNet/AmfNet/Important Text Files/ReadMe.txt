AMF.NET is an open source alternative to Macromedia's Flash Remoting for .NET.

AMF.NET is primarily developed for .NET 2.0. Shortly after the release of a new version, a 1.x release will
be made.

*** Attention: Use at your own Risk***
Users of this library should be aware of it’s total infancy. Only those desperate for the functionality, 
or developers with a solid grasp of .NET programming and AMF should consider using this library.
Most types are intentionally internal until the library's codebase becomes more stable.


*** How to Use***
Until the library is more mature, the following documentation represents the extent of the help
available to the brave souls trying to use this package.

Step 1 -
In your project, add a referece to Fuel.AmfNet.dll 
 (one is available in the bin/debug folder, or you can compile your own in either debug/release)
 

Step 2 - 
Register "Fuel.AmfNet.GatewayHandler, Fuel.AmfNet" as an HttpHandler
  In the system.web element of your web.config, place the following lines:
  <httpHandlers>
   <add verb="*" path="gateway.aspx" type="Fuel.AmfNet.GatewayHandler, Fuel.AmfNet"/>
  </httpHandlers>

(if anyone can explain to me why Macromedia's implementation uses an HttpModule, please let me know)


Step 3 - 
Create functions like you normally would:
 namespace Company.Security
 {
    public class User
    {
      privat int _userId;
      private string _userName;
      public int UserId
      {
         get { return _userId; }
      }
      public string UserName
      {
        get { return _userName; }
        set { _userName = value; }
      }
   
      public User GetUser(int userId)
      {
         //implementation
         return foundUser;
      }
   }
 }
 
 Step 4 - 
 Setup your flash movie to use the gateway setup in step 2:
 var service:Service = new Service('http://www.openmymind.net/gateway.aspx', null, 'Company.Security, Company');
 
 The 1st parameter is the path to the gateway, this is directly tied to the path specified in Step 2.
 Note that unlike Macromedia's implementation, there is no physical file "gateway.aspx".  You can name
 this imaginary file anything you like in your web.config (so long as it ends with .aspx), and match the name
 in your flash movie
 
 The 2nd parameter is the logger to send debug messages to
 
 The 3rd parameter is the .NET type name to load. This follows the same rule as any .NET types.
 For example, if the assembly is located in the GAC, the fully quantified name must be specified.
 The format for assemblies located in the local bin folder is:
 Namespace.Class, AssemblyName
 
 If your class resides in a 2005 web project, the assembly name is likely "App_Code" unless you changed it
 
 
Step 5 -
Call the .NET method in flash, passing in appropriate parameters, and hookup callback to receive the response:
 
var pc:PendingCall = service.GetUser(1);
pc.responder = new RelayResponder(this, "onSuccess", "onFault");
function onSuccess(re:ResultEvent)
{	
	trace(re.result.UserName);
}

function onFault()
{
    trace("Fudge! :(");
}