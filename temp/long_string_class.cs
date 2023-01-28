using System;
using System.Text;
using System.Net;

// .NET7
internal class LongString : IDisposable
{
    
    public override string ToString() { return this.payload ; }
    private string payload  = string.Empty ;

    public LongString ()
	{
		byte[] bytes = new byte[64 * 1024]; // 64KB
		Random rnd = new Random();
		rnd.NextBytes(bytes);
		string myString = Encoding.ASCII.GetString(bytes);
		//  In .NET Core use the WebUtility.UrlEncode method 
        // from the System.Net namespace .
		payload = WebUtility.UrlEncode(myString);
	}

#region IDisposable implementation with finalizer
private bool isDisposed = false;
public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
protected virtual void Dispose(bool disposing) {
  if (!isDisposed) {
    if (disposing) {
      // 
      if ((payload != null) && (false == string.IsNullOrEmpty(payload) ) )  
             payload = string.Empty ;
   }
  }
  isDisposed = true;
}
#endregion
}