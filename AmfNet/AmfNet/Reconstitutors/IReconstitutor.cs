using System.Collections;

namespace Fuel.AmfNet
{
   public interface IReconstitutor
   {
      string RegisteredName { get; }
      bool ParametersAreValid(Hashtable parameters);
      object Reconstitute(Hashtable parameters);
   }
}